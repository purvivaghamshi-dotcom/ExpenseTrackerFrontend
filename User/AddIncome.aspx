
<%@ Page Title="Add Income" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AddIncome.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.AddIncome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .income-message {
            display: block;
            margin-bottom: 18px;
            padding: 12px;
            border-radius: 8px;
        }

        .income-success {
            color: #39ff88;
            border: 1px solid #39ff88;
            background: rgba(57,255,136,0.08);
        }

        .income-error {
            color: #ff6b81;
            border: 1px solid #ff3b6b;
            background: rgba(255,59,107,0.08);
        }

        /* CENTER THE HEADING */
        .page-heading {
            width: 100%;
            text-align: center;
            margin-left: auto;
            margin-right: auto;
        }

        /* CENTER THE FORM CARD */
        .page-heading + .card {
            width: 100%;
            max-width: 800px;
            margin: 0 auto 30px;
            padding: 30px;
        }

        /* CENTER FIELD LABELS */
        .page-heading + .card .form-group label {
            display: block;
            text-align: center;
            margin-bottom: 10px;
            color: #dce2e8;
        }

        /* KEEP INPUTS AND TEXT AREAS CONSISTENT */
        .page-heading + .card .form-control {
            display: block;
            width: 100%;
        }

        /* METALLIC NEON DROPDOWN */
        .page-heading + .card select.form-control {
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
        .page-heading + .card select.form-control:hover {
            border-color: #00bcd4 !important;
            box-shadow:
                inset 0 1px 1px rgba(255,255,255,.1),
                0 0 8px rgba(0,234,255,.16),
                0 4px 12px rgba(0,0,0,.4);
        }

        /* DROPDOWN FOCUS */
        .page-heading + .card select.form-control:focus {
            border-color: #00eaff !important;
            box-shadow:
                0 0 0 2px rgba(0,234,255,.12),
                0 0 15px rgba(0,234,255,.24),
                inset 0 2px 5px rgba(0,0,0,.4);
        }

        /* DROPDOWN OPTIONS */
        .page-heading + .card select.form-control option {
            background-color: #111419;
            color: #e8edf3;
            padding: 12px;
            font-size: 14px;
        }

        .page-heading + .card select.form-control option:checked {
            background-color: #18343c;
            color: #00eaff;
        }

        /* CENTER SUCCESS AND ERROR MESSAGES */
        .page-heading + .card .income-message {
            text-align: center;
        }

        /* CENTER THE ADD INCOME BUTTON */
        .page-heading + .card input[id$="btnAddIncome"] {
            display: block;
            min-width: 200px;
            margin: 24px auto 0;
        }

        /* KEEP AMOUNT AND DATE COLUMNS EVEN */
        .page-heading + .card .two-column {
            align-items: start;
            gap: 22px;
        }

        /* MOBILE RESPONSIVENESS */
        @media (max-width: 600px) {
            .page-heading + .card {
                padding: 22px 16px;
            }

            .page-heading + .card .two-column {
                grid-template-columns: 1fr;
                gap: 0;
            }

            .page-heading + .card input[id$="btnAddIncome"] {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Add Income</h2>
        <p>Record money received.</p>
    </div>

    <div class="card" style="max-width:800px;">

        <asp:Label ID="lblMessage" runat="server"
            Visible="false" CssClass="income-message" />

        <div class="two-column">
            <div class="form-group">
                <label for="<%= txtAmount.ClientID %>">Amount</label>
                <asp:TextBox ID="txtAmount" runat="server"
                    CssClass="form-control"
                    TextMode="Number"
                    placeholder="₹ Enter amount" />
            </div>

            <div class="form-group">
                <label for="<%= txtDate.ClientID %>">Date</label>
                <asp:TextBox ID="txtDate" runat="server"
                    CssClass="form-control"
                    TextMode="Date" />
            </div>
        </div>

        <div class="form-group">
            <label for="<%= ddlIncomeSource.ClientID %>">Income Source</label>
            <asp:DropDownList ID="ddlIncomeSource" runat="server"
                CssClass="form-control">
                <asp:ListItem Text="Salary" Value="Salary" />
                <asp:ListItem Text="Freelance" Value="Freelance" />
                <asp:ListItem Text="Business" Value="Business" />
                <asp:ListItem Text="Scholarship" Value="Scholarship" />
                <asp:ListItem Text="Gift" Value="Gift" />
                <asp:ListItem Text="Other" Value="Other" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="<%= txtDescription.ClientID %>">Description</label>
            <asp:TextBox ID="txtDescription" runat="server"
                CssClass="form-control"
                TextMode="MultiLine"
                Rows="4"
                placeholder="Enter income description" />
        </div>

        <asp:Button ID="btnAddIncome" runat="server"
            Text="ADD INCOME"
            CssClass="btn btn-green"
            OnClick="btnAddIncome_Click" />

    </div>

</asp:Content>
