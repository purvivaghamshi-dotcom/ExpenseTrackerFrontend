
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class Reports : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null ||
                Session["Role"] == null ||
                Session["Role"].ToString() != "User")
            {
                Response.Redirect("~/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                DateTime today = DateTime.Today;

                txtStartDate.Text =
                    new DateTime(today.Year, today.Month, 1).ToString("yyyy-MM-dd");

                txtEndDate.Text = today.ToString("yyyy-MM-dd");

                LoadReport();
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(
                    txtStartDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out startDate) ||
                !DateTime.TryParseExact(
                    txtEndDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out endDate))
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text = "Please select valid start and end dates.";
                return;
            }

            if (startDate.Date > endDate.Date)
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text = "The From Date cannot be later than the To Date.";
                return;
            }

            LoadReport();
        }

        private void LoadReport()
        {
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(
                    txtStartDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out startDate) ||
                !DateTime.TryParseExact(
                    txtEndDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out endDate) ||
                startDate.Date > endDate.Date)
            {
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            // The exclusive upper boundary includes all transactions
            // recorded on the selected end date, including their times.
            DateTime endExclusive = endDate.Date.AddDays(1);

            lblReportPeriod.Text =
                startDate.ToString("dd MMM yyyy") + " - " +
                endDate.ToString("dd MMM yyyy");

            try
            {
                LoadTotals(userId, startDate.Date, endExclusive);
                LoadCategoryBreakdown(userId, startDate.Date, endExclusive);
                LoadTransactions(userId, startDate.Date, endExclusive);

                lblMessage.Text = "";
            }
            catch (Exception)
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text =
                    "Unable to load the report. Please check your database connection and try again.";
            }
        }

        private void LoadTotals(int userId, DateTime startDate, DateTime endExclusive)
        {
            decimal totalIncome = 0;
            decimal totalExpense = 0;

            string query = @"
                SELECT
                    ISNULL(SUM(CASE
                        WHEN TransactionType = 'Income' THEN Amount
                        ELSE 0
                    END), 0) AS TotalIncome,

                    ISNULL(SUM(CASE
                        WHEN TransactionType = 'Expense' THEN Amount
                        ELSE 0
                    END), 0) AS TotalExpense

                FROM dbo.Transactions
                WHERE UserId = @UserId
                  AND TransactionDate >= @StartDate
                  AND TransactionDate < @EndExclusive
                  AND TransactionType IN ('Income', 'Expense')";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@EndExclusive", SqlDbType.DateTime).Value = endExclusive;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totalIncome = Convert.ToDecimal(reader["TotalIncome"]);
                        totalExpense = Convert.ToDecimal(reader["TotalExpense"]);
                    }
                }
            }

            decimal savings = totalIncome - totalExpense;

            decimal savingRate = totalIncome > 0
                ? (savings / totalIncome) * 100m
                : 0m;

            lblTotalIncome.Text = FormatRupees(totalIncome);
            lblTotalExpense.Text = FormatRupees(totalExpense);
            lblSavings.Text = FormatRupees(savings);
            lblSavingRate.Text = savingRate.ToString("N2") + "%";
        }

        private void LoadCategoryBreakdown(
            int userId, DateTime startDate, DateTime endExclusive)
        {
            DataTable categories = new DataTable();

            string query = @"
                SELECT
                    ISNULL(NULLIF(LTRIM(RTRIM(Title)), ''), 'Uncategorized')
                        AS Category,
                    SUM(Amount) AS CategoryAmount

                FROM dbo.Transactions

                WHERE UserId = @UserId
                  AND TransactionType = 'Expense'
                  AND TransactionDate >= @StartDate
                  AND TransactionDate < @EndExclusive

                GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(Title)), ''), 'Uncategorized')
                ORDER BY SUM(Amount) DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@EndExclusive", SqlDbType.DateTime).Value = endExclusive;

                adapter.Fill(categories);
            }

            categories.Columns.Add("AmountDisplay", typeof(string));
            categories.Columns.Add("PercentageDisplay", typeof(string));
            categories.Columns.Add("ProgressWidth", typeof(int));

            decimal totalExpenses = 0;

            foreach (DataRow row in categories.Rows)
            {
                totalExpenses += Convert.ToDecimal(row["CategoryAmount"]);
            }

            foreach (DataRow row in categories.Rows)
            {
                decimal amount = Convert.ToDecimal(row["CategoryAmount"]);

                decimal percentage = totalExpenses > 0
                    ? (amount / totalExpenses) * 100m
                    : 0m;

                row["AmountDisplay"] = FormatRupees(amount);
                row["PercentageDisplay"] = percentage.ToString("N1") + "%";
                row["ProgressWidth"] = (int)Math.Min(
                    100, Math.Round(percentage, MidpointRounding.AwayFromZero));
            }

            rptCategories.DataSource = categories;
            rptCategories.DataBind();

            pnlNoCategories.Visible = categories.Rows.Count == 0;
        }

        private void LoadTransactions(
            int userId, DateTime startDate, DateTime endExclusive)
        {
            DataTable transactions = new DataTable();

            string query = @"
                SELECT
                    TransactionDate,
                    Title,
                    TransactionType,
                    Notes,
                    Amount

                FROM dbo.Transactions

                WHERE UserId = @UserId
                  AND TransactionDate >= @StartDate
                  AND TransactionDate < @EndExclusive

                ORDER BY TransactionDate DESC, TransactionId DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@EndExclusive", SqlDbType.DateTime).Value = endExclusive;

                adapter.Fill(transactions);
            }

            transactions.Columns.Add("DateDisplay", typeof(string));
            transactions.Columns.Add("AmountDisplay", typeof(string));

            foreach (DataRow row in transactions.Rows)
            {
                DateTime transactionDate =
                    Convert.ToDateTime(row["TransactionDate"]);

                decimal amount = Convert.ToDecimal(row["Amount"]);

                row["DateDisplay"] = transactionDate.ToString("dd MMM yyyy");
                row["AmountDisplay"] = FormatRupees(amount);

                if (row["Notes"] == DBNull.Value)
                {
                    row["Notes"] = "";
                }
            }

            rptTransactions.DataSource = transactions;
            rptTransactions.DataBind();

            pnlNoTransactions.Visible = transactions.Rows.Count == 0;
        }

        private string FormatRupees(decimal amount)
        {
            return "₹" + amount.ToString("N2", CultureInfo.GetCultureInfo("en-IN"));
        }
    }
}
