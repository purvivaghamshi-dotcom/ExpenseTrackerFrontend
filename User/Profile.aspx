
<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .profile-muted {
            color: #858b95;
            margin-bottom: 8px;
        }

        .profile-value {
            display: block;
            color: #00e5ff;
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 18px;
            overflow-wrap: anywhere;
        }

        .profile-message {
            display: block;
            margin: 12px 0;
            font-weight: 600;
        }

        .profile-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 18px;
        }

        .profile-divider {
            border: 0;
            border-top: 1px solid #30343b;
            margin: 22px 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-heading">
        <h2>My Profile</h2>
        <p>Manage your account information and security.</p>
    </div>

    <asp:Label ID="lblPageMessage" runat="server"
        CssClass="profile-message" />

    <div class="two-column">

        <div class="card">
            <h3>Personal Information</h3>

            <div class="form-group">
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server"
                    CssClass="form-control"
                    MaxLength="100"
                    placeholder="Enter your full name" />
            </div>

            <div class="form-group">
                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server"
                    CssClass="form-control"
                    TextMode="Email"
                    MaxLength="150"
                    placeholder="Enter your email address" />
            </div>

            <div class="form-group">
                <label>Account Type</label>
                <asp:Label ID="lblAccountType" runat="server"
                    CssClass="profile-value" />
            </div>

            <div class="form-group">
                <label>Member Since</label>
                <asp:Label ID="lblCreatedAt" runat="server"
                    CssClass="profile-value" />
            </div>

            <asp:Label ID="lblPersonalMessage" runat="server"
                CssClass="profile-message" />

            <div class="profile-actions">
                <asp:Button ID="btnSaveProfile" runat="server"
                    Text="SAVE CHANGES"
                    CssClass="btn btn-cyan"
                    OnClick="btnSaveProfile_Click" />
            </div>
        </div>

        <div class="card">
            <h3>Account Security</h3>

            <p class="profile-muted">Change your password</p>

            <div class="form-group">
                <label>Current Password</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="200"
                    autocomplete="current-password"
                    placeholder="Enter current password" />
            </div>

            <div class="form-group">
                <label>New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="200"
                    autocomplete="new-password"
                    placeholder="Enter new password" />
            </div>

            <div class="form-group">
                <label>Confirm New Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="200"
                    autocomplete="new-password"
                    placeholder="Confirm new password" />
            </div>

            <asp:Label ID="lblPasswordMessage" runat="server"
                CssClass="profile-message" />

            <div class="profile-actions">
                <asp:Button ID="btnChangePassword" runat="server"
                    Text="CHANGE PASSWORD"
                    CssClass="btn btn-purple"
                    OnClick="btnChangePassword_Click" />
            </div>

            <hr class="profile-divider" />

            <p class="profile-muted">
                Keep your password private and use a strong password.
            </p>
        </div>

    </div>

</asp:Content>
