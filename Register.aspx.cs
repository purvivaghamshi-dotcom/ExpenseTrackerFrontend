
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ExpenseTrackerFrontend
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear an old registration message when the page opens.
                lblMessage.Text = "";
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (!Page.IsValid)
                return;

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim().ToLowerInvariant();
            string password = txtPassword.Text;

            if (fullName.Length == 0 || fullName.Length > 100)
            {
                ShowMessage("Please enter a valid full name.");
                return;
            }

            if (email.Length == 0 || email.Length > 256 ||
                !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowMessage("Please enter a valid email address.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                ShowMessage("Password must contain 8 to 128 characters.");
                return;
            }

            if (!string.Equals(password, txtConfirmPassword.Text,
                StringComparison.Ordinal))
            {
                ShowMessage("Passwords do not match.");
                return;
            }

            string connectionString =
                ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"]
                .ConnectionString;

            // PBKDF2 password hash; the salt is stored with the hash.
            string passwordHash = HashPassword(password);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check whether the email is already registered.
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email", con))
                    {
                        checkCmd.Parameters.Add(
                            "@Email", SqlDbType.NVarChar, 256).Value = email;

                        int existingUsers =
                            Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingUsers > 0)
                        {
                            ShowMessage(
                                "This email is already registered. Please log in.");
                            return;
                        }
                    }

                    // Always create public registrations as ordinary users.
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO dbo.Users
                          (FullName, Email, PasswordHash, Role)
                          VALUES
                          (@FullName, @Email, @PasswordHash, @Role)", con))
                    {
                        cmd.Parameters.Add(
                            "@FullName", SqlDbType.NVarChar, 100).Value =
                            fullName;

                        cmd.Parameters.Add(
                            "@Email", SqlDbType.NVarChar, 256).Value = email;

                        cmd.Parameters.Add(
                            "@PasswordHash", SqlDbType.NVarChar, 500).Value =
                            passwordHash;

                        cmd.Parameters.Add(
                            "@Role", SqlDbType.NVarChar, 20).Value = "User";

                        cmd.ExecuteNonQuery();
                    }
                }

                // Do not log the user in automatically yet.
                // Login will verify the stored hash in the next step.
                Response.Redirect(
                    ResolveUrl("~/User/Login.aspx"), false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException ex)
            {
                // SQL error 2601/2627 means a unique value already exists.
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    ShowMessage(
                        "This email is already registered. Please log in.");
                }
                else
                {
                    System.Diagnostics.Trace.TraceError(
                        "Registration database error: " + ex);

                    ShowMessage(
                        "Registration could not be completed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Registration error: " + ex);

                ShowMessage(
                    "Registration could not be completed. Please try again.");
            }
        }

        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            const int iterations = 100000;
            const int hashLength = 32;

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(hashLength);

                // Format: algorithm marker, iteration count, salt, hash.
                return "PBKDF2$" + iterations + "$" +
                    Convert.ToBase64String(salt) + "$" +
                    Convert.ToBase64String(hash);
            }
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}
