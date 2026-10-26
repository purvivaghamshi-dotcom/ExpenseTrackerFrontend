
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace ExpenseTrackerFrontend.Admin
{
    public partial class AdminDashboard : Page
    {
        private readonly CultureInfo indianCulture =
            new CultureInfo("en-IN");

        protected void Page_Load(object sender, EventArgs e)
        {
            // Protect the Admin Dashboard.
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
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"];

            if (settings == null)
            {
                lblDashboardMessage.Text =
                    "The database connection setting was not found.";
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    con.Open();

                    LoadStatistics(con);
                    LoadRecentActivity(con);
                }
            }
            catch (Exception)
            {
                lblDashboardMessage.Text =
                    "Unable to load dashboard data. Check the database " +
                    "connection and table structure.";
            }
        }

        private void LoadStatistics(SqlConnection con)
        {
            string query = @"
                SELECT
                    (
                        SELECT COUNT(*)
                        FROM dbo.Users
                        WHERE ISNULL([Role], 'User') <> 'Admin'
                    ) AS TotalUsers,

                    (
                        SELECT COUNT(*)
                        FROM dbo.Transactions
                    ) AS TotalTransactions,

                    (
                        SELECT ISNULL(
                            SUM(
                                CASE
                                    WHEN LOWER(LTRIM(RTRIM(TransactionType)))
                                         = 'income'
                                    THEN Amount
                                    ELSE 0
                                END
                            ), 0
                        )
                        FROM dbo.Transactions
                    ) AS TotalIncome,

                    (
                        SELECT ISNULL(
                            SUM(
                                CASE
                                    WHEN LOWER(LTRIM(RTRIM(TransactionType)))
                                         = 'expense'
                                    THEN Amount
                                    ELSE 0
                                END
                            ), 0
                        )
                        FROM dbo.Transactions
                    ) AS TotalExpenses;";

            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int totalUsers =
                        Convert.ToInt32(reader["TotalUsers"]);

                    int totalTransactions =
                        Convert.ToInt32(reader["TotalTransactions"]);

                    decimal totalIncome =
                        Convert.ToDecimal(reader["TotalIncome"]);

                    decimal totalExpenses =
                        Convert.ToDecimal(reader["TotalExpenses"]);

                    lblTotalUsers.Text =
                        totalUsers.ToString("N0", indianCulture);

                    lblTotalTransactions.Text =
                        totalTransactions.ToString("N0", indianCulture);

                    lblTotalIncome.Text =
                        "₹" + totalIncome.ToString("N0", indianCulture);

                    lblTotalExpenses.Text =
                        "₹" + totalExpenses.ToString("N0", indianCulture);
                }
            }
        }

        private void LoadRecentActivity(SqlConnection con)
        {
            string query = @"
                SELECT TOP (10)
                    RecentItems.UserName,
                    RecentItems.Activity,
                    RecentItems.ActivityDate
                FROM
                (
                    SELECT
                        ISNULL(
                            NULLIF(u.FullName, ''),
                            'Unknown User'
                        ) AS UserName,

                        CASE
                            WHEN LOWER(LTRIM(RTRIM(t.TransactionType)))
                                 = 'income'
                                THEN 'Added Income'
                            WHEN LOWER(LTRIM(RTRIM(t.TransactionType)))
                                 = 'expense'
                                THEN 'Added Expense'
                            ELSE 'Added Transaction'
                        END AS Activity,

                        ISNULL(t.TransactionDate, t.CreatedAt)
                            AS ActivityDate

                    FROM dbo.Transactions t
                    LEFT JOIN dbo.Users u
                        ON u.UserId = t.UserId

                    UNION ALL

                    SELECT
                        ISNULL(
                            NULLIF(u.FullName, ''),
                            'Unknown User'
                        ) AS UserName,

                        'Created Budget' AS Activity,
                        b.CreatedAt AS ActivityDate

                    FROM dbo.Budgets b
                    LEFT JOIN dbo.Users u
                        ON u.UserId = b.UserId
                ) AS RecentItems

                ORDER BY RecentItems.ActivityDate DESC;";

            DataTable table = new DataTable();

            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(table);
            }

            rptRecentActivity.DataSource = table;
            rptRecentActivity.DataBind();

            lblNoRecentActivity.Visible = table.Rows.Count == 0;
            rptRecentActivity.Visible = table.Rows.Count > 0;
        }
    }
}
