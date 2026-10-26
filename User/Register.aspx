
<%@ Page Title="Register" Language="C#" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Register" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Create Account - Expense Tracker</title>

    <style>
        * { box-sizing: border-box; }

        html, body {
            margin: 0;
            padding: 0;
            width: 100%;
            min-height: 100%;
            font-family: Arial, Helvetica, sans-serif;
        }

        body {
            background:
                radial-gradient(circle at 20% 20%, rgba(0,229,255,.08), transparent 30%),
                radial-gradient(circle at 80% 80%, rgba(180,0,255,.08), transparent 30%),
                linear-gradient(135deg, #050608, #0b0d11 50%, #050608);
            color: #fff;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 25px 12px;
        }

        .register-container {
            width: 100%;
            max-width: 520px;
            padding: 10px;
        }

        .logo {
            text-align: center;
            font-size: 32px;
            font-weight: 900;
            letter-spacing: 2px;
            margin-bottom: 25px;
            text-shadow: 0 0 10px rgba(0,229,255,.5);
        }

        .logo span {
            color: #00e5ff;
            text-shadow: 0 0 5px #00e5ff, 0 0 15px rgba(0,229,255,.7);
        }

        .register-box {
            background: linear-gradient(145deg, #191d23, #0d1015);
            border: 1px solid #3b424c;
            border-radius: 18px;
            padding: 38px;
            box-shadow:
                inset 0 1px 0 rgba(255,255,255,.08),
                inset 0 -1px 0 rgba(0,0,0,.8),
                0 15px 45px rgba(0,0,0,.7),
                0 0 25px rgba(0,229,255,.06);
        }

        .title {
            text-align: center;
            margin: 0 0 8px;
            font-size: 28px;
        }

        .subtitle {
            text-align: center;
            color: #8d96a3;
            margin-bottom: 30px;
            font-size: 14px;
        }

        .form-group { margin-bottom: 18px; }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #dce2e8;
            font-size: 14px;
            font-weight: 600;
        }

        .form-control {
            display: block;
            width: 100%;
            height: 48px;
            background: linear-gradient(180deg, #15191f, #0c0f13) !important;
            color: #fff !important;
            border: 1px solid #4a525d !important;
            border-radius: 8px;
            padding: 0 14px;
            outline: none;
            font-size: 14px;
            box-shadow:
                inset 0 2px 4px rgba(0,0,0,.7),
                inset 0 -1px 0 rgba(255,255,255,.04);
        }

        .form-control::placeholder { color: #737c87; }

        .form-control:focus {
            border-color: #00e5ff !important;
            box-shadow:
                0 0 0 2px rgba(0,229,255,.10),
                0 0 15px rgba(0,229,255,.20),
                inset 0 2px 4px rgba(0,0,0,.7);
        }

        .metal-button {
            display: block;
            width: 100%;
            min-height: 50px;
            border-radius: 8px;
            border: 1px solid #00c7df;
            background: linear-gradient(
                180deg,
                #e8faff 0%,
                #82e8f7 8%,
                #159db2 48%,
                #087182 52%,
                #0c5360 92%,
                #062f38 100%
            );
            color: #fff;
            font-weight: 900;
            font-size: 14px;
            letter-spacing: 1px;
            cursor: pointer;
            box-shadow:
                inset 0 1px 0 rgba(255,255,255,.65),
                inset 0 -2px 0 rgba(0,0,0,.5),
                0 0 12px rgba(0,229,255,.25),
                0 5px 12px rgba(0,0,0,.6);
            transition: .2s ease;
        }

        .metal-button:hover {
            transform: translateY(-1px);
            box-shadow:
                inset 0 1px 0 rgba(255,255,255,.8),
                inset 0 -2px 0 rgba(0,0,0,.5),
                0 0 22px rgba(0,229,255,.45),
                0 7px 16px rgba(0,0,0,.7);
        }

        .message {
            display: block;
            margin: 12px 0;
            color: #ff657d;
            text-align: center;
            font-size: 13px;
            overflow-wrap: anywhere;
        }

        .login-text {
            text-align: center;
            margin-top: 25px;
            color: #858e99;
            font-size: 14px;
        }

        .login-link {
            color: #00e5ff;
            text-decoration: none;
            font-weight: bold;
        }

        .login-link:hover {
            color: #fff;
            text-shadow: 0 0 8px #00e5ff;
        }

        .admin-link {
            display: block;
            text-align: center;
            margin-top: 20px;
            color: #c46cff;
            text-decoration: none;
            font-size: 13px;
        }

        .admin-link:hover {
            color: #fff;
            text-shadow: 0 0 8px #b800ff;
        }

        @media (max-width: 600px) {
            .register-container { padding: 5px; }
            .register-box { padding: 25px 20px; }
            .logo { font-size: 26px; }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="register-container">

        <div class="logo">
            EXPENSE<span>TRACKER</span>
        </div>

        <div class="register-box">

            <h1 class="title">Create Your Account</h1>

            <p class="subtitle">
                Start managing your expenses smarter.
            </p>

            <div class="form-group">
                <label for="<%= txtFullName.ClientID %>">Full Name</label>
                <asp:TextBox
                    ID="txtFullName"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Enter your full name"
                    MaxLength="150"
                    autocomplete="name" />
            </div>

            <div class="form-group">
                <label for="<%= txtEmail.ClientID %>">Email</label>
                <asp:TextBox
                    ID="txtEmail"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Email"
                    placeholder="Enter your email"
                    MaxLength="256"
                    autocomplete="email" />
            </div>

            <div class="form-group">
                <label for="<%= txtPassword.ClientID %>">Password</label>
                <asp:TextBox
                    ID="txtPassword"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    placeholder="Create password"
                    autocomplete="new-password" />
            </div>

            <div class="form-group">
                <label for="<%= txtConfirmPassword.ClientID %>">
                    Confirm Password
                </label>
                <asp:TextBox
                    ID="txtConfirmPassword"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    placeholder="Confirm password"
                    autocomplete="new-password" />
            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message"
                EnableViewState="false" />

            <asp:Button
                ID="btnRegister"
                runat="server"
                Text="CREATE ACCOUNT"
                CssClass="metal-button"
                OnClick="btnRegister_Click"
                CausesValidation="false" />

            <div class="login-text">
                Already have an account?
                <a href="<%= ResolveUrl("~/User/Login.aspx") %>"
                   class="login-link">Login here</a>
            </div>

            <a href="<%= ResolveUrl("~/Admin/AdminLogin.aspx") %>"
               class="admin-link">
                Administrator Login
            </a>

        </div>
    </div>

</form>
</body>
</html>
