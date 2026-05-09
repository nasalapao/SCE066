<%@ Page Title="Customer Email Send - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CustomerEmailSend.aspx.cs" Inherits="CustomerEmailSend" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Customer Email Send - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />
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

        .checkbox-row {
            display: flex;
            align-items: center;
            gap: 8px;
            min-height: 40px;
            font-size: 0.9rem;
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

        .mail-grid td:nth-child(2) {
            min-width: 260px;
        }

        .mail-grid td:last-child {
            min-width: 120px;
            white-space: nowrap;
        }
    </style>
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#<%= txtDateFrom.ClientID %>').datepicker({ dateFormat: 'dd/mm/yy' });
            $('#<%= txtDateTo.ClientID %>').datepicker({ dateFormat: 'dd/mm/yy' });
        });
    </script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="work-shell">
        <section class="work-panel">
            <h1 class="work-title">Customer Email Send</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />

            <div class="row g-3">
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= txtInvoiceNo.ClientID %>">Invoice No</label>
                    <asp:TextBox ID="txtInvoiceNo" runat="server" CssClass="field-control" MaxLength="20" />
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= txtCustomerCode.ClientID %>">Customer Code</label>
                    <asp:TextBox ID="txtCustomerCode" runat="server" CssClass="field-control" MaxLength="10" />
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= txtDateFrom.ClientID %>">Date From</label>
                    <asp:TextBox ID="txtDateFrom" runat="server" CssClass="field-control" MaxLength="10" autocomplete="off" />
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= txtDateTo.ClientID %>">Date To</label>
                    <asp:TextBox ID="txtDateTo" runat="server" CssClass="field-control" MaxLength="10" autocomplete="off" />
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label" for="<%= ddlStatus.ClientID %>">Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="field-control">
                        <asp:ListItem Text="Pending" Value="PENDING" />
                        <asp:ListItem Text="Sent" Value="SENT" />
                        <asp:ListItem Text="All" Value="ALL" />
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label">&nbsp;</label>
                    <div class="checkbox-row">
                        <asp:CheckBox ID="chkHasCustomerEmail" runat="server" Checked="true" />
                        <label for="<%= chkHasCustomerEmail.ClientID %>">Has customer email</label>
                    </div>
                </div>
                <div class="col-12 col-md-2">
                    <label class="field-label">&nbsp;</label>
                    <div class="action-row">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-main" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-soft" OnClick="btnReset_Click" CausesValidation="false" />
                    </div>
                </div>
            </div>

            <div class="work-section">
                <div class="grid-wrap">
                    <asp:GridView ID="gvInvoice" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="mail-grid"
                        DataKeyNames="SHIVNO,SHCUNO,CUSTOMER_NAME,SHMLCT"
                        EmptyDataText="ไม่พบข้อมูล"
                        OnRowCommand="gvInvoice_RowCommand"
                        OnRowDataBound="gvInvoice_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="SHIVNO" HeaderText="Invoice" />
                            <asp:BoundField DataField="CUSTOMER_DISPLAY" HeaderText="Customer" />
                            <asp:BoundField DataField="INVOICE_UPLOAD_DATE" HeaderText="INV Upload Date" />
                            <asp:TemplateField HeaderText="File">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkViewFile" runat="server" Text="View" CssClass="file-link"
                                        NavigateUrl='<%# GetInvoiceFileUrl(Eval("SHIVNO")) %>' Target="_blank" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="MAIL_STATUS_TEXT" HeaderText="Mail Status" />
                            <asp:BoundField DataField="SHMLTS" HeaderText="Last Sent" />
                            <asp:BoundField DataField="SHMLUS_DISPLAY" HeaderText="Last User" />
                            <asp:BoundField DataField="SHMLCT" HeaderText="Sent Count" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnSendMail" runat="server" Text="SendMail" CssClass="btn-main"
                                        CommandName="SendCustomerMail" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
