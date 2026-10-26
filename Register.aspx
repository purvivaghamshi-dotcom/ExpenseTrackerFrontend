
<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ExpenseTrackerFrontend._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="register-container">
        <div class="register-card">
            <h1>Create Account</h1>
            <p class="register-subtitle">Start managing your expenses today.</p>

            <asp:Label ID="lblMessage" runat="server"
                EnableViewState="false"
                CssClass="register-message"
                role="status" />

            <div class="register-field">
                <asp:Label ID="lblFullName" runat="server"
                    AssociatedControlID="txtFullName"
                    Text="Full Name" />

                <asp:TextBox ID="txtFullName" runat="server"
                    MaxLength="100"
                    CssClass="register-input"
                    placeholder="Enter your full name" />

                <asp:RequiredFieldValidator ID="valFullName" runat="server"
                    ControlToValidate="txtFullName"
                    ErrorMessage="Please enter your full name."
                    Display="Dynamic"
                    CssClass="register-error" />
            </div>

            <div class="register-field">
                <asp:Label ID="lblEmail" runat="server"
                    AssociatedControlID="txtEmail"
                    Text="Email Address" />

                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email"
                    MaxLength="256"
                    CssClass="register-input"
                    placeholder="Enter your email address" />

                <asp:RequiredFieldValidator ID="valEmail" runat="server"
                    ControlToValidate="txtEmail"
                    ErrorMessage="Please enter your email."
                    Display="Dynamic"
                    CssClass="register-error" />

                <asp:RegularExpressionValidator ID="valEmailFormat" runat="server"
                    ControlToValidate="txtEmail"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                    ErrorMessage="Please enter a valid email address."
                    Display="Dynamic"
                    CssClass="register-error" />
            </div>

            <div class="register-field">
                <asp:Label ID="lblPassword" runat="server"
                    AssociatedControlID="txtPassword"
                    Text="Password" />

                <asp:TextBox ID="txtPassword" runat="server"
                    TextMode="Password"
                    MaxLength="128"
                    CssClass="register-input"
                    placeholder="Create a password" />

                <asp:RequiredFieldValidator ID="valPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ErrorMessage="Please enter a password."
                    Display="Dynamic"
                    CssClass="register-error" />

                <asp:RegularExpressionValidator ID="valPasswordLength" runat="server"
                    ControlToValidate="txtPassword"
                    ValidationExpression="^[\s\S]{8,128}$"
                    ErrorMessage="Password must contain at least 8 characters."
                    Display="Dynamic"
                    CssClass="register-error" />
            </div>

            <div class="register-field">
                <asp:Label ID="lblConfirmPassword" runat="server"
                    AssociatedControlID="txtConfirmPassword"
                    Text="Confirm Password" />

                <asp:TextBox ID="txtConfirmPassword" runat="server"
                    TextMode="Password"
                    MaxLength="128"
                    CssClass="register-input"
                    placeholder="Enter password again" />

                <asp:RequiredFieldValidator ID="valConfirmPassword" runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Please confirm your password."
                    Display="Dynamic"
                    CssClass="register-error" />

                <asp:CompareValidator ID="valPasswordMatch" runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ControlToCompare="txtPassword"
                    ErrorMessage="Passwords do not match."
                    Display="Dynamic"
                    CssClass="register-error" />
            </div>

            <asp:Button ID="btnRegister" runat="server"
                Text="CREATE ACCOUNT"
                CssClass="register-button"
                OnClick="btnRegister_Click" />

            <p class="register-login-link">
                Already have an account?
                <asp:HyperLink ID="lnkLogin" runat="server"
                    NavigateUrl="~/User/Login.aspx"
                    Text="Login" />
            </p>
        </div>
    </div>

    <style>
        .register-container {
            width: 100%;
            max-width: 520px;
            margin: 35px auto;
            padding: 15px;
        }

        .register-card {
            padding: 32px;
            border: 1px solid #444;
            border-radius: 16px;
            background: #111;
            color: #f5f5f5;
            box-shadow: 0 0 18px rgba(0, 255, 200, 0.10);
        }

        .register-card h1 {
            text-align: center;
            font-size: 30px;
            margin: 0 0 8px;
            color: #00f5c8;
        }

        .register-subtitle {
            text-align: center;
            color: #bbb;
            margin-bottom: 24px;
        }

        .register-field {
            margin-bottom: 17px;
        }

        .register-field label {
            display: block;
            margin-bottom: 7px;
            color: #ddd;
            font-weight: 600;
        }

        .register-input {
            display: block;
            box-sizing: border-box;
            width: 100%;
            padding: 12px 14px;
            color: #fff;
            background: #202020;
            border: 1px solid #555;
            border-radius: 7px;
        }

        .register-input:focus {
            outline: none;
            border-color: #00f5c8;
            box-shadow: 0 0 7px rgba(0, 245, 200, 0.25);
        }

        .register-button {
            width: 100%;
            padding: 13px;
            border: 1px solid #00f5c8;
            border-radius: 8px;
            color: #07120f;
            background: linear-gradient(135deg, #00f5c8, #a0ffe9);
            font-weight: bold;
            cursor: pointer;
        }

        .register-button:hover {
            filter: brightness(1.08);
        }

        .register-message {
            display: block;
            margin-bottom: 15px;
            color: #00f5c8;
        }

        .register-error {
            display: block;
            margin-top: 5px;
            color: #ff7070;
            font-size: 13px;
        }

        .register-login-link {
            text-align: center;
            margin: 20px 0 0;
            color: #ccc;
        }

        .register-login-link a {
            color: #00f5c8;
        }
    </style>

</asp:Content>
