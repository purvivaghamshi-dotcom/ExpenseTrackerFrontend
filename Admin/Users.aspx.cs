
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExpenseTrackerFrontend.Admin
{
    public partial class Users : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);

            if (Session["UserId"] == null ||
                !role.Equals("Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Admin/AdminLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                BindUsers();
            }
        }

        private string GetConnectionString()
        {
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"];

            if (settings == null)
            {
                throw new InvalidOperationException(
                    "ExpenseTrackerDB connection string was not found.");
            }

            return settings.ConnectionString;
        }

        private void BindUsers()
        {
            string searchText = txtSearchUsers.Text.Trim();

            string query = @"
                SELECT
                    u.UserId,
                    u.FullName,
                    u.Email,
                    ISNULL(u.Role, 'User') AS Role,
                    u.CreatedAt,
                    COUNT(t.TransactionId) AS TransactionCount

                FROM dbo.Users AS u

                LEFT JOIN dbo.Transactions AS t
                    ON t.UserId = u.UserId

                WHERE
                    @SearchText = N''
                    OR CONVERT(NVARCHAR(20), u.UserId)
                        LIKE @SearchPattern
                    OR ISNULL(u.FullName, N'')
                        LIKE @SearchPattern
                    OR ISNULL(u.Email, N'')
                        LIKE @SearchPattern
                    OR ISNULL(u.Role, N'')
                        LIKE @SearchPattern

                GROUP BY
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.CreatedAt

                ORDER BY
                    u.CreatedAt DESC,
                    u.UserId DESC;";

            try
            {
                DataTable table = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@SearchText",
                        SqlDbType.NVarChar,
                        256).Value = searchText;

                    cmd.Parameters.Add(
                        "@SearchPattern",
                        SqlDbType.NVarChar,
                        260).Value = "%" + searchText + "%";

                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }

                gvUsers.DataSource = table;
                gvUsers.DataBind();

                if (table.Rows.Count == 0)
                {
                    lblMessage.Text = "No matching users found.";
                }
                else
                {
                    lblMessage.Text =
                        "Total matching accounts: " +
                        table.Rows.Count;
                }
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to load users. Check the database connection " +
                    "and table structure.";

                gvUsers.DataSource = null;
                gvUsers.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            pnlUserDetails.Visible = false;
            BindUsers();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchUsers.Text = "";
            pnlUserDetails.Visible = false;
            BindUsers();
        }

        protected void gvUsers_RowCommand(
            object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
            {
                return;
            }

            int userId;

            if (!int.TryParse(
                e.CommandArgument.ToString(), out userId))
            {
                lblMessage.Text = "Invalid user selection.";
                return;
            }

            if (e.CommandName == "ViewUser")
            {
                LoadUserDetails(userId);
            }
            else if (e.CommandName == "DeleteUser")
            {
                DeleteUser(userId);
            }
        }

        private void LoadUserDetails(int userId)
        {
            string query = @"
                SELECT UserId, FullName, Email, [Role], CreatedAt
                FROM dbo.Users
                WHERE UserId = @UserId;";

            try
            {
                using (SqlConnection con =
                    new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@UserId", SqlDbType.Int).Value = userId;

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblDetailId.Text =
                                reader["UserId"].ToString();

                            lblDetailName.Text =
                                reader["FullName"].ToString();

                            lblDetailEmail.Text =
                                reader["Email"].ToString();

                            lblDetailRole.Text =
                                reader["Role"] == DBNull.Value
                                ? "User"
                                : reader["Role"].ToString();

                            if (reader["CreatedAt"] != DBNull.Value)
                            {
                                lblDetailCreatedAt.Text =
                                    Convert.ToDateTime(
                                        reader["CreatedAt"])
                                    .ToString("dd MMM yyyy");
                            }
                            else
                            {
                                lblDetailCreatedAt.Text = "Not available";
                            }

                            pnlUserDetails.Visible = true;
                            lblMessage.Text = "User details loaded.";
                        }
                        else
                        {
                            pnlUserDetails.Visible = false;
                            lblMessage.Text = "User account not found.";
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to load the selected user's details.";
            }
        }

        private void DeleteUser(int userId)
        {
            string query = @"
                SELECT
                    [Role],
                    (SELECT COUNT(*)
                     FROM dbo.Transactions
                     WHERE UserId = @UserId) AS TransactionCount,
                    (SELECT COUNT(*)
                     FROM dbo.Budgets
                     WHERE UserId = @UserId) AS BudgetCount
                FROM dbo.Users
                WHERE UserId = @UserId;";

            try
            {
                using (SqlConnection con =
                    new SqlConnection(GetConnectionString()))
                {
                    con.Open();

                    using (SqlTransaction transaction =
                        con.BeginTransaction())
                    {
                        try
                        {
                            string userRole;
                            int transactionCount;
                            int budgetCount;

                            using (SqlCommand cmd =
                                new SqlCommand(query, con, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@UserId", SqlDbType.Int).Value = userId;

                                using (SqlDataReader reader =
                                    cmd.ExecuteReader())
                                {
                                    if (!reader.Read())
                                    {
                                        reader.Close();
                                        transaction.Rollback();

                                        lblMessage.Text =
                                            "User account not found.";
                                        return;
                                    }

                                    userRole =
                                        reader["Role"] == DBNull.Value
                                        ? "User"
                                        : reader["Role"].ToString();

                                    transactionCount =
                                        Convert.ToInt32(
                                            reader["TransactionCount"]);

                                    budgetCount =
                                        Convert.ToInt32(
                                            reader["BudgetCount"]);
                                }
                            }

                            if (userRole.Equals(
                                "Admin",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                transaction.Rollback();
                                lblMessage.Text =
                                    "Administrator accounts cannot be deleted here.";
                                return;
                            }

                            if (transactionCount > 0 || budgetCount > 0)
                            {
                                transaction.Rollback();

                                lblMessage.Text =
                                    "This account has linked transactions or " +
                                    "budgets. It cannot be deleted to protect " +
                                    "financial records.";

                                BindUsers();
                                return;
                            }

                            string deleteQuery = @"
                                DELETE FROM dbo.Users
                                WHERE UserId = @UserId
                                  AND ISNULL([Role], 'User') <> 'Admin';";

                            int rowsDeleted;

                            using (SqlCommand deleteCmd =
                                new SqlCommand(
                                    deleteQuery, con, transaction))
                            {
                                deleteCmd.Parameters.Add(
                                    "@UserId", SqlDbType.Int).Value = userId;

                                rowsDeleted = deleteCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            pnlUserDetails.Visible = false;

                            lblMessage.Text = rowsDeleted == 1
                                ? "User account deleted successfully."
                                : "The account was not deleted.";

                            BindUsers();
                        }
                        catch
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch
                            {
                                // The transaction may already be completed.
                            }

                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to delete this account. It may have related " +
                    "records or a database constraint.";
            }
        }
    }
}
