
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace ExpenseTrackerFrontend.User
{
    public partial class AddExpense : System.Web.UI.Page
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

        protected void btnAddExpense_Click(object sender, EventArgs e)
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
                ShowMessage("Please enter a valid amount greater than zero.", false);
                return;
            }

            DateTime expenseDate;
            if (!DateTime.TryParseExact(
                    txtDate.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out expenseDate))
            {
                ShowMessage("Please select a valid expense date.", false);
                return;
            }

            string category = ddlCategory.SelectedValue;
            string paymentMethod = ddlPaymentMethod.SelectedValue;
            string description = txtDescription.Text.Trim();

            string notes = "Payment Method: " + paymentMethod;

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
                        cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50).Value = "Expense";
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = category;

                        SqlParameter amountParameter =
                            cmd.Parameters.Add("@Amount", SqlDbType.Decimal);
                        amountParameter.Precision = 18;
                        amountParameter.Scale = 2;
                        amountParameter.Value = amount;

                        cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = expenseDate;
                        cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, -1).Value = notes;
                        cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                txtAmount.Text = "";
                txtDescription.Text = "";
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                ddlCategory.SelectedIndex = 0;
                ddlPaymentMethod.SelectedIndex = 0;

                ShowMessage("Expense added successfully!", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Add Expense failed: " + ex);

                ShowMessage(
                    "Unable to save the expense. Please check your database connection and table column types.",
                    false);
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = success
                ? "expense-message expense-success"
                : "expense-message expense-error";
            lblMessage.Visible = true;
        }
    }
}
