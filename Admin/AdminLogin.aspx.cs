
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Web.UI;

namespace ExpenseTrackerFrontend.Admin
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void btnAdminLogin_Click(object sender, EventArgs e)
        {
            string email = txtAdminEmail.Text.Trim();
            string password = txtAdminPassword.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                lblAdminMessage.Text =
                    "Please enter your admin email and password.";
                return;
            }

            try
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings[
                        "ExpenseTrackerDB"];

                if (settings == null)
                {
                    lblAdminMessage.Text =
                        "Database connection configuration is missing.";
                    return;
                }

                bool loginSuccess = false;
                string userId = "";
                string fullName = "";
                string userEmail = "";

                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    string query = @"
                        SELECT UserId, FullName, Email, PasswordHash, [Role]
                        FROM dbo.Users
                        WHERE Email = @Email";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@Email", SqlDbType.NVarChar, 256).Value =
                            email;

                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedRole =
                                    reader["Role"] == DBNull.Value
                                    ? ""
                                    : reader["Role"].ToString();

                                string storedHash =
                                    reader["PasswordHash"] == DBNull.Value
                                    ? ""
                                    : reader["PasswordHash"].ToString();

                                bool passwordMatches = false;

                                if (!string.IsNullOrWhiteSpace(storedHash))
                                {
                                    try
                                    {
                                        passwordMatches =
                                            Crypto.VerifyHashedPassword(
                                                storedHash, password);
                                    }
                                    catch
                                    {
                                        // Reject malformed or incompatible hashes.
                                        passwordMatches = false;
                                    }
                                }

                                // Both conditions must be true:
                                // correct password and Admin role.
                                if (passwordMatches &&
                                    storedRole.Equals(
                                        "Admin",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    userId = reader["UserId"].ToString();
                                    fullName = reader["FullName"].ToString();
                                    userEmail = reader["Email"].ToString();

                                    loginSuccess = true;
                                }
                            }
                        }
                    }
                }

                if (!loginSuccess)
                {
                    lblAdminMessage.Text =
                        "Invalid credentials or administrator access denied.";
                    return;
                }

                // Create a fresh authenticated admin session.
                Session.Clear();
                Session["UserId"] = userId;
                Session["FullName"] = fullName;
                Session["Username"] = fullName;
                Session["Email"] = userEmail;
                Session["Role"] = "Admin";

                Response.Redirect(
                    "~/Admin/AdminDashboard.aspx", false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                lblAdminMessage.Text =
                    "Unable to log in. Please check the database connection and try again.";
            }
        }
    }
}
