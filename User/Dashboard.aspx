
<%@ Page Title="Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Dashboard" %>

<asp:Content ID="DashboardContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .dashboard-page {
            color: #f2f4f8;
            padding: 10px 0 30px;
        }

        .dashboard-title {
            margin-bottom: 30px;
        }

        .dashboard-title h1 {
            font-size: 34px;
            margin: 0 0 8px;
            color: #ffffff;
            text-shadow: 0 0 8px #00eaff,
                         0 0 18px rgba(0,234,255,0.5);
        }

        .dashboard-title p {
            color: #858b95;
            font-size: 15px;
            margin: 0;
        }

        .summary-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .summary-card {
            box-sizing: border-box;
            min-width: 0;
            background: linear-gradient(145deg, #191c22, #0d0f12);
            border: 1px solid #303640;
            border-radius: 14px;
            padding: 25px;
            min-height: 145px;
            box-shadow: 0 8px 25px rgba(0,0,0,0.35);
            transition: transform 0.25s, border-color 0.25s;
        }

        .summary-card:hover {
            transform: translateY(-3px);
            border-color: #00eaff;
        }

        .card-label {
            color: #a0a6b1;
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 1.5px;
            margin-bottom: 18px;
        }

        .card-value {
            display: block;
            font-size: 30px;
            font-weight: bold;
            overflow-wrap: anywhere;
        }

        .balance {
            color: #00eaff;
            text-shadow: 0 0 10px rgba(0,234,255,0.45);
        }

        .income {
            color: #39ff88;
            text-shadow: 0 0 10px rgba(57,255,136,0.35);
        }

        .expense {
            color: #ff4d79;
            text-shadow: 0 0 10px rgba(255,59,107,0.35);
        }

        .dashboard-grid {
            display: grid;
            grid-template-columns: minmax(0, 2fr) minmax(230px, 1fr);
            gap: 20px;
            align-items: start;
        }

        .panel {
            box-sizing: border-box;
            min-width: 0;
            background: linear-gradient(145deg, #191c22, #0d0f12);
            border: 1px solid #303640;
            border-radius: 14px;
            padding: 25px;
            box-shadow: 0 8px 25px rgba(0,0,0,0.35);
        }

        .panel h2 {
            font-size: 19px;
            margin: 0 0 20px;
            color: #ffffff;
        }

        .transaction {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 15px;
            padding: 15px 0;
            border-bottom: 1px solid #292d35;
        }

        .transaction:last-child {
            border-bottom: none;
        }

        .transaction-name {
            color: #eeeeee;
            font-size: 14px;
            overflow-wrap: anywhere;
        }

        .transaction-date {
            color: #858b95;
            font-size: 12px;
            margin-top: 5px;
        }

        .transaction-amount {
            font-weight: bold;
            white-space: nowrap;
            text-align: right;
        }

        .transaction-income {
            color: #39ff88;
        }

        .transaction-expense {
            color: #ff4d79;
        }

        .empty-transactions {
            color: #a0a6b1;
            padding: 12px 0;
            font-size: 14px;
            line-height: 1.6;
        }

        .dashboard-message {
            display: block;
            margin-bottom: 20px;
            padding: 14px;
            border: 1px solid #ff4d79;
            border-radius: 8px;
            color: #ff809d;
            background: rgba(255,59,107,0.08);
            overflow-wrap: anywhere;
        }

        .quick-actions {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .quick-btn {
            box-sizing: border-box;
            display: block;
            width: 100%;
            text-decoration: none;
            padding: 14px;
            text-align: center;
            border-radius: 8px;
            background: linear-gradient(145deg, #1c2027, #101216);
            border: 1px solid #3b424e;
            color: #eeeeee;
            transition: 0.25s;
        }

        .quick-btn:hover {
            color: #00eaff;
            border-color: #00eaff;
            box-shadow: 0 0 10px rgba(0,234,255,0.25);
            transform: translateX(3px);
        }

        .dashboard-note {
            color: #858b95;
            font-size: 12px;
            margin-top: 18px;
            line-height: 1.5;
        }

        @media (max-width: 900px) {
            .summary-grid {
                grid-template-columns: 1fr;
            }

            .dashboard-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 480px) {
            .dashboard-title h1 {
                font-size: 28px;
            }

            .summary-card,
            .panel {
                padding: 18px;
            }

            .card-value {
                font-size: 26px;
            }

            .transaction {
                align-items: flex-start;
            }
        }
    </style>

    <div class="dashboard-page">

        <div class="dashboard-title">
            <h1>Dashboard</h1>
            <p>Welcome back! Here's an overview of your finances.</p>
        </div>

        <asp:Label ID="lblDashboardMessage"
            runat="server"
            Visible="false"
            CssClass="dashboard-message"
            EnableViewState="false" />

        <div class="summary-grid">

            <div class="summary-card">
                <div class="card-label">Total Balance</div>
                <asp:Label ID="lblBalance"
                    runat="server"
                    CssClass="card-value balance"
                    Text="₹0.00" />
            </div>

            <div class="summary-card">
                <div class="card-label">Total Income</div>
                <asp:Label ID="lblIncome"
                    runat="server"
                    CssClass="card-value income"
                    Text="₹0.00" />
            </div>

            <div class="summary-card">
                <div class="card-label">Total Expenses</div>
                <asp:Label ID="lblExpenses"
                    runat="server"
                    CssClass="card-value expense"
                    Text="₹0.00" />
            </div>

        </div>

        <div class="dashboard-grid">

            <div class="panel">
                <h2>Recent Transactions</h2>

                <asp:Repeater ID="rptRecentTransactions" runat="server">

                    <ItemTemplate>
                        <div class="transaction">

                            <div>
                                <div class="transaction-name">
                                    <%#: Eval("Title") %>
                                    <span style="color:#858b95;">
                                        (<%#: Eval("TransactionType") %>)
                                    </span>
                                </div>

                                <div class="transaction-date">
                                    <%#: Eval("TransactionDate", "{0:dd MMM yyyy}") %>
                                </div>
                            </div>

                            <div class='transaction-amount <%# Convert.ToString(Eval("TransactionType")) == "Income" ? "transaction-income" : "transaction-expense" %>'>
                                <%# Convert.ToString(Eval("TransactionType")) == "Income" ? "+" : "-" %><%# string.Format(new System.Globalization.CultureInfo("en-IN"), "₹{0:N2}", Eval("Amount")) %>
                            </div>

                        </div>
                    </ItemTemplate>

                    <FooterTemplate>
                        <% if (rptRecentTransactions.Items.Count == 0) { %>
                            <div class="empty-transactions">
                                No transactions yet. Add your first income or expense.
                            </div>
                        <% } %>
                    </FooterTemplate>

                </asp:Repeater>
            </div>

            <div class="panel">
                <h2>Quick Actions</h2>

                <div class="quick-actions">

                    <a class="quick-btn"
                       href="<%= ResolveUrl("~/User/AddExpense.aspx") %>">
                        + Add Expense
                    </a>

                    <a class="quick-btn"
                       href="<%= ResolveUrl("~/User/AddIncome.aspx") %>">
                        + Add Income
                    </a>

                    <a class="quick-btn"
                       href="<%= ResolveUrl("~/User/Transactions.aspx") %>">
                        View Transactions
                    </a>

                    <a class="quick-btn"
                       href="<%= ResolveUrl("~/User/Budgets.aspx") %>">
                        View Budgets
                    </a>

                    <a class="quick-btn"
                       href="<%= ResolveUrl("~/User/Reports.aspx") %>">
                        View Reports
                    </a>

                </div>

                <div class="dashboard-note">
                    Keep track of your income and expenses in one place.
                </div>
            </div>

        </div>

    </div>

</asp:Content>
