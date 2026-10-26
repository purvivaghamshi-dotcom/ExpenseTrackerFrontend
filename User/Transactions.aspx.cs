
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ExpenseTrackerFrontend.User
{
    public partial class Transactions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null ||
                Session["Role"] == null ||
                Convert.ToString(Session["Role"]) != "User")
            {
                Response.Redirect("~/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTransactions();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            int userId;

            if (!int.TryParse(Convert.ToString(Session["UserId"]), out userId))
            {
                Response.Redirect("~/User/Login.aspx");
                return;
            }

            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"].ConnectionString;

                DataTable table = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT
                            TransactionId,
                            TransactionDate,
                            Title,
                            Title AS Category,
                            TransactionType,
                            Amount
                        FROM dbo.Transactions
                        WHERE UserId = @UserId
                          AND (
                                @Search = ''
                                OR Title LIKE @SearchPattern
                                OR ISNULL(Notes, '') LIKE @SearchPattern
                              )
                          AND (
                                @Type = 'All'
                                OR TransactionType = @Type
                              )
                        ORDER BY TransactionDate DESC, TransactionId DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        string search = txtSearch.Text.Trim();
                        string type = ddlType.SelectedValue;

                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = search;
                        cmd.Parameters.Add("@SearchPattern", SqlDbType.NVarChar, 202).Value =
                            "%" + search + "%";
                        cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = type;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(table);
                        }
                    }
                }

                rptTransactions.DataSource = table;
                rptTransactions.DataBind();

                phEmpty.Visible = table.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Loading transactions failed: " + ex);

                ShowMessage(
                    "Unable to load transactions. Please check your database connection.",
                    false);
            }
        }

        protected void rptTransactions_ItemCommand(
            object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteTransaction")
            {
                return;
            }

            int userId;
            int transactionId;

            if (!int.TryParse(Convert.ToString(Session["UserId"]), out userId) ||
                !int.TryParse(Convert.ToString(e.CommandArgument), out transactionId))
            {
                ShowMessage("Invalid transaction or login session.", false);
                return;
            }

            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["ExpenseTrackerDB"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        DELETE FROM dbo.Transactions
                        WHERE TransactionId = @TransactionId
                          AND UserId = @UserId;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@TransactionId", SqlDbType.Int).Value =
                            transactionId;

                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        con.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Transaction deleted successfully.", true);
                        }
                        else
                        {
                            ShowMessage(
                                "Transaction was not found or you don't have permission to delete it.",
                                false);
                        }
                    }
                }

                LoadTransactions();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Deleting transaction failed: " + ex);

                ShowMessage(
                    "Unable to delete the transaction. Please try again.",
                    false);
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = success
                ? "transactions-message transactions-success"
                : "transactions-message transactions-error";

            lblMessage.Visible = true;
        }
    }
}
