
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class Dashboard : Page
    {
        private readonly CultureInfo indianCulture =
            new CultureInfo("en-IN");

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure only logged-in users can view the dashboard.
            if (Session["UserId"] == null ||
                Session["Role"] == null ||
                !string.Equals(
                    Convert.ToString(Session["Role"]),
                    "User",
                    StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/User/Login.aspx", false);
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
            int userId;

            if (!int.TryParse(
                Convert.ToString(Session["UserId"]),
                out userId))
            {
                ShowMessage("Your login session is invalid. Please log in again.");
                return;
            }

            try
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"];

                if (settings == null ||
                    string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    ShowMessage(
                        "The ExpenseTrackerDB connection string was not found in Web.config.");
                    return;
                }

                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    con.Open();

                    // Calculate income and expense totals for this user.
                    string totalsQuery = @"
                        SELECT
                            ISNULL(SUM(
                                CASE
                                    WHEN TransactionType = 'Income'
                                    THEN Amount
                                    ELSE 0
                                END
                            ), 0) AS TotalIncome,

                            ISNULL(SUM(
                                CASE
                                    WHEN TransactionType = 'Expense'
                                    THEN Amount
                                    ELSE 0
                                END
                            ), 0) AS TotalExpenses

                        FROM dbo.Transactions
                        WHERE UserId = @UserId;";

                    decimal totalIncome = 0m;
                    decimal totalExpenses = 0m;

                    using (SqlCommand cmd =
                        new SqlCommand(totalsQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@UserId", SqlDbType.Int).Value = userId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalIncome =
                                    Convert.ToDecimal(reader["TotalIncome"]);

                                totalExpenses =
                                    Convert.ToDecimal(reader["TotalExpenses"]);
                            }
                        }
                    }

                    decimal balance = totalIncome - totalExpenses;

                    lblIncome.Text = FormatCurrency(totalIncome);
                    lblExpenses.Text = FormatCurrency(totalExpenses);
                    lblBalance.Text = FormatCurrency(balance);

                    // Get the latest five transactions for this user.
                    string recentQuery = @"
                        SELECT TOP 5
                            TransactionId,
                            TransactionType,
                            Title,
                            Amount,
                            TransactionDate
                        FROM dbo.Transactions
                        WHERE UserId = @UserId
                        ORDER BY TransactionDate DESC, TransactionId DESC;";

                    DataTable recentTransactions = new DataTable();

                    using (SqlCommand cmd =
                        new SqlCommand(recentQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@UserId", SqlDbType.Int).Value = userId;

                        using (SqlDataAdapter adapter =
                            new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(recentTransactions);
                        }
                    }

                    rptRecentTransactions.DataSource = recentTransactions;
                    rptRecentTransactions.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Record the technical details for debugging.
                System.Diagnostics.Trace.TraceError(
                    "Dashboard loading failed: " + ex);

                // Show a useful message without exposing database details.
                ShowMessage(
                    "Unable to load dashboard data. Check your database connection and Transactions table, then rebuild the project.");
            }
        }

        private string FormatCurrency(decimal amount)
        {
            return "₹" + amount.ToString("N2", indianCulture);
        }

        private void ShowMessage(string message)
        {
            lblDashboardMessage.Text = message;
            lblDashboardMessage.Visible = true;
        }
    }
}
