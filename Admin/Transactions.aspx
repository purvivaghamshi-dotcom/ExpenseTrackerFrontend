
<%@ Page Title="Transaction Management" Language="C#"
    MasterPageFile="~/Admin/AdminMaster.master"
    AutoEventWireup="true"
    CodeBehind="Transactions.aspx.cs"
    Inherits="ExpenseTrackerFrontend.Admin.Transactions" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="page-heading">
        <h2>Transaction Management</h2>
        <p>Monitor all user transactions.</p>
    </div>

    <div class="card">

        <div class="two-column">

            <asp:TextBox
                ID="txtSearchTransaction"
                runat="server"
                CssClass="form-control"
                placeholder="Search by user, description, amount, or notes..." />

            <asp:DropDownList
                ID="ddlTransactionType"
                runat="server"
                CssClass="form-control">

                <asp:ListItem
                    Text="All Transactions"
                    Value="All Transactions" />

                <asp:ListItem
                    Text="Income"
                    Value="Income" />

                <asp:ListItem
                    Text="Expense"
                    Value="Expense" />

            </asp:DropDownList>

        </div>

        <br />

        <asp:Button
            ID="btnSearchTransactions"
            runat="server"
            Text="Search Transactions"
            CssClass="btn btn-cyan"
            OnClick="btnSearchTransactions_Click"
            CausesValidation="false" />

        &nbsp;

        <asp:Button
            ID="btnClearTransactions"
            runat="server"
            Text="Clear Filters"
            CssClass="btn btn-purple"
            OnClick="btnClearTransactions_Click"
            CausesValidation="false" />

        <br /><br />

        <asp:Label
            ID="lblTransactionMessage"
            runat="server"
            EnableViewState="false"
            ForeColor="#00eaff" />

    </div>

    <div class="card">

        <h3>All User Transactions</h3>

        <div style="width:100%; overflow-x:auto;">

            <asp:GridView
                ID="gvTransactions"
                runat="server"
                CssClass="data-table"
                AutoGenerateColumns="false"
                GridLines="None"
                EmptyDataText="No transactions found.">

                <Columns>

                    <asp:BoundField
                        DataField="UserName"
                        HeaderText="User" />

                    <asp:BoundField
                        DataField="Description"
                        HeaderText="Description" />

                    <asp:BoundField
                        DataField="Notes"
                        HeaderText="Notes" />

                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>

                            <asp:Label
                                ID="lblType"
                                runat="server"
                                Text='<%#: Eval("TransactionType") %>'
                                ForeColor='<%# GetTransactionColor(
                                    Eval("TransactionType")) %>' />

                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="Amount"
                        HeaderText="Amount"
                        DataFormatString="₹{0:N2}"
                        HtmlEncode="false" />

                    <asp:BoundField
                        DataField="TransactionDate"
                        HeaderText="Date"
                        DataFormatString="{0:dd MMM yyyy}"
                        HtmlEncode="false" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
