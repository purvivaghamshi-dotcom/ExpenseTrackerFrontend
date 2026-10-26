
<%@ Page Title="Login" Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="ExpenseTrackerFrontend.User.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login - Expense Tracker</title>

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
            min-height: 100vh;
            background:
                radial-gradient(circle at 20% 20%, rgba(0,229,255,0.08), transparent 30%),
                radial-gradient(circle at 80% 80%, rgba(180,0,255,0.08), transparent 30%),
                linear-gradient(135deg, #050608, #0b0d11 50%, #050608);
            color: white;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .login-container {
            width: 100%;
            max-width: 450px;
            padding: 20px;
        }

        .logo {
            text-align: center;
            font-size: 32px;
            font-weight: 900;
            letter-spacing: 2px;
            margin-bottom: 25px;
            text-shadow: 0 0 10px rgba(0,229,255,0.5);
        }

        .logo span {
            color: #00e5ff;
            text-shadow: 0 0 5px #00e5ff, 0 0 15px rgba(0,229,255,0.7);
        }

        .login-box {
            background: linear-gradient(145deg, #191d23, #0d1015);
            border: 1px solid #3b424c;
            border-radius: 18px;
            padding: 40px;
            box-shadow:
                inset 0 1px 0 rgba(255,255,255,0.08),
                0 15px 45px rgba(0,0,0,0.7),
                0 0 25px rgba(0,229,255,0.06);
        }

        .title {
            text-align: center;
            font-size: 28px;
            margin-bottom: 8px;
        }

        .subtitle {
            text-align: center;
            color: #858e99;
            margin-bottom: 30px;
        }

        .form-group { margin-bottom: 20px; }

        .form-label {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #dce2e8;
        }

        .form-control {
            display: block;
            width: 100%;
            height: 48px;
            background: linear-gradient(180deg, #15191f, #0c0f13);
            color: #fff;
            border: 1px solid #4a525d;
            border-radius: 8px;
            padding: 0 14px;
            outline: none;
            box-shadow: inset 0 2px 4px rgba(0,0,0,0.7);
        }

        .form-control::placeholder { color: #737c87; }

        .form-control:focus {
            border-color: #00e5ff;
            box-shadow: 0 0 0 2px rgba(0,229,255,0.10),
                        0 0 15px rgba(0,229,255,0.20);
        }

        .metal-button {
            display: block;
            width: 100%;
            min-height: 50px;
            text-align: center;
            color: white;
            font-weight: 900;
            letter-spacing: 1px;
            border-radius: 8px;
            border: 1px solid #00c7df;
            background: linear-gradient(
                180deg, #e8faff 0%, #82e8f7 8%, #159db2 48%,
                #087182 52%, #0c5360 92%, #062f38 100%
            );
            color: #fff;
            cursor: pointer;
            box-shadow:
                inset 0 1px 0 rgba(255,255,255,0.65),
                inset 0 -2px 0 rgba(0,0,0,0.5),
                0 0 12px rgba(0,229,255,0.25),
                0 5px 12px rgba(0,0,0,0.6);
            transition: 0.2s;
        }

        .metal-button:hover {
            transform: translateY(-1px);
            box-shadow: 0 0 22px rgba(0,229,255,0.45),
                        0 7px 16px rgba(0,0,0,0.7);
        }

        .register-text {
            text-align: center;
            margin-top: 25px;
            color: #858e99;
        }

        .register-link {
            color: #00e5ff;
            text-decoration: none;
            font-weight: bold;
        }

        .register-link:hover {
            color: white;
            text-shadow: 0 0 8px #00e5ff;
        }

        .login-message {
            display: block;
            margin: 0 0 18px;
            color: #ff8b8b;
            text-align: center;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="login-container">
        <div class="logo">
            EXPENSE<span>TRACKER</span>
        </div>

        <div class="login-box">
            <h1 class="title">Welcome Back</h1>

            <p class="subtitle">
                Login to access your expense dashboard.
            </p>

            <asp:Label ID="lblMessage" runat="server"
                CssClass="login-message"
                EnableViewState="false"
                role="status" />

            <div class="form-group">
                <asp:Label ID="lblEmail" runat="server"
                    AssociatedControlID="txtEmail"
                    CssClass="form-label"
                    Text="Email" />

                <asp:TextBox ID="txtEmail" runat="server"
                    TextMode="Email"
                    MaxLength="256"
                    CssClass="form-control"
                    placeholder="Enter your email"
                    autocomplete="username" />
            </div>

            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server"
                    AssociatedControlID="txtPassword"
                    CssClass="form-label"
                    Text="Password" />

                <asp:TextBox ID="txtPassword" runat="server"
                    TextMode="Password"
                    MaxLength="128"
                    CssClass="form-control"
                    placeholder="Enter your password"
                    autocomplete="current-password" />
            </div>

            <asp:Button ID="btnLogin" runat="server"
                Text="LOGIN"
                CssClass="metal-button"
                OnClick="btnLogin_Click" />

            <div class="register-text">
                Don't have an account?
                <asp:HyperLink ID="lnkRegister" runat="server"
                    NavigateUrl="~/Register.aspx"
                    CssClass="register-link"
                    Text="Create Account" />
            </div>
        </div>
    </div>

</form>
</body>
</html>
