
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExpenseTrackerFrontend.Admin
{
    public partial class Transactions : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only authenticated administrators may view all transactions.
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
                BindTransactions();
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

        private void BindTransactions()
        {
            string searchText = txtSearchTransaction.Text.Trim();
            string selectedType = ddlTransactionType.SelectedValue;

            string query = @"
                SELECT
                    t.TransactionId,

                    ISNULL(
                        NULLIF(u.FullName, N''),
                        N'Unknown User'
                    ) AS UserName,

                    ISNULL(t.Title, N'') AS Description,
                    ISNULL(t.Notes, N'') AS Notes,
                    ISNULL(t.TransactionType, N'') AS TransactionType,
                    t.Amount,

                    COALESCE(
                        t.TransactionDate,
                        t.CreatedAt
                    ) AS TransactionDate

                FROM dbo.Transactions AS t

                LEFT JOIN dbo.Users AS u
                    ON u.UserId = t.UserId

                WHERE
                    (
                        @SearchText = N''
                        OR CONVERT(NVARCHAR(20), t.TransactionId)
                            LIKE @SearchPattern
                        OR ISNULL(u.FullName, N'')
                            LIKE @SearchPattern
                        OR ISNULL(u.Email, N'')
                            LIKE @SearchPattern
                        OR ISNULL(t.Title, N'')
                            LIKE @SearchPattern
                        OR ISNULL(t.Notes, N'')
                            LIKE @SearchPattern
                        OR ISNULL(t.TransactionType, N'')
                            LIKE @SearchPattern
                        OR CONVERT(NVARCHAR(50), t.Amount)
                            LIKE @SearchPattern
                    )

                    AND
                    (
                        @Type = N'All Transactions'
                        OR LOWER(
                            LTRIM(RTRIM(
                                ISNULL(t.TransactionType, N'')
                            ))
                        ) = LOWER(@Type)
                    )

                ORDER BY
                    COALESCE(t.TransactionDate, t.CreatedAt) DESC,
                    t.TransactionId DESC;";

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

                    cmd.Parameters.Add(
                        "@Type",
                        SqlDbType.NVarChar,
                        30).Value = selectedType;

                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }

                gvTransactions.DataSource = table;
                gvTransactions.DataBind();

                lblTransactionMessage.Text =
                    "Transactions found: " + table.Rows.Count;
            }
            catch (Exception)
            {
                lblTransactionMessage.Text =
                    "Unable to load transactions. Check the database " +
                    "connection and table structure.";

                gvTransactions.DataSource = null;
                gvTransactions.DataBind();
            }
        }

        protected void btnSearchTransactions_Click(
            object sender, EventArgs e)
        {
            BindTransactions();
        }

        protected void btnClearTransactions_Click(
            object sender, EventArgs e)
        {
            txtSearchTransaction.Text = "";
            ddlTransactionType.SelectedValue = "All Transactions";

            BindTransactions();
        }

        public Color GetTransactionColor(object transactionType)
        {
            string type = Convert.ToString(transactionType);

            if (type.Equals(
                "Income", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(57, 255, 136);
            }

            if (type.Equals(
                "Expense", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(255, 122, 0);
            }

            return Color.White;
        }
    }
}
