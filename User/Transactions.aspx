
<%@ Page Title="Transactions" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Transactions.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Transactions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .transactions-filter {
            display: grid;
            grid-template-columns: 2fr 1fr auto;
            gap: 15px;
            align-items: center;
        }

        .transactions-message {
            display: block;
            margin-bottom: 15px;
            padding: 12px;
            border-radius: 8px;
        }

        .transactions-success {
            color: #39ff88;
            border: 1px solid #39ff88;
            background: rgba(57,255,136,0.08);
        }

        .transactions-error {
            color: #ff6b81;
            border: 1px solid #ff3b6b;
            background: rgba(255,59,107,0.08);
        }

        .transactions-table-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .transactions-table {
            width: 100%;
            border-collapse: collapse;
        }

        .transactions-table th,
        .transactions-table td {
            padding: 14px 12px;
            text-align: left;
            border-bottom: 1px solid #292d35;
            white-space: nowrap;
        }

        .transactions-table th {
            color: #00eaff;
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .transactions-table td {
            color: #e5e7eb;
            font-size: 14px;
        }

        .transaction-income {
            color: #39ff88 !important;
            font-weight: bold;
        }

        .transaction-expense {
            color: #ff7a00 !important;
            font-weight: bold;
        }

        .transaction-type-income {
            color: #39ff88;
        }

        .transaction-type-expense {
            color: #ff7a00;
        }

        .transactions-empty {
            color: #858b95 !important;
            text-align: center !important;
            padding: 25px !important;
        }

        .transaction-delete {
            cursor: pointer;
        }

        /* CENTER THE PAGE HEADING */
        .page-heading {
            width: 100%;
            text-align: center;
            margin-left: auto;
            margin-right: auto;
        }

        /* CENTER BOTH CONTENT CARDS */
        .page-heading ~ .card {
            width: 100%;
            margin: 0 auto 25px;
        }

        /* CENTER THE FILTER CARD */
        .transactions-filter {
            align-items: center;
        }

        /* METALLIC NEON DROPDOWN */
        select.form-control {
            display: block;
            width: 100%;
            min-height: 48px;
            padding: 0 42px 0 14px;

            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 14px;
            font-weight: 500;
            letter-spacing: .3px;

            color: #f2f4f8 !important;
            color-scheme: dark;

            background-color: #111419 !important;
            background-image:
                url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='%2300eaff' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E"),
                linear-gradient(145deg, #303640 0%, #16191f 45%, #0c0e12 100%) !important;

            background-repeat: no-repeat, no-repeat !important;
            background-position: right 14px center, center !important;
            background-size: 18px 18px, 100% 100% !important;

            border: 1px solid #515966 !important;
            border-radius: 9px;
            outline: none;
            cursor: pointer;

            box-shadow:
                inset 0 1px 1px rgba(255,255,255,.09),
                inset 0 -2px 4px rgba(0,0,0,.55),
                0 3px 8px rgba(0,0,0,.3);

            transition: border-color .25s ease,
                        box-shadow .25s ease;
        }

        /* DROPDOWN HOVER */
        select.form-control:hover {
            color: #ffffff !important;
            border-color: #00bcd4 !important;

            box-shadow:
                inset 0 1px 1px rgba(255,255,255,.1),
                0 0 8px rgba(0,234,255,.16),
                0 4px 12px rgba(0,0,0,.4);
        }

        /* DROPDOWN FOCUS */
        select.form-control:focus {
            color: #ffffff !important;
            border-color: #00eaff !important;

            box-shadow:
                0 0 0 2px rgba(0,234,255,.12),
                0 0 15px rgba(0,234,255,.24),
                inset 0 2px 5px rgba(0,0,0,.4);
        }

        /* DARK DROPDOWN OPTIONS */
        select.form-control option {
            background-color: #111419;
            color: #e8edf3;
            padding: 12px;
            font-size: 14px;
        }

        select.form-control option:checked {
            background-color: #18343c;
            color: #00eaff;
            font-weight: 600;
        }

        /* CENTER STATUS MESSAGE */
        .transactions-message {
            width: 100%;
            text-align: center;
        }

        /* CENTER SEARCH BUTTON */
        .transactions-filter input[id$="btnSearch"] {
            justify-self: center;
            min-width: 110px;
        }

        /* IMPROVE SEARCH INPUT */
        .transactions-filter input[id$="txtSearch"] {
            min-height: 48px;
        }

        .transactions-filter input[id$="txtSearch"]:focus {
            border-color: #00eaff;
            box-shadow: 0 0 12px rgba(0,234,255,.2);
        }

        /* TABLE CARD */
        .transactions-table-wrap {
            border-radius: 9px;
        }

        .transactions-table {
            margin-left: auto;
            margin-right: auto;
        }

        /* RESPONSIVE LAYOUT */
        @media(max-width: 700px) {
            .transactions-filter {
                grid-template-columns: 1fr;
                gap: 15px;
            }

            .transactions-filter input[id$="btnSearch"] {
                width: 100%;
                justify-self: stretch;
            }

            .page-heading ~ .card {
                padding: 20px 15px;
            }

            .transactions-table th,
            .transactions-table td {
                padding: 11px 10px;
                font-size: 13px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Transactions</h2>
        <p>View and manage your transactions.</p>
    </div>

    <asp:Label ID="lblMessage" runat="server"
        Visible="false" CssClass="transactions-message" />

    <div class="card">
        <div class="transactions-filter">

            <asp:TextBox ID="txtSearch" runat="server"
                CssClass="form-control"
                placeholder="Search transactions..."
                MaxLength="100" />

            <asp:DropDownList ID="ddlType" runat="server"
                CssClass="form-control">
                <asp:ListItem Text="All Types" Value="All" />
                <asp:ListItem Text="Income" Value="Income" />
                <asp:ListItem Text="Expense" Value="Expense" />
            </asp:DropDownList>

            <asp:Button ID="btnSearch" runat="server"
                Text="Search"
                CssClass="btn btn-green"
                OnClick="btnSearch_Click" />

        </div>
    </div>

    <div class="card">
        <div class="transactions-table-wrap">

            <table class="data-table transactions-table">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Description</th>
                        <th>Category</th>
                        <th>Type</th>
                        <th>Amount</th>
                        <th>Action</th>
                    </tr>
                </thead>

                <tbody>
                    <asp:Repeater ID="rptTransactions" runat="server"
                        OnItemCommand="rptTransactions_ItemCommand">

                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%#: Eval("TransactionDate", "{0:dd MMM yyyy}") %>
                                </td>

                                <td>
                                    <%#: Eval("Title") %>
                                </td>

                                <td>
                                    <%#: Eval("Category") %>
                                </td>

                                <td class='<%# Convert.ToString(Eval("TransactionType")) == "Income" ? "transaction-type-income" : "transaction-type-expense" %>'>
                                    <%#: Eval("TransactionType") %>
                                </td>

                                <td class='<%# Convert.ToString(Eval("TransactionType")) == "Income" ? "transaction-income" : "transaction-expense" %>'>
                                    <%# Convert.ToString(Eval("TransactionType")) == "Income" ? "+" : "-" %><%# string.Format(new System.Globalization.CultureInfo("en-IN"), "₹{0:N2}", Eval("Amount")) %>
                                </td>

                                <td>
                                    <asp:Button ID="btnDelete" runat="server"
                                        Text="Delete"
                                        CssClass="btn btn-red transaction-delete"
                                        CommandName="DeleteTransaction"
                                        CommandArgument='<%# Eval("TransactionId") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Are you sure you want to delete this transaction?');" />
                                </td>
                            </tr>
                        </ItemTemplate>

                    </asp:Repeater>

                    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                        <tr>
                            <td colspan="6" class="transactions-empty">
                                No transactions found.
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>

        </div>
    </div>

</asp:Content>
