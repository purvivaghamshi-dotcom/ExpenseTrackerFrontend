
<%@ Page Title="Admin Login" Language="C#"
    MasterPageFile="~/Admin/AdminMaster.master"
    AutoEventWireup="true"
    CodeBehind="AdminLogin.aspx.cs"
    Inherits="ExpenseTrackerFrontend.Admin.AdminLogin" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="page-heading">
        <h2>Administrator Login</h2>
        <p>Access the ExpenseTracker management system.</p>
    </div>

    <div class="card" style="max-width:500px;">

        <div class="form-group">
            <label for="<%= txtAdminEmail.ClientID %>">
                Admin Email
            </label>

            <asp:TextBox
                ID="txtAdminEmail"
                runat="server"
                CssClass="form-control"
                TextMode="Email"
                MaxLength="256"
                placeholder="Enter your registered admin email" />
        </div>

        <div class="form-group">
            <label for="<%= txtAdminPassword.ClientID %>">
                Password
            </label>

            <asp:TextBox
                ID="txtAdminPassword"
                runat="server"
                CssClass="form-control"
                TextMode="Password"
                placeholder="Enter your password" />
        </div>

        <asp:Label
            ID="lblAdminMessage"
            runat="server"
            EnableViewState="false"
            ForeColor="#ff657d" />

        <br /><br />

        <asp:Button
            ID="btnAdminLogin"
            runat="server"
            Text="ADMIN LOGIN"
            CssClass="btn btn-purple"
            OnClick="btnAdminLogin_Click"
            CausesValidation="false" />

    </div>

</asp:Content>
