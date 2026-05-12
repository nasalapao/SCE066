<%@ Page Title="Invoice Auto Reminder - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="InvoiceAutoReminder.aspx.cs" Inherits="InvoiceAutoReminder" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Invoice Auto Reminder - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .work-shell {
            max-width: 1480px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .work-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 22px;
        }

        .work-title {
            margin: 0 0 16px;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .work-section {
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

        .action-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .btn-main {
            min-height: 40px;
            border: 0;
            border-radius: 8px;
            padding: 8px 14px;
            background: #1f4e79;
            color: #fff;
            font-weight: 700;
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

        .grid-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .mail-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.86rem;
        }

        .mail-grid th {
            background: #f3f6fa;
            color: #1f2933;
            font-weight: 700;
            white-space: nowrap;
        }

        .mail-grid th,
        .mail-grid td {
            border: 1px solid #e1e8f0;
            padding: 8px 10px;
            vertical-align: middle;
        }

        .file-link {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 34px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            padding: 6px 12px;
            color: #1f4e79;
            font-weight: 700;
            text-decoration: none;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="work-shell">
        <section class="work-panel">
            <h1 class="work-title">Invoice Auto Reminder</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />
            <asp:Literal ID="litCloseScript" runat="server" />

            <div class="row g-3">
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= txtDays.ClientID %>">Days</label>
                    <asp:TextBox ID="txtDays" runat="server" CssClass="field-control" MaxLength="3" />
                </div>
                <div class="col-12 col-md-3">
                    <label class="field-label">&nbsp;</label>
                    <div class="action-row">
                        <asp:Button ID="btnRun" runat="server" Text="Run Reminder" CssClass="btn-main" OnClick="btnRun_Click" />
                    </div>
                </div>
            </div>

            <div class="work-section">
                <div class="grid-wrap">
                    <asp:GridView ID="gvInvoice" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="mail-grid"
                        EmptyDataText="ไม่พบข้อมูล">
                        <Columns>
                            <asp:BoundField DataField="SHIVNO" HeaderText="Invoice" />
                            <asp:BoundField DataField="CUSTOMER_DISPLAY" HeaderText="Customer" />
                            <asp:BoundField DataField="INVOICE_UPLOAD_DATE_DISPLAY" HeaderText="INV Upload Date" />
                            <asp:BoundField DataField="OVERDUE_DAYS" HeaderText="Overdue Days" />
                            <asp:TemplateField HeaderText="Link">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkManage" runat="server" Text="Open" CssClass="file-link"
                                        NavigateUrl='<%# Eval("MANAGE_URL") %>' Target="_blank" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
