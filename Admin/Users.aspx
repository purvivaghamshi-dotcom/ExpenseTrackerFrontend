
<%@ Page Title="User Management" Language="C#"
    MasterPageFile="~/Admin/AdminMaster.master"
    AutoEventWireup="true"
    CodeBehind="Users.aspx.cs"
    Inherits="ExpenseTrackerFrontend.Admin.Users" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="page-heading">
        <h2>User Management</h2>
        <p>View and manage registered users.</p>
    </div>

    <div class="card">

        <div class="form-group">
            <asp:TextBox
                ID="txtSearchUsers"
                runat="server"
                CssClass="form-control"
                placeholder="Search by ID, name, email, or role..." />
        </div>

        <asp:Button
            ID="btnSearch"
            runat="server"
            Text="Search Users"
            CssClass="btn btn-cyan"
            OnClick="btnSearch_Click"
            CausesValidation="false" />

        &nbsp;

        <asp:Button
            ID="btnClearSearch"
            runat="server"
            Text="Clear Search"
            CssClass="btn btn-purple"
            OnClick="btnClearSearch_Click"
            CausesValidation="false" />

        <br /><br />

        <asp:Label
            ID="lblMessage"
            runat="server"
            EnableViewState="false"
            ForeColor="#00eaff" />

    </div>

    <div class="card">

        <h3>Registered Users</h3>

        <div style="width:100%; overflow-x:auto;">

            <asp:GridView
                ID="gvUsers"
                runat="server"
                CssClass="data-table"
                AutoGenerateColumns="false"
                DataKeyNames="UserId"
                GridLines="None"
                EmptyDataText="No registered users found."
                OnRowCommand="gvUsers_RowCommand">

                <Columns>

                    <asp:BoundField
                        DataField="UserId"
                        HeaderText="ID" />

                    <asp:BoundField
                        DataField="FullName"
                        HeaderText="Name" />

                    <asp:BoundField
                        DataField="Email"
                        HeaderText="Email" />

                    <asp:BoundField
                        DataField="TransactionCount"
                        HeaderText="Transactions" />

                    <asp:BoundField
                        DataField="Role"
                        HeaderText="Role" />

                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>

                            <asp:Button
                                ID="btnViewUser"
                                runat="server"
                                Text="View"
                                CssClass="btn btn-cyan"
                                CommandName="ViewUser"
                                CommandArgument='<%# Eval("UserId") %>'
                                CausesValidation="false" />

                            &nbsp;

                            <asp:Button
                                ID="btnDeleteUser"
                                runat="server"
                                Text="Delete"
                                CssClass="btn btn-red"
                                CommandName="DeleteUser"
                                CommandArgument='<%# Eval("UserId") %>'
                                CausesValidation="false"
                                OnClientClick="return confirm('Delete this account? Accounts with transactions or budgets cannot be deleted.');" />

                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>
    </div>

    <asp:Panel
        ID="pnlUserDetails"
        runat="server"
        CssClass="card"
        Visible="false">

        <h3>User Details</h3>

        <p>
            <strong>User ID:</strong>
            <asp:Label ID="lblDetailId" runat="server" />
        </p>

        <p>
            <strong>Full Name:</strong>
            <asp:Label ID="lblDetailName" runat="server" />
        </p>

        <p>
            <strong>Email:</strong>
            <asp:Label ID="lblDetailEmail" runat="server" />
        </p>

        <p>
            <strong>Role:</strong>
            <asp:Label ID="lblDetailRole" runat="server" />
        </p>

        <p>
            <strong>Registered On:</strong>
            <asp:Label ID="lblDetailCreatedAt" runat="server" />
        </p>

    </asp:Panel>

</asp:Content>
