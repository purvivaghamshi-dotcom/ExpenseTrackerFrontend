
<%@ Page Title="Split Expenses" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="SplitExpenses.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.SplitExpenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .split-form .form-control {
            width: 100%;
            box-sizing: border-box;
        }

        .split-form .form-group {
            margin-bottom: 18px;
        }

        .split-message {
            display: block;
            margin-top: 14px;
            font-weight: 600;
        }

        .split-list {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }

        .split-card {
            min-width: 0;
        }

        .split-card h3 {
            overflow-wrap: anywhere;
        }

        .split-info {
            margin: 10px 0;
        }

        .split-table-wrap {
            overflow-x: auto;
            margin-top: 15px;
        }

        .split-table-wrap .data-table {
            width: 100%;
        }

        .split-table-wrap select {
            max-width: 125px;
            padding: 6px;
            border-radius: 6px;
        }

        .split-empty {
            margin-top: 20px;
            padding: 20px;
            text-align: center;
        }

        .split-actions {
            margin-top: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>Split Expenses</h2>
        <p>Split bills with friends and track payments.</p>
    </div>

    <div class="card split-form">

        <h3>Create a New Split</h3>

        <div class="two-column">

            <div class="form-group">
                <label for="<%= txtExpenseName.ClientID %>">Expense Name</label>
                <asp:TextBox
                    ID="txtExpenseName"
                    runat="server"
                    CssClass="form-control"
                    MaxLength="150"
                    placeholder="Dinner with friends" />
            </div>

            <div class="form-group">
                <label for="<%= txtTotalAmount.ClientID %>">Total Amount (₹)</label>
                <asp:TextBox
                    ID="txtTotalAmount"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Number"
                    step="0.01"
                    placeholder="2000" />
            </div>

        </div>

        <div class="two-column">

            <div class="form-group">
                <label for="<%= txtPeople.ClientID %>">Participant Names</label>
                <asp:TextBox
                    ID="txtPeople"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Rahul, Neha, Amit" />
                <small>Separate names with commas.</small>
            </div>

            <div class="form-group">
                <label>Number of People</label>
                <p>Automatically calculated from the names you enter.</p>
            </div>

        </div>

        <asp:Button
            ID="btnCreateSplit"
            runat="server"
            Text="CREATE SPLIT"
            CssClass="btn btn-cyan"
            OnClick="btnCreateSplit_Click" />

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="split-message"
            EnableViewState="false" />

    </div>

    <div class="page-heading">
        <h2>Your Saved Splits</h2>
        <p>View each person's share and update payment status.</p>
    </div>

    <asp:Panel ID="pnlNoSplits" runat="server"
        CssClass="card split-empty" Visible="false">
        <h3>No split expenses yet</h3>
        <p>Create your first split above.</p>
    </asp:Panel>

    <div class="split-list">

        <asp:Repeater
            ID="rptSplits"
            runat="server"
            OnItemCommand="rptSplits_ItemCommand">

            <ItemTemplate>

                <div class="card split-card">

                    <h3><%#: Eval("ExpenseName") %></h3>

                    <p class="split-info">
                        Total Bill:
                        <strong><%#: Eval("TotalAmount", "{0:C}") %></strong>
                    </p>

                    <p class="split-info">
                        People:
                        <strong><%#: Eval("NumberOfPeople") %></strong>
                    </p>

                    <div class="split-table-wrap">
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>Person</th>
                                    <th>Share</th>
                                    <th>Status</th>
                                    <th>Update</th>
                                </tr>
                            </thead>

                            <tbody>
                                <asp:Repeater
                                    ID="rptParticipants"
                                    runat="server"
                                    DataSource='<%# Eval("Participants") %>'
                                    OnItemCommand="rptParticipants_ItemCommand">

                                    <ItemTemplate>
                                        <tr>
                                            <td><%#: Eval("PersonName") %></td>
                                            <td><%#: Eval("ShareAmount", "{0:C}") %></td>
                                            <td><%#: Eval("PaymentStatus") %></td>
                                            <td>
                                                <asp:DropDownList
                                                    ID="ddlPaymentStatus"
                                                    runat="server"
                                                    SelectedValue='<%# Eval("PaymentStatus") %>'>
                                                    <asp:ListItem Text="Pending" Value="Pending" />
                                                    <asp:ListItem Text="Paid" Value="Paid" />
                                                </asp:DropDownList>

                                                <asp:Button
                                                    ID="btnSaveStatus"
                                                    runat="server"
                                                    Text="Save"
                                                    CssClass="btn btn-cyan"
                                                    CommandName="UpdateStatus"
                                                    CommandArgument='<%# Eval("ParticipantId") %>'
                                                    CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>

                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="split-actions">
                        <asp:Button
                            ID="btnDeleteSplit"
                            runat="server"
                            Text="DELETE SPLIT"
                            CssClass="btn btn-red"
                            CommandName="DeleteSplit"
                            CommandArgument='<%# Eval("SplitId") %>'
                            CausesValidation="false"
                            OnClientClick="return confirm('Delete this split and its participant records?');" />
                    </div>

                </div>

            </ItemTemplate>

        </asp:Repeater>

    </div>

</asp:Content>
