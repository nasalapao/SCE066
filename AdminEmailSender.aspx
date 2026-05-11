<%@ Page Title="Email Sender - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminEmailSender.aspx.cs" Inherits="AdminEmailSender" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Email Sender - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .sender-shell {
            max-width: 1120px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .sender-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 24px;
        }

        .sender-title {
            margin: 0 0 18px;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .sender-section {
            margin-top: 18px;
            padding-top: 18px;
            border-top: 1px solid #e8eef5;
        }

        .field-label {
            display: block;
            margin-bottom: 6px;
            font-size: 0.84rem;
            font-weight: 700;
        }

        .field-control {
            width: 100%;
            min-height: 40px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            padding: 8px 10px;
            font-size: 0.9rem;
        }

        .readonly-value {
            min-height: 40px;
            border: 1px solid #e1e8f0;
            border-radius: 8px;
            background: #f7f9fb;
            padding: 9px 10px;
            color: #334155;
            font-size: 0.9rem;
        }

        .action-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 16px;
        }

        .btn-main,
        .btn-soft {
            min-height: 40px;
            border-radius: 8px;
            padding: 8px 14px;
            font-weight: 700;
        }

        .btn-main {
            border: 0;
            background: #1f4e79;
            color: #fff;
        }

        .btn-soft {
            border: 1px solid #c8d4df;
            background: #fff;
            color: #1f4e79;
        }

        .message-error,
        .message-success {
            display: block;
            margin-bottom: 16px;
            padding: 10px 12px;
            border-radius: 8px;
            font-size: 0.88rem;
        }

        .message-error {
            background: #fff3f3;
            border: 1px solid #ffd4d4;
            color: #a61b1b;
        }

        .message-success {
            background: #f0fff4;
            border: 1px solid #bfe8c7;
            color: #1d6b34;
        }

        .howto-list {
            margin: 0;
            padding-left: 22px;
            color: #334155;
            font-size: 0.92rem;
            line-height: 1.75;
        }

        .link-row {
            margin-top: 10px;
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 0.88rem;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sender-shell">
        <section class="sender-panel">
            <h1 class="sender-title">Customer Email Sender</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />

            <asp:HiddenField ID="hdSenderCode" runat="server" Value="CUSTOMER_INVOICE" />

            <div class="row g-3">
                <div class="col-12 col-md-6">
                    <label class="field-label" for="<%= txtSenderEmail.ClientID %>">Sender Email</label>
                    <asp:TextBox ID="txtSenderEmail" runat="server" CssClass="field-control" MaxLength="150" />
                </div>
                <div class="col-12 col-md-6">
                    <label class="field-label" for="<%= txtAppPassword.ClientID %>">App Password</label>
                    <asp:TextBox ID="txtAppPassword" runat="server" CssClass="field-control" MaxLength="300" />
                </div>
                <div class="col-12 col-md-6">
                    <label class="field-label" for="<%= txtTestToEmail.ClientID %>">Test TO</label>
                    <asp:TextBox ID="txtTestToEmail" runat="server" CssClass="field-control" MaxLength="150" />
                </div>
                <div class="col-12">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                    <label for="<%= chkActive.ClientID %>">Active</label>
                </div>
            </div>

            <div class="sender-section">
                <h2 class="sender-title">SMTP Settings</h2>
                <div class="row g-3">
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="field-label">Host</div>
                        <div class="readonly-value">smtp.office365.com</div>
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="field-label">Port</div>
                        <div class="readonly-value">587</div>
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="field-label">SSL</div>
                        <div class="readonly-value">true</div>
                    </div>
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="field-label">TargetName</div>
                        <div class="readonly-value">STARTTLS/smtp.office365.com</div>
                    </div>
                </div>
            </div>

            <div class="sender-section">
                <h2 class="sender-title">How to create App Password</h2>
                <ol class="howto-list">
                    <li>Open <a href="https://mysignins.microsoft.com/security-info" target="_blank" rel="noopener">https://mysignins.microsoft.com/security-info</a></li>
                    <li>Select <strong>Add sign-in method</strong>, then choose <strong>App password</strong>.</li>
                    <li>Copy the generated password and paste it into <strong>App Password</strong>.</li>
                </ol>
                <div class="link-row">
                    <a href="https://support.microsoft.com/en-us/account-billing/app-passwords-for-a-work-or-school-account-d6dc8c6d-4bf7-4851-ad95-6d07799387e9" target="_blank" rel="noopener">Microsoft: App passwords</a>
                    <a href="https://learn.microsoft.com/en-us/entra/identity/authentication/howto-mfa-app-passwords" target="_blank" rel="noopener">Microsoft Entra MFA app passwords</a>
                </div>
            </div>

            <div class="action-row">
                <asp:Button ID="btnSave" runat="server" Text="Save Sender" CssClass="btn-main" OnClick="btnSave_Click" />
                <asp:Button ID="btnTest" runat="server" Text="Test Send" CssClass="btn-soft" OnClick="btnTest_Click" CausesValidation="false" />
                <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="btn-soft" OnClick="btnReload_Click" CausesValidation="false" />
            </div>
        </section>
    </div>
</asp:Content>
