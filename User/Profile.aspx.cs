
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class Profile : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["ExpenseTrackerDB"]
                    .ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private int GetUserId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void LoadProfile()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT FullName, Email, Role, CreatedAt
                      FROM dbo.Users
                      WHERE UserId = @UserId", con))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = GetUserId();

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtFullName.Text = Convert.ToString(reader["FullName"]);
                            txtEmail.Text = Convert.ToString(reader["Email"]);
                            lblAccountType.Text = Convert.ToString(reader["Role"]);

                            lblCreatedAt.Text = Convert.ToDateTime(reader["CreatedAt"])
                                .ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            Session.Clear();
                            Response.Redirect("~/User/Login.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowPageMessage("Unable to load profile. Please try again.", false);
            }
        }

        // SAVE PROFILE
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            lblPersonalMessage.Text = "";

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
            {
                ShowPersonalMessage(
                    "Enter a name containing no more than 100 characters.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || email.Length > 150)
            {
                ShowPersonalMessage("Enter a valid email address.", false);
                return;
            }

            try
            {
                MailAddress parsedEmail = new MailAddress(email);

                if (!string.Equals(parsedEmail.Address, email,
                    StringComparison.OrdinalIgnoreCase))
                {
                    ShowPersonalMessage("Enter a valid email address.", false);
                    return;
                }
            }
            catch (FormatException)
            {
                ShowPersonalMessage("Enter a valid email address.", false);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    using (SqlCommand check = new SqlCommand(
                        @"SELECT COUNT(*) FROM dbo.Users
                          WHERE Email = @Email AND UserId <> @UserId", con))
                    {
                        check.Parameters.Add("@Email", SqlDbType.NVarChar, 150)
                            .Value = email;
                        check.Parameters.Add("@UserId", SqlDbType.Int)
                            .Value = GetUserId();

                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        {
                            ShowPersonalMessage(
                                "This email is already used by another account.", false);
                            return;
                        }
                    }

                    using (SqlCommand update = new SqlCommand(
                        @"UPDATE dbo.Users
                          SET FullName = @FullName, Email = @Email
                          WHERE UserId = @UserId", con))
                    {
                        update.Parameters.Add("@FullName", SqlDbType.NVarChar, 100)
                            .Value = fullName;
                        update.Parameters.Add("@Email", SqlDbType.NVarChar, 150)
                            .Value = email;
                        update.Parameters.Add("@UserId", SqlDbType.Int)
                            .Value = GetUserId();

                        if (update.ExecuteNonQuery() > 0)
                        {
                            Session["FullName"] = fullName;
                            Session["Email"] = email;

                            ShowPersonalMessage(
                                "Profile updated successfully!", true);
                        }
                        else
                        {
                            ShowPersonalMessage(
                                "Profile was not updated. Please log in again.", false);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                ShowPersonalMessage(
                    "Database error while saving your profile. Please try again.", false);
            }
            catch (Exception)
            {
                ShowPersonalMessage(
                    "Unable to save your profile. Please try again.", false);
            }
        }

        // CHANGE PASSWORD
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowPasswordMessage("Fill in all three password fields.", false);
                return;
            }

            if (newPassword.Length < 8 || newPassword.Length > 200)
            {
                ShowPasswordMessage(
                    "The new password must be between 8 and 200 characters.", false);
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowPasswordMessage(
                    "The new password and confirmation do not match.", false);
                return;
            }

            if (currentPassword == newPassword)
            {
                ShowPasswordMessage(
                    "Choose a new password different from your current password.", false);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    string storedHash;

                    using (SqlCommand select = new SqlCommand(
                        "SELECT PasswordHash FROM dbo.Users WHERE UserId = @UserId", con))
                    {
                        select.Parameters.Add("@UserId", SqlDbType.Int).Value = GetUserId();

                        object result = select.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            ShowPasswordMessage("Account not found. Please log in again.", false);
                            return;
                        }

                        storedHash = Convert.ToString(result);
                    }

                    if (!VerifyPassword(currentPassword, storedHash))
                    {
                        ShowPasswordMessage("Your current password is incorrect.", false);
                        return;
                    }

                    string newHash = HashPassword(newPassword);

                    using (SqlCommand update = new SqlCommand(
                        @"UPDATE dbo.Users
                          SET PasswordHash = @PasswordHash
                          WHERE UserId = @UserId", con))
                    {
                        update.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500)
                            .Value = newHash;
                        update.Parameters.Add("@UserId", SqlDbType.Int)
                            .Value = GetUserId();

                        if (update.ExecuteNonQuery() > 0)
                        {
                            txtCurrentPassword.Text = "";
                            txtNewPassword.Text = "";
                            txtConfirmPassword.Text = "";

                            ShowPasswordMessage("Password changed successfully!", true);
                        }
                        else
                        {
                            ShowPasswordMessage("Password could not be changed.", false);
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowPasswordMessage(
                    "Unable to change your password. Please try again.", false);
            }
        }

        // Creates a salted PBKDF2 password hash.
        // Must match the format used by the existing Login/Register code.
        private string HashPassword(string password)
        {
            const int iterations = 100000;
            byte[] salt = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(password, salt, iterations))
            {
                hash = pbkdf2.GetBytes(32);
            }

            return "PBKDF2$" + iterations + "$" +
                Convert.ToBase64String(salt) + "$" +
                Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                string[] parts = storedHash.Split('$');

                if (parts.Length != 4 || parts[0] != "PBKDF2")
                    return false;

                int iterations;

                if (!int.TryParse(parts[1], out iterations) ||
                    iterations < 10000 || iterations > 1000000)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);

                if (salt.Length != 16 || expectedHash.Length != 32)
                    return false;

                byte[] actualHash;

                using (Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    actualHash = pbkdf2.GetBytes(32);
                }

                int difference = 0;

                for (int i = 0; i < expectedHash.Length; i++)
                    difference |= expectedHash[i] ^ actualHash[i];

                return difference == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ShowPageMessage(string message, bool success)
        {
            lblPageMessage.Text = message;
            lblPageMessage.ForeColor = success
                ? System.Drawing.Color.LightGreen
                : System.Drawing.Color.OrangeRed;
        }

        private void ShowPersonalMessage(string message, bool success)
        {
            lblPersonalMessage.Text = message;
            lblPersonalMessage.ForeColor = success
                ? System.Drawing.Color.LightGreen
                : System.Drawing.Color.OrangeRed;
        }

        private void ShowPasswordMessage(string message, bool success)
        {
            lblPasswordMessage.Text = message;
            lblPasswordMessage.ForeColor = success
                ? System.Drawing.Color.LightGreen
                : System.Drawing.Color.OrangeRed;
        }
    }
}
