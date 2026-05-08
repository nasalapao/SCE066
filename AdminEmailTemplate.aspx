<%@ Page Title="Email Template - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminEmailTemplate.aspx.cs" Inherits="AdminEmailTemplate" CodePage="65001" ValidateRequest="false" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Email Template - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .admin-shell {
            max-width: 1120px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .admin-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 24px;
        }

        .admin-title {
            margin: 0 0 18px;
            font-size: 1.35rem;
            font-weight: 700;
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

        .body-control {
            min-height: 260px;
            font-family: Consolas, "Courier New", monospace;
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

        .hint-text {
            margin-top: 8px;
            color: #5b6b7a;
            font-size: 0.84rem;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-shell">
        <section class="admin-panel">
            <h1 class="admin-title">Customer Invoice Email Template</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />

            <asp:HiddenField ID="hdTemplateCode" runat="server" Value="CUSTOMER_INVOICE" />

            <div class="row g-3">
                <div class="col-12">
                    <label class="field-label" for="<%= txtSubject.ClientID %>">Subject</label>
                    <asp:TextBox ID="txtSubject" runat="server" CssClass="field-control" MaxLength="250" />
                </div>
                <div class="col-12">
                    <label class="field-label" for="<%= txtBody.ClientID %>">Body</label>
                    <asp:TextBox ID="txtBody" runat="server" CssClass="field-control body-control" TextMode="MultiLine" />
                    <div class="hint-text">Placeholders: {INVOICE_NO}, {CUSTOMER_CODE}, {CUSTOMER_NAME}, {SEND_DATE}</div>
                </div>
                <div class="col-12">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                    <label for="<%= chkActive.ClientID %>">Active</label>
                </div>
            </div>

            <div class="action-row">
                <asp:Button ID="btnSave" runat="server" Text="Save Template" CssClass="btn-main" OnClick="btnSave_Click" />
                <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="btn-soft" OnClick="btnReload_Click" CausesValidation="false" />
            </div>
        </section>
    </div>
</asp:Content>
