
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class Register : Page
    {
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            lblMessage.Visible = true;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                lblMessage.Text = "Please fill in all required fields.";
                return;
            }

            if (password != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            if (password.Length < 8)
            {
                lblMessage.Text =
                    "Password must contain at least 8 characters.";
                return;
            }

            try
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[
                        "ExpenseTrackerDB"];

                if (settings == null)
                {
                    lblMessage.Text =
                        "Database connection configuration is missing.";
                    return;
                }

                // Generate and verify the hash using one consistent method.
                string passwordHash = Crypto.HashPassword(password);

                if (!Crypto.VerifyHashedPassword(passwordHash, password))
                {
                    lblMessage.Text =
                        "Password hashing verification failed. Nothing was saved.";
                    return;
                }

                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    con.Open();

                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        try
                        {
                            // Prevent duplicate registrations.
                            string checkQuery = @"
                                SELECT COUNT(*)
                                FROM dbo.Users
                                WHERE Email = @Email;";

                            using (SqlCommand checkCmd =
                                new SqlCommand(checkQuery, con, tx))
                            {
                                checkCmd.Parameters.Add(
                                    "@Email", SqlDbType.NVarChar, 256)
                                    .Value = email;

                                int count = Convert.ToInt32(
                                    checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    tx.Rollback();
                                    lblMessage.Text =
                                        "This email is already registered. Please log in.";
                                    return;
                                }
                            }

                            string insertQuery = @"
                                INSERT INTO dbo.Users
                                    (FullName, Email, PasswordHash, [Role], CreatedAt)
                                VALUES
                                    (@FullName, @Email, @PasswordHash, @Role, GETDATE());";

                            using (SqlCommand cmd =
                                new SqlCommand(insertQuery, con, tx))
                            {
                                cmd.Parameters.Add(
                                    "@FullName", SqlDbType.NVarChar, 150)
                                    .Value = fullName;

                                cmd.Parameters.Add(
                                    "@Email", SqlDbType.NVarChar, 256)
                                    .Value = email;

                                cmd.Parameters.Add(
                                    "@PasswordHash", SqlDbType.NVarChar, 500)
                                    .Value = passwordHash;

                                cmd.Parameters.Add(
                                    "@Role", SqlDbType.NVarChar, 50)
                                    .Value = "User";

                                cmd.ExecuteNonQuery();
                            }

                            // Read back exactly what SQL Server stored.
                            string savedHash;

                            using (SqlCommand verifyCmd = new SqlCommand(
                                @"SELECT PasswordHash
                                  FROM dbo.Users
                                  WHERE Email = @Email;",
                                con, tx))
                            {
                                verifyCmd.Parameters.Add(
                                    "@Email", SqlDbType.NVarChar, 256)
                                    .Value = email;

                                object result = verifyCmd.ExecuteScalar();

                                savedHash = result == null ||
                                            result == DBNull.Value
                                    ? ""
                                    : result.ToString();
                            }

                            bool savedHashWorks = false;

                            if (!string.IsNullOrWhiteSpace(savedHash))
                            {
                                try
                                {
                                    savedHashWorks =
                                        Crypto.VerifyHashedPassword(
                                            savedHash, password);
                                }
                                catch (FormatException)
                                {
                                    savedHashWorks = false;
                                }
                            }

                            if (!savedHashWorks)
                            {
                                tx.Rollback();

                                lblMessage.Text =
                                    "The saved password hash could not be verified. " +
                                    "Registration was cancelled. Check your database " +
                                    "and password-hashing configuration.";

                                return;
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            try
                            {
                                tx.Rollback();
                            }
                            catch
                            {
                                // The transaction may already be completed.
                            }

                            throw;
                        }
                    }
                }

                Response.Redirect("~/User/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Registration failed. Please check the database connection " +
                    "and try again.";
            }
        }
    }
}
