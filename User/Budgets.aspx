
<%@ Page Title="Budgets" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Budgets.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Budgets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .budget-form {
            margin-bottom: 25px;
        }

        .budget-actions {
            margin-top: 18px;
        }

        .budget-message {
            display: block;
            margin-top: 14px;
            font-weight: 600;
        }

        .budget-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 20px;
            margin-top: 24px;
        }

        .budget-card {
            min-width: 0;
        }

        .budget-card h3 {
            margin-bottom: 12px;
        }

        .budget-detail {
            margin: 8px 0;
            overflow-wrap: anywhere;
        }

        .budget-progress-track {
            width: 100%;
            height: 10px;
            margin: 15px 0;
            overflow: hidden;
            border-radius: 20px;
            background: rgba(255,255,255,0.12);
        }

        .budget-progress-fill {
            height: 100%;
            border-radius: 20px;
            background: linear-gradient(90deg, #a855f7, #22d3ee);
            transition: width 0.3s ease;
        }

        .budget-over {
            background: linear-gradient(90deg, #ef4444, #f97316);
        }

        .budget-status {
            font-weight: 600;
            margin: 10px 0;
        }

        .budget-delete {
            margin-top: 12px;
        }

        .budget-empty {
            margin-top: 20px;
            padding: 20px;
            text-align: center;
        }

        .budget-form .form-control {
            width: 100%;
            box-sizing: border-box;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Monthly Budgets</h2>
        <p>Control your spending limits and track your monthly expenses.</p>
    </div>

    <div class="card budget-form">

        <h3>Create a Budget</h3>

        <div class="two-column">

            <div class="form-group">
                <label for="<%= ddlCategory.ClientID %>">Budget Category</label>

                <asp:DropDownList
                    ID="ddlCategory"
                    runat="server"
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
                <label for="<%= txtBudgetAmount.ClientID %>">Budget Limit (₹)</label>

                <asp:TextBox
                    ID="txtBudgetAmount"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Number"
                    step="0.01"
                    min="0.01"
                    placeholder="Enter budget amount" />
            </div>

        </div>

        <div class="budget-actions">
            <asp:Button
                ID="btnCreateBudget"
                runat="server"
                Text="CREATE BUDGET"
                CssClass="btn btn-purple"
                OnClick="btnCreateBudget_Click" />
        </div>

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="budget-message"
            EnableViewState="false" />

    </div>

    <div class="page-heading">
        <h2>This Month's Budgets</h2>
        <p>
            <asp:Label ID="lblCurrentMonth" runat="server" />
        </p>
    </div>

    <asp:Panel ID="pnlNoBudgets" runat="server"
        CssClass="card budget-empty" Visible="false">
        <h3>No budgets created yet</h3>
        <p>Create your first budget above to start tracking your expenses.</p>
    </asp:Panel>

    <div class="budget-grid">

        <asp:Repeater
            ID="rptBudgets"
            runat="server"
            OnItemCommand="rptBudgets_ItemCommand">

            <ItemTemplate>

                <div class="card budget-card">

                    <h3><%#: Eval("BudgetName") %></h3>

                    <p class="budget-detail">
                        Spent:
                        <strong><%#: Eval("AmountSpent", "{0:C}") %></strong>
                    </p>

                    <p class="budget-detail">
                        Budget limit:
                        <strong><%#: Eval("BudgetAmount", "{0:C}") %></strong>
                    </p>

                    <p class="budget-detail">
                        Remaining:
                        <strong><%#: Eval("RemainingAmount", "{0:C}") %></strong>
                    </p>

                    <div class="budget-progress-track">
                        <div
                            class='<%# Convert.ToDecimal(Eval("AmountSpent")) >= Convert.ToDecimal(Eval("BudgetAmount")) ? "budget-progress-fill budget-over" : "budget-progress-fill" %>'
                            style='width: <%#: Eval("ProgressWidth") %>%;'>
                        </div>
                    </div>

                    <p class="budget-status">
                        <%#: Eval("BudgetStatus") %>
                    </p>

                    <asp:Button
                        ID="btnDeleteBudget"
                        runat="server"
                        Text="DELETE BUDGET"
                        CssClass="btn btn-red budget-delete"
                        CommandName="DeleteBudget"
                        CommandArgument='<%# Eval("BudgetId") %>'
                        CausesValidation="false"
                        OnClientClick="return confirm('Are you sure you want to delete this budget?');" />

                </div>

            </ItemTemplate>

        </asp:Repeater>

    </div>

</asp:Content>
