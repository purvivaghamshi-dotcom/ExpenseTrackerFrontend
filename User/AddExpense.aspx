
<%@ Page Title="Add Expense" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AddExpense.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.AddExpense" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .expense-message {
            display: block;
            margin: 0 0 18px 0;
            padding: 12px;
            border-radius: 8px;
        }

        .expense-success {
            color: #39ff88;
            border: 1px solid #39ff88;
            background: rgba(57,255,136,0.08);
        }

        .expense-error {
            color: #ff6b81;
            border: 1px solid #ff3b6b;
            background: rgba(255,59,107,0.08);
        }

        /* METALLIC NEON DROPDOWN STYLING */
        select.form-control {
            display: block;
            width: 100%;
            min-height: 48px;
            padding: 0 42px 0 14px;

            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 14px;
            font-weight: 500;
            letter-spacing: 0.3px;

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
                inset 0 1px 1px rgba(255,255,255,0.09),
                inset 0 -2px 4px rgba(0,0,0,0.55),
                0 3px 8px rgba(0,0,0,0.3);

            transition:
                border-color 0.25s ease,
                box-shadow 0.25s ease,
                background-color 0.25s ease;
        }

        /* HOVER EFFECT */
        select.form-control:hover {
            color: #ffffff !important;
            border-color: #00bcd4 !important;

            box-shadow:
                inset 0 1px 1px rgba(255,255,255,0.1),
                0 0 8px rgba(0,234,255,0.16),
                0 4px 12px rgba(0,0,0,0.4);
        }

        /* CYAN FOCUS GLOW */
        select.form-control:focus {
            color: #ffffff !important;
            border-color: #00eaff !important;

            box-shadow:
                0 0 0 2px rgba(0,234,255,0.12),
                0 0 15px rgba(0,234,255,0.24),
                inset 0 2px 5px rgba(0,0,0,0.4);
        }

        /* DARK DROPDOWN OPTIONS */
        select.form-control option {
            background-color: #111419;
            color: #e8edf3;
            padding: 12px;
            font-size: 14px;
        }

        /* SELECTED OPTION */
        select.form-control option:checked {
            background-color: #18343c;
            color: #00eaff;
            font-weight: 600;
        }

        /* DISABLED DROPDOWN, IF USED LATER */
        select.form-control:disabled {
            opacity: 0.55;
            cursor: not-allowed;
            box-shadow: none;
        }
        
/* CENTER THE ENTIRE ADD EXPENSE FORM */

.page-heading {
    width: 100%;
    text-align: center;
    margin-left: auto;
    margin-right: auto;
}

/* Center the form card on the page */
.page-heading + .card {
    width: 100%;
    max-width: 800px;
    margin: 0 auto 30px;
}

/* Center field labels */
.page-heading + .card .form-group label {
    display: block;
    text-align: center;
    margin-bottom: 10px;
}

/* Keep the dropdowns and inputs evenly aligned */
.page-heading + .card .form-control {
    width: 100%;
    display: block;
}

/* Center the success or error message */
.page-heading + .card .expense-message {
    text-align: center;
}

/* Center the Add Expense button */
.page-heading + .card input[id$="btnAddExpense"] {
    display: block;
    min-width: 200px;
    margin: 24px auto 0;
}

/* Keep the two-column fields balanced */
.page-heading + .card .two-column {
    align-items: start;
    gap: 22px;
}

/* Responsive layout for smaller screens */
@media (max-width: 600px) {
    .page-heading + .card {
        padding: 22px 16px;
    }

    .page-heading + .card .two-column {
        grid-template-columns: 1fr;
        gap: 0;
    }

    .page-heading + .card input[id$="btnAddExpense"] {
        width: 100%;
    }
}

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Add Expense</h2>
        <p>Record a new expense.</p>
    </div>

    <div class="card" style="max-width:800px;">

        <asp:Label ID="lblMessage" runat="server"
            Visible="false" CssClass="expense-message" />

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

        <div class="two-column">
            <div class="form-group">
                <label for="<%= ddlCategory.ClientID %>">Category</label>
                <asp:DropDownList ID="ddlCategory" runat="server"
                    CssClass="form-control">
                    <asp:ListItem Text="Food" Value="Food" />
                    <asp:ListItem Text="Travel" Value="Travel" />
                    <asp:ListItem Text="Shopping" Value="Shopping" />
                    <asp:ListItem Text="Bills" Value="Bills" />
                    <asp:ListItem Text="Education" Value="Education" />
                    <asp:ListItem Text="Entertainment" Value="Entertainment" />
                    <asp:ListItem Text="Other" Value="Other" />
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="<%= ddlPaymentMethod.ClientID %>">Payment Method</label>
                <asp:DropDownList ID="ddlPaymentMethod" runat="server"
                    CssClass="form-control">
                    <asp:ListItem Text="Cash" Value="Cash" />
                    <asp:ListItem Text="UPI" Value="UPI" />
                    <asp:ListItem Text="Credit Card" Value="Credit Card" />
                    <asp:ListItem Text="Debit Card" Value="Debit Card" />
                    <asp:ListItem Text="Bank Transfer" Value="Bank Transfer" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-group">
            <label for="<%= txtDescription.ClientID %>">Description</label>
            <asp:TextBox ID="txtDescription" runat="server"
                CssClass="form-control"
                TextMode="MultiLine"
                Rows="4"
                placeholder="Enter expense description" />
        </div>

        <asp:Button ID="btnAddExpense" runat="server"
            Text="ADD EXPENSE"
            CssClass="btn btn-orange"
            OnClick="btnAddExpense_Click" />

    </div>

</asp:Content>
