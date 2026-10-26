
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ExpenseTrackerFrontend.User
{
    public partial class AddIncome : System.Web.UI.Page
    {
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
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        protected void btnAddIncome_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            int userId;
            if (!int.TryParse(Convert.ToString(Session["UserId"]), out userId))
            {
                ShowMessage("Your login session is invalid. Please log in again.", false);
                return;
            }

            decimal amount;
            if (!decimal.TryParse(
                    txtAmount.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out amount) || amount <= 0)
            {
                ShowMessage("Please enter an amount greater than zero.", false);
                return;
            }

            DateTime incomeDate;
            if (!DateTime.TryParseExact(
                    txtDate.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out incomeDate))
            {
                ShowMessage("Please select a valid income date.", false);
                return;
            }

            string source = ddlIncomeSource.SelectedValue;
            string description = txtDescription.Text.Trim();

            string notes = "Income Source: " + source;

            if (!string.IsNullOrWhiteSpace(description))
            {
                notes += Environment.NewLine + "Description: " + description;
            }

            string connectionString =
                ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO dbo.Transactions
                            (UserId, TransactionType, Title, Amount,
                             TransactionDate, Notes, CreatedAt)
                        VALUES
                            (@UserId, @TransactionType, @Title, @Amount,
                             @TransactionDate, @Notes, @CreatedAt);";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50).Value = "Income";
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = source;

                        SqlParameter amountParameter =
                            cmd.Parameters.Add("@Amount", SqlDbType.Decimal);
                        amountParameter.Precision = 18;
                        amountParameter.Scale = 2;
                        amountParameter.Value = amount;

                        cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = incomeDate;
                        cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, -1).Value = notes;
                        cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                txtAmount.Text = "";
                txtDescription.Text = "";
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                ddlIncomeSource.SelectedIndex = 0;

                ShowMessage("Income added successfully!", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Add Income failed: " + ex);

                ShowMessage(
                    "Unable to save income. Please check your database connection and table column types.",
                    false);
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = success
                ? "income-message income-success"
                : "income-message income-error";
            lblMessage.Visible = true;
        }
    }
}
