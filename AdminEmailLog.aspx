<%@ Page Title="Email Log - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminEmailLog.aspx.cs" Inherits="AdminEmailLog" CodePage="65001" ValidateRequest="false" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Email Log - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .log-shell {
            max-width: 1240px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .log-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 24px;
        }

        .log-title {
            margin: 0 0 18px;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .log-section {
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

        .message-error {
            display: block;
            margin-bottom: 16px;
            padding: 10px 12px;
            border-radius: 8px;
            font-size: 0.88rem;
            background: #fff3f3;
            border: 1px solid #ffd4d4;
            color: #a61b1b;
        }

        .hint-text {
            margin-top: 8px;
            color: #5b6b7a;
            font-size: 0.84rem;
        }

        .grid-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .log-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.84rem;
        }

        .log-grid th {
            background: #f3f6fa;
            color: #1f2933;
            font-weight: 700;
            white-space: nowrap;
        }

        .log-grid th,
        .log-grid td {
            border: 1px solid #e1e8f0;
            padding: 8px 10px;
            vertical-align: top;
        }

        .log-grid td {
            max-width: 260px;
            overflow-wrap: anywhere;
        }

        .detail-grid {
            display: grid;
            grid-template-columns: 120px minmax(0, 1fr);
            gap: 8px 14px;
            font-size: 0.9rem;
        }

        .detail-label {
            color: #5b6b7a;
            font-weight: 700;
        }

        .detail-value {
            color: #1f2933;
            overflow-wrap: anywhere;
        }

        .body-preview {
            width: 100%;
            min-height: 220px;
            height: 420px;
            border: 1px solid #e1e8f0;
            border-radius: 8px;
            background: #fff;
        }

        .empty-text {
            color: #5b6b7a;
            font-size: 0.9rem;
        }

        .modal-overlay {
            position: fixed;
            inset: 0;
            display: none;
            align-items: center;
            justify-content: center;
            padding: 24px;
            background: rgba(15, 35, 55, 0.45);
            z-index: 1000;
        }

        .modal-overlay.is-open {
            display: flex;
        }

        .email-log-modal-dialog {
            width: min(980px, 100%);
            height: calc(100vh - 48px);
            max-height: calc(100vh - 48px);
            display: flex;
            flex-direction: column;
            overflow: hidden;
            pointer-events: auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 24px 70px rgba(15, 35, 55, 0.28);
            padding: 24px;
        }

        .modal-header-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 18px;
        }

        .modal-close {
            width: 40px;
            height: 40px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            background: #fff;
            color: #1f4e79;
            font-size: 1.4rem;
            line-height: 1;
            font-weight: 700;
        }

        .modal-close:hover,
        .modal-close:focus {
            border-color: #1f4e79;
            color: #173a59;
        }

        .modal-body-area {
            min-height: 0;
            display: flex;
            flex-direction: column;
            gap: 18px;
            flex: 1;
        }

        .modal-detail-wrap {
            flex: 0 0 auto;
        }

        .modal-preview-wrap {
            min-height: 0;
            display: flex;
            flex-direction: column;
            flex: 1;
            padding-top: 18px;
            border-top: 1px solid #e8eef5;
        }

        .modal-preview-wrap .body-preview {
            flex: 1;
            min-height: 0;
            height: auto;
        }

        @media (max-width: 767.98px) {
            .detail-grid {
                grid-template-columns: 1fr;
                gap: 4px 0;
            }

            .modal-overlay {
                padding: 12px;
            }

            .email-log-modal-dialog {
                height: calc(100vh - 24px);
                max-height: calc(100vh - 24px);
                padding: 18px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="log-shell">
        <section class="log-panel">
            <h1 class="log-title">Email Log</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />

            <div class="row g-3">
                <div class="col-12 col-lg-4">
                    <label class="field-label" for="<%= txtKeyword.ClientID %>">Keyword</label>
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="field-control" MaxLength="200" />
                </div>
                <div class="col-12 col-md-6 col-lg-2">
                    <label class="field-label" for="<%= txtDateFrom.ClientID %>">Date From</label>
                    <asp:TextBox ID="txtDateFrom" runat="server" CssClass="field-control" TextMode="Date" />
                </div>
                <div class="col-12 col-md-6 col-lg-2">
                    <label class="field-label" for="<%= txtDateTo.ClientID %>">Date To</label>
                    <asp:TextBox ID="txtDateTo" runat="server" CssClass="field-control" TextMode="Date" />
                </div>
                <div class="col-12 col-md-6 col-lg-2">
                    <label class="field-label" for="<%= ddlStatus.ClientID %>">Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="field-control">
                        <asp:ListItem Text="All" Value="" />
                        <asp:ListItem Text="SUCCESS" Value="SUCCESS" />
                        <asp:ListItem Text="FAILED" Value="FAILED" />
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-6 col-lg-2">
                    <label class="field-label" for="<%= txtMailKind.ClientID %>">Mail Kind</label>
                    <asp:TextBox ID="txtMailKind" runat="server" CssClass="field-control" MaxLength="20" />
                </div>
            </div>

            <div class="action-row">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-main" OnClick="btnSearch_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-soft" OnClick="btnReset_Click" CausesValidation="false" />
            </div>
            <div class="hint-text">Showing latest 200 logs by sent timestamp.</div>

            <div class="log-section">
                <div class="grid-wrap">
                    <asp:GridView ID="gvEmailLog" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="log-grid"
                        DataKeyNames="LOG_ID"
                        EmptyDataText="No email log found"
                        OnRowCommand="gvEmailLog_RowCommand">
                        <Columns>
                            <asp:ButtonField Text="View" CommandName="ViewLog" ButtonType="Button" ControlStyle-CssClass="btn-soft" />
                            <asp:BoundField DataField="LOG_ID" HeaderText="Log ID" />
                            <asp:BoundField DataField="SENT_TIMESTAMP" HeaderText="Sent Time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                            <asp:BoundField DataField="MAIL_KIND" HeaderText="Kind" />
                            <asp:BoundField DataField="STATUS" HeaderText="Status" />
                            <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice" />
                            <asp:BoundField DataField="CUSTOMER_CODE" HeaderText="Customer" />
                            <asp:BoundField DataField="SUBJECT_TEXT" HeaderText="Subject" />
                            <asp:BoundField DataField="SENT_USER" HeaderText="User" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <asp:Panel ID="pnlDetail" runat="server" CssClass="modal-overlay" Style="display:none;" aria-hidden="true">
                <div class="email-log-modal-dialog" role="dialog" aria-modal="true" aria-labelledby="emailLogModalTitle">
                    <div class="modal-header-row">
                        <h2 id="emailLogModalTitle" class="log-title">Email Preview</h2>
                        <button type="button" class="modal-close" aria-label="Close email preview" onclick="closeEmailLogModal();">&times;</button>
                    </div>

                    <div class="modal-body-area">
                        <div class="modal-detail-wrap">
                            <div class="detail-grid">
                                <div class="detail-label">Log ID</div>
                                <div class="detail-value"><asp:Literal ID="litLogId" runat="server" /></div>
                                <div class="detail-label">Sent Time</div>
                                <div class="detail-value"><asp:Literal ID="litSentTime" runat="server" /></div>
                                <div class="detail-label">Kind</div>
                                <div class="detail-value"><asp:Literal ID="litMailKind" runat="server" /></div>
                                <div class="detail-label">Status</div>
                                <div class="detail-value"><asp:Literal ID="litStatus" runat="server" /></div>
                                <div class="detail-label">Invoice</div>
                                <div class="detail-value"><asp:Literal ID="litInvoiceNo" runat="server" /></div>
                                <div class="detail-label">Customer</div>
                                <div class="detail-value"><asp:Literal ID="litCustomerCode" runat="server" /></div>
                                <div class="detail-label">To</div>
                                <div class="detail-value"><asp:Literal ID="litToRecipients" runat="server" /></div>
                                <div class="detail-label">Cc</div>
                                <div class="detail-value"><asp:Literal ID="litCcRecipients" runat="server" /></div>
                                <div class="detail-label">Subject</div>
                                <div class="detail-value"><asp:Literal ID="litSubject" runat="server" /></div>
                                <div class="detail-label">Sent User</div>
                                <div class="detail-value"><asp:Literal ID="litSentUser" runat="server" /></div>
                                <div class="detail-label">Error</div>
                                <div class="detail-value"><asp:Literal ID="litErrorMessage" runat="server" /></div>
                            </div>
                        </div>

                        <div class="modal-preview-wrap">
                            <div class="field-label">Body</div>
                            <iframe id="frameBody" runat="server" class="body-preview" sandbox=""></iframe>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </section>
    </div>

    <script type="text/javascript">
        function getEmailLogModal() {
            return document.getElementById('<%= pnlDetail.ClientID %>');
        }

        function openEmailLogModal() {
            var modal = getEmailLogModal();
            if (!modal) {
                return;
            }

            modal.classList.add('is-open');
            modal.style.display = 'flex';
            modal.setAttribute('aria-hidden', 'false');
            document.body.style.overflow = 'hidden';
        }

        function closeEmailLogModal() {
            var modal = getEmailLogModal();
            if (!modal) {
                return;
            }

            modal.classList.remove('is-open');
            modal.style.display = 'none';
            modal.setAttribute('aria-hidden', 'true');
            document.body.style.overflow = '';
        }

        document.addEventListener('click', function (event) {
            var modal = getEmailLogModal();
            if (modal && event.target === modal) {
                closeEmailLogModal();
            }
        });

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                closeEmailLogModal();
            }
        });
    </script>
</asp:Content>
