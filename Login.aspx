<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" CodePage="65001" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Login - Shipping Document Control</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Thai:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        :root {
            --page-bg: #f3f6fa;
            --surface: #ffffff;
            --primary: #1f4e79;
            --primary-deep: #173a59;
            --text-main: #1f2933;
            --text-soft: #5b6b7a;
            --line-soft: #e8eef5;
        }

        body {
            min-height: 100vh;
            margin: 0;
            font-family: 'Noto Sans Thai', 'Segoe UI', Tahoma, sans-serif;
            color: var(--text-main);
            background: var(--page-bg);
        }

        .login-shell {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
        }

        .login-panel {
            width: 100%;
            max-width: 420px;
            padding: 28px;
            background: var(--surface);
            border: 1px solid var(--line-soft);
            border-radius: 12px;
            box-shadow: 0 12px 30px rgba(15, 35, 55, 0.08);
        }

        .login-title {
            margin: 0;
            font-size: 1.5rem;
            font-weight: 700;
            letter-spacing: 0;
        }

        .login-subtitle {
            margin: 8px 0 24px;
            color: var(--text-soft);
            font-size: 0.92rem;
        }

        .field-label {
            display: block;
            margin-bottom: 6px;
            font-size: 0.85rem;
            font-weight: 600;
        }

        .field-control {
            width: 100%;
            min-height: 42px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            padding: 8px 12px;
            font-size: 0.92rem;
        }

        .field-control:focus {
            outline: none;
            border-color: rgba(31, 78, 121, 0.55);
            box-shadow: 0 0 0 0.15rem rgba(31, 78, 121, 0.12);
        }

        .login-button {
            width: 100%;
            min-height: 44px;
            border: 0;
            border-radius: 8px;
            background: var(--primary);
            color: #fff;
            font-weight: 700;
        }

        .login-button:hover {
            background: var(--primary-deep);
        }

        .error-message {
            display: block;
            margin-bottom: 16px;
            padding: 10px 12px;
            border-radius: 8px;
            background: #fff3f3;
            border: 1px solid #ffd4d4;
            color: #a61b1b;
            font-size: 0.86rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <main class="login-shell">
            <section class="login-panel" aria-label="Login">
                <h1 class="login-title">Shipping Document Control</h1>
                <asp:Label ID="lbError" runat="server" CssClass="error-message" Visible="false" />

                <div class="mb-3">
                    <label class="field-label" for="<%= txtPersonCode.ClientID %>">รหัสพนักงาน</label>
                    <asp:TextBox ID="txtPersonCode" runat="server" CssClass="field-control" MaxLength="50" />
                </div>

                <div class="mb-4">
                    <label class="field-label" for="<%= txtPassword.ClientID %>">รหัสผ่าน</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="field-control" TextMode="Password" MaxLength="100" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="เข้าสู่ระบบ" CssClass="login-button" OnClick="btnLogin_Click" />
            </section>
        </main>
    </form>
</body>
</html>
