
<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Reports.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .report-filter {
            margin-bottom: 24px;
        }

        .report-filter .form-control {
            width: 100%;
            box-sizing: border-box;
        }

        .report-filter .form-group {
            margin-bottom: 14px;
        }

        .report-message {
            display: block;
            margin-top: 12px;
            font-weight: 600;
        }

        .report-section {
            margin-top: 24px;
        }

        .report-category {
            margin: 18px 0;
        }

        .report-category-heading {
            display: flex;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 8px;
        }

        .report-progress-track {
            height: 10px;
            overflow: hidden;
            border-radius: 20px;
            background: rgba(255,255,255,0.12);
        }

        .report-progress-fill {
            height: 100%;
            border-radius: 20px;
            background: linear-gradient(90deg, #a855f7, #22d3ee);
        }

        .report-table-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .report-table-wrap .data-table {
            width: 100%;
        }

        .report-empty {
            padding: 18px;
            text-align: center;
        }

        .report-period {
            margin-top: 6px;
            opacity: 0.85;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Reports &amp; Analytics</h2>
        <p>Understand your income, savings, and spending patterns.</p>
        <p class="report-period">
            Report period:
            <asp:Label ID="lblReportPeriod" runat="server" />
        </p>
    </div>

    <div class="card report-filter">

        <h3>Filter Report by Date</h3>

        <div class="two-column">

            <div class="form-group">
                <label for="<%= txtStartDate.ClientID %>">From Date</label>
                <asp:TextBox
                    ID="txtStartDate"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Date" />
            </div>

            <div class="form-group">
                <label for="<%= txtEndDate.ClientID %>">To Date</label>
                <asp:TextBox
                    ID="txtEndDate"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Date" />
            </div>

        </div>

        <asp:Button
            ID="btnGenerateReport"
            runat="server"
            Text="GENERATE REPORT"
            CssClass="btn btn-cyan"
            OnClick="btnGenerateReport_Click" />

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="report-message"
            EnableViewState="false" />

    </div>

    <div class="stats-grid">

        <div class="stat-card neon-green">
            <div class="stat-title">TOTAL INCOME</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalIncome" runat="server" Text="₹0.00" />
            </div>
        </div>

        <div class="stat-card neon-orange">
            <div class="stat-title">TOTAL EXPENSE</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalExpense" runat="server" Text="₹0.00" />
            </div>
        </div>

        <div class="stat-card neon-cyan">
            <div class="stat-title">SAVINGS / NET BALANCE</div>
            <div class="stat-value">
                <asp:Label ID="lblSavings" runat="server" Text="₹0.00" />
            </div>
        </div>

        <div class="stat-card neon-purple">
            <div class="stat-title">SAVING RATE</div>
            <div class="stat-value">
                <asp:Label ID="lblSavingRate" runat="server" Text="0.00%" />
            </div>
        </div>

    </div>

    <div class="card report-section">

        <h3>Expense Breakdown by Category</h3>
        <p>Categories are based on the expense titles saved in your transactions.</p>

        <asp:Panel ID="pnlNoCategories" runat="server"
            CssClass="report-empty" Visible="false">
            <p>No expenses found for the selected period.</p>
        </asp:Panel>

        <asp:Repeater ID="rptCategories" runat="server">
            <ItemTemplate>

                <div class="report-category">

                    <div class="report-category-heading">
                        <strong><%#: Eval("Category") %></strong>
                        <span><%#: Eval("AmountDisplay") %> (<%#: Eval("PercentageDisplay") %>)</span>
                    </div>

                    <div class="report-progress-track">
                        <div
                            class="report-progress-fill"
                            style='width: <%#: Eval("ProgressWidth") %>%;'>
                        </div>
                    </div>

                </div>

            </ItemTemplate>
        </asp:Repeater>

    </div>

    <div class="card report-section">

        <h3>Transaction Summary</h3>

        <asp:Panel ID="pnlNoTransactions" runat="server"
            CssClass="report-empty" Visible="false">
            <p>No transactions found for the selected dates.</p>
        </asp:Panel>

        <div class="report-table-wrap">

            <asp:Repeater ID="rptTransactions" runat="server">
                <HeaderTemplate>
                    <table class="data-table">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Title / Category</th>
                                <th>Type</th>
                                <th>Notes</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr>
                        <td><%#: Eval("DateDisplay") %></td>
                        <td><%#: Eval("Title") %></td>
                        <td><%#: Eval("TransactionType") %></td>
                        <td><%#: Eval("Notes") %></td>
                        <td><%#: Eval("AmountDisplay") %></td>
                    </tr>
                </ItemTemplate>

                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

        </div>

    </div>

</asp:Content>
