
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExpenseTrackerFrontend.User
{
    public partial class SplitExpenses : System.Web.UI.Page
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
                BindSplits();
            }
        }

        protected void btnCreateSplit_Click(object sender, EventArgs e)
        {
            lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
            lblMessage.Text = "";

            string expenseName = txtExpenseName.Text.Trim();

            if (string.IsNullOrWhiteSpace(expenseName))
            {
                lblMessage.Text = "Please enter an expense name.";
                return;
            }

            decimal totalAmount;

            bool validAmount = decimal.TryParse(
                txtTotalAmount.Text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out totalAmount);

            if (!validAmount)
                validAmount = decimal.TryParse(txtTotalAmount.Text, out totalAmount);

            if (!validAmount || totalAmount <= 0 || totalAmount > 9999999999999999m)
            {
                lblMessage.Text = "Please enter a valid positive amount.";
                return;
            }

            // Restrict the amount to two decimal places.
            if (decimal.Round(totalAmount, 2) != totalAmount)
            {
                lblMessage.Text = "Please enter an amount with at most two decimal places.";
                return;
            }

            List<string> names = txtPeople.Text
                .Split(',')
                .Select(n => n.Trim())
                .Where(n => n.Length > 0)
                .ToList();

            if (names.Count < 2)
            {
                lblMessage.Text = "Enter at least two participant names, separated by commas.";
                return;
            }

            if (names.Count > 100)
            {
                lblMessage.Text = "You can add a maximum of 100 participants per split.";
                return;
            }

            if (names.Any(n => n.Length > 100))
            {
                lblMessage.Text = "Each participant name must be 100 characters or fewer.";
                return;
            }

            if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
            {
                lblMessage.Text = "Participant names must be unique within a split.";
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            int splitId;

                            string insertSplit = @"
                                INSERT INTO dbo.ExpenseSplits
                                    (UserId, ExpenseName, TotalAmount,
                                     NumberOfPeople, CreatedAt)
                                VALUES
                                    (@UserId, @ExpenseName, @TotalAmount,
                                     @NumberOfPeople, @CreatedAt);

                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd = new SqlCommand(insertSplit, con, transaction))
                            {
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                cmd.Parameters.Add("@ExpenseName", SqlDbType.NVarChar, 150).Value = expenseName;

                                SqlParameter amountParameter =
                                    cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal);
                                amountParameter.Precision = 18;
                                amountParameter.Scale = 2;
                                amountParameter.Value = totalAmount;

                                cmd.Parameters.Add("@NumberOfPeople", SqlDbType.Int).Value = names.Count;
                                cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                                splitId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Work in integer paise so shares add up exactly.
                            long totalPaise = Convert.ToInt64(totalAmount * 100m);
                            long baseSharePaise = totalPaise / names.Count;
                            long remainderPaise = totalPaise % names.Count;

                            string insertParticipant = @"
                                INSERT INTO dbo.SplitParticipants
                                    (SplitId, PersonName, ShareAmount, PaymentStatus)
                                VALUES
                                    (@SplitId, @PersonName, @ShareAmount, 'Pending')";

                            for (int i = 0; i < names.Count; i++)
                            {
                                long sharePaise = baseSharePaise + (i < remainderPaise ? 1 : 0);
                                decimal share = sharePaise / 100m;

                                using (SqlCommand cmd = new SqlCommand(
                                    insertParticipant, con, transaction))
                                {
                                    cmd.Parameters.Add("@SplitId", SqlDbType.Int).Value = splitId;
                                    cmd.Parameters.Add("@PersonName", SqlDbType.NVarChar, 100).Value = names[i];

                                    SqlParameter shareParameter =
                                        cmd.Parameters.Add("@ShareAmount", SqlDbType.Decimal);
                                    shareParameter.Precision = 18;
                                    shareParameter.Scale = 2;
                                    shareParameter.Value = share;

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                lblMessage.ForeColor = System.Drawing.Color.LightGreen;
                lblMessage.Text = "Split created successfully!";
                txtExpenseName.Text = "";
                txtTotalAmount.Text = "";
                txtPeople.Text = "";

                BindSplits();
            }
            catch (Exception)
            {
                lblMessage.Text =
                    "Unable to create the split. Check that both split tables were created successfully.";
            }
        }

        private void BindSplits()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DataTable splits = new DataTable();

            string query = @"
                SELECT SplitId, ExpenseName, TotalAmount, NumberOfPeople
                FROM dbo.ExpenseSplits
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC, SplitId DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                adapter.Fill(splits);
            }

            splits.Columns.Add("Participants", typeof(DataTable));

            foreach (DataRow split in splits.Rows)
            {
                DataTable participants = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ParticipantId, PersonName, ShareAmount, PaymentStatus
                    FROM dbo.SplitParticipants
                    WHERE SplitId = @SplitId
                    ORDER BY ParticipantId", con))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@SplitId", SqlDbType.Int).Value =
                        Convert.ToInt32(split["SplitId"]);

                    adapter.Fill(participants);
                }

                split["Participants"] = participants;
            }

            rptSplits.DataSource = splits;
            rptSplits.DataBind();

            pnlNoSplits.Visible = splits.Rows.Count == 0;
        }

        protected void rptParticipants_ItemCommand(
            object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "UpdateStatus")
                return;

            int participantId;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out participantId))
                return;

            DropDownList ddl = e.Item.FindControl("ddlPaymentStatus") as DropDownList;

            if (ddl == null ||
                (ddl.SelectedValue != "Paid" && ddl.SelectedValue != "Pending"))
                return;

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE p
                    SET p.PaymentStatus = @PaymentStatus
                    FROM dbo.SplitParticipants p
                    INNER JOIN dbo.ExpenseSplits s ON s.SplitId = p.SplitId
                    WHERE p.ParticipantId = @ParticipantId
                      AND s.UserId = @UserId", con))
                {
                    cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 20).Value =
                        ddl.SelectedValue;
                    cmd.Parameters.Add("@ParticipantId", SqlDbType.Int).Value = participantId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    con.Open();
                    int updated = cmd.ExecuteNonQuery();

                    lblMessage.ForeColor = updated > 0
                        ? System.Drawing.Color.LightGreen
                        : System.Drawing.Color.OrangeRed;

                    lblMessage.Text = updated > 0
                        ? "Payment status updated."
                        : "Participant record was not found.";
                }

                BindSplits();
            }
            catch (Exception)
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text = "Unable to update payment status. Please try again.";
            }
        }

        protected void rptSplits_ItemCommand(
            object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteSplit")
                return;

            int splitId;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out splitId))
                return;

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
                                DELETE FROM dbo.SplitParticipants
                                WHERE SplitId IN
                                (
                                    SELECT SplitId
                                    FROM dbo.ExpenseSplits
                                    WHERE SplitId = @SplitId AND UserId = @UserId
                                )", con, transaction))
                            {
                                cmd.Parameters.Add("@SplitId", SqlDbType.Int).Value = splitId;
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                cmd.ExecuteNonQuery();
                            }

                            int deleted;

                            using (SqlCommand cmd = new SqlCommand(@"
                                DELETE FROM dbo.ExpenseSplits
                                WHERE SplitId = @SplitId AND UserId = @UserId", con, transaction))
                            {
                                cmd.Parameters.Add("@SplitId", SqlDbType.Int).Value = splitId;
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                deleted = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            lblMessage.ForeColor = deleted > 0
                                ? System.Drawing.Color.LightGreen
                                : System.Drawing.Color.OrangeRed;

                            lblMessage.Text = deleted > 0
                                ? "Split deleted successfully."
                                : "Split was not found.";
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                BindSplits();
            }
            catch (Exception)
            {
                lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                lblMessage.Text = "Unable to delete this split. Please try again.";
            }
        }
    }
}
