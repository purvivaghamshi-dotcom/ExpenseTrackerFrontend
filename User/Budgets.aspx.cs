
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExpenseTrackerFrontend.User
{
    public partial class Budgets : System.Web.UI.Page
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
                lblCurrentMonth.Text =
                    DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

                BindBudgets();
            }
        }

        protected void btnCreateBudget_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.ForeColor = System.Drawing.Color.OrangeRed;

            decimal amount;
            if (!decimal.TryParse(
                    txtBudgetAmount.Text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out amount) &&
                !decimal.TryParse(txtBudgetAmount.Text, out amount))
            {
                lblMessage.Text = "Please enter a valid budget amount.";
                return;
            }

            if (amount <= 0)
            {
                lblMessage.Text = "Budget amount must be greater than zero.";
                return;
            }

            string category = ddlCategory.SelectedValue;
            int userId = Convert.ToInt32(Session["UserId"]);
            DateTime today = DateTime.Today;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Avoid creating the same category budget twice
                    // for the same user in the same month.
                    string checkQuery = @"
                        SELECT COUNT(*)
                        FROM dbo.Budgets
                        WHERE UserId = @UserId
                          AND BudgetName = @BudgetName
                          AND MonthNumber = @MonthNumber
                          AND YearNumber = @YearNumber";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        checkCmd.Parameters.Add("@BudgetName", SqlDbType.NVarChar, 100).Value = category;
                        checkCmd.Parameters.Add("@MonthNumber", SqlDbType.Int).Value = today.Month;
                        checkCmd.Parameters.Add("@YearNumber", SqlDbType.Int).Value = today.Year;

                        int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            lblMessage.Text =
                                "A budget for " + category +
                                " already exists for this month. Delete it first if you want to replace it.";
                            BindBudgets();
                            return;
                        }
                    }

                    string insertQuery = @"
                        INSERT INTO dbo.Budgets
                            (UserId, BudgetName, BudgetAmount,
                             MonthNumber, YearNumber, CreatedAt)
                        VALUES
                            (@UserId, @BudgetName, @BudgetAmount,
                             @MonthNumber, @YearNumber, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@BudgetName", SqlDbType.NVarChar, 100).Value = category;

                        SqlParameter amountParameter =
                            cmd.Parameters.Add("@BudgetAmount", SqlDbType.Decimal);
                        amountParameter.Precision = 18;
                        amountParameter.Scale = 2;
                        amountParameter.Value = amount;

                        cmd.Parameters.Add("@MonthNumber", SqlDbType.Int).Value = today.Month;
                        cmd.Parameters.Add("@YearNumber", SqlDbType.Int).Value = today.Year;
                        cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.ForeColor = System.Drawing.Color.LightGreen;
                lblMessage.Text = "Budget created successfully!";
                txtBudgetAmount.Text = "";

                BindBudgets();
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to create the budget. Please check your database connection and table columns.";
            }
        }

        private void BindBudgets()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DateTime today = DateTime.Today;

            DataTable table = new DataTable();

            string query = @"
                SELECT
                    b.BudgetId,
                    b.BudgetName,
                    b.BudgetAmount,
                    ISNULL(
                        (
                            SELECT SUM(t.Amount)
                            FROM dbo.Transactions t
                            WHERE t.UserId = b.UserId
                              AND t.TransactionType = 'Expense'
                              AND t.Title = b.BudgetName
                              AND MONTH(t.TransactionDate) = b.MonthNumber
                              AND YEAR(t.TransactionDate) = b.YearNumber
                        ), 0
                    ) AS AmountSpent
                FROM dbo.Budgets b
                WHERE b.UserId = @UserId
                  AND b.MonthNumber = @MonthNumber
                  AND b.YearNumber = @YearNumber
                ORDER BY b.BudgetName";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@MonthNumber", SqlDbType.Int).Value = today.Month;
                cmd.Parameters.Add("@YearNumber", SqlDbType.Int).Value = today.Year;

                adapter.Fill(table);
            }

            if (!table.Columns.Contains("RemainingAmount"))
                table.Columns.Add("RemainingAmount", typeof(decimal));

            if (!table.Columns.Contains("ProgressWidth"))
                table.Columns.Add("ProgressWidth", typeof(int));

            if (!table.Columns.Contains("BudgetStatus"))
                table.Columns.Add("BudgetStatus", typeof(string));

            foreach (DataRow row in table.Rows)
            {
                decimal limit = Convert.ToDecimal(row["BudgetAmount"]);
                decimal spent = Convert.ToDecimal(row["AmountSpent"]);
                decimal remaining = limit - spent;

                int progress = limit > 0
                    ? (int)Math.Min(100, Math.Round((spent / limit) * 100))
                    : 0;

                row["RemainingAmount"] = remaining;
                row["ProgressWidth"] = progress;

                if (spent > limit)
                {
                    row["BudgetStatus"] =
                        "Over budget by " + (spent - limit).ToString("C");
                }
                else if (spent == limit)
                {
                    row["BudgetStatus"] = "Budget limit reached!";
                }
                else
                {
                    row["BudgetStatus"] = progress + "% used";
                }
            }

            rptBudgets.DataSource = table;
            rptBudgets.DataBind();

            pnlNoBudgets.Visible = table.Rows.Count == 0;
        }

        protected void rptBudgets_ItemCommand(
            object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteBudget")
                return;

            int budgetId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out budgetId))
                return;

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    DELETE FROM dbo.Budgets
                    WHERE BudgetId = @BudgetId
                      AND UserId = @UserId
                      AND MonthNumber = @MonthNumber
                      AND YearNumber = @YearNumber", con))
                {
                    cmd.Parameters.Add("@BudgetId", SqlDbType.Int).Value = budgetId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@MonthNumber", SqlDbType.Int).Value = DateTime.Today.Month;
                    cmd.Parameters.Add("@YearNumber", SqlDbType.Int).Value = DateTime.Today.Year;

                    con.Open();
                    int deleted = cmd.ExecuteNonQuery();

                    lblMessage.ForeColor = deleted > 0
                        ? System.Drawing.Color.LightGreen
                        : System.Drawing.Color.OrangeRed;

                    lblMessage.Text = deleted > 0
                        ? "Budget deleted successfully."
                        : "The budget could not be found.";
                }

                BindBudgets();
            }
            catch (Exception)
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text = "Unable to delete this budget. Please try again.";
            }
        }
    }
}
