using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class Login : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please enter your email and password.";
                return;
            }

            try
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"];

                if (settings == null)
                {
                    lblMessage.Text = "Database connection is missing.";
                    return;
                }

                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    string query = @"
                        SELECT UserId, FullName, Email, PasswordHash, [Role]
                        FROM dbo.Users
                        WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@Email", SqlDbType.NVarChar, 256).Value = email;

                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                lblMessage.Text = "Invalid email or password.";
                                return;
                            }

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
                                    passwordMatches = false;
                                }
                            }

                            if (!passwordMatches)
                            {
                                lblMessage.Text =
                                    "Invalid email or password.";
                                return;
                            }

                            string userId = reader["UserId"].ToString();
                            string fullName = reader["FullName"].ToString();
                            string userEmail = reader["Email"].ToString();
                            string role = reader["Role"].ToString();

                            Session["UserId"] = userId;
                            Session["FullName"] = fullName;
                            Session["Username"] = fullName;
                            Session["Email"] = userEmail;
                            Session["Role"] = role;

                            reader.Close();

                            if (role.Equals(
                                "Admin",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                Response.Redirect(
                                    "~/Admin/AdminDashboard.aspx", false);
                            }
                            else
                            {
                                Response.Redirect(
                                    "~/User/Dashboard.aspx", false);
                            }

                            Context.ApplicationInstance.CompleteRequest();
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to log in. Please check your connection and try again.";
            }
        }
    }
}