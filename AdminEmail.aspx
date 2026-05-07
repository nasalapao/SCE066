<%@ Page Title="Email Management - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminEmail.aspx.cs" Inherits="AdminEmail" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Email Management - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .admin-shell {
            max-width: 1440px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .admin-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 12px;
            box-shadow: 0 12px 30px rgba(15, 35, 55, 0.08);
            padding: 24px;
        }

        .admin-title {
            margin: 0 0 18px;
            font-size: 1.45rem;
            font-weight: 700;
        }

        .admin-section {
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

        .field-control:focus {
            outline: none;
            border-color: rgba(31, 78, 121, 0.55);
            box-shadow: 0 0 0 0.15rem rgba(31, 78, 121, 0.12);
        }

        .checkbox-row {
            display: flex;
            align-items: center;
            gap: 8px;
            min-height: 40px;
            font-size: 0.9rem;
        }

        .action-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .btn-main,
        .btn-soft,
        .btn-danger-soft {
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

        .btn-main:hover {
            background: #173a59;
        }

        .btn-soft {
            border: 1px solid #c8d4df;
            background: #fff;
            color: #1f4e79;
        }

        .btn-danger-soft {
            border: 1px solid #f1c0c0;
            background: #fff7f7;
            color: #a61b1b;
        }

        .error-message,
        .success-message {
            display: block;
            margin-bottom: 16px;
            padding: 10px 12px;
            border-radius: 8px;
            font-size: 0.88rem;
        }

        .error-message {
            background: #fff3f3;
            border: 1px solid #ffd4d4;
            color: #a61b1b;
        }

        .success-message {
            background: #f0fff4;
            border: 1px solid #bfe8c7;
            color: #1d6b34;
        }

        .grid-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .email-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.86rem;
        }

        .email-grid th {
            background: #f3f6fa;
            color: #1f2933;
            font-weight: 700;
            white-space: nowrap;
        }

        .email-grid th,
        .email-grid td {
            border: 1px solid #e1e8f0;
            padding: 8px 10px;
            vertical-align: middle;
        }

        .email-grid td:nth-child(4) {
            min-width: 360px;
        }

        .email-grid th:nth-child(6),
        .email-grid th:nth-child(7),
        .email-grid td:nth-child(6),
        .email-grid td:nth-child(7) {
            min-width: 150px;
        }

        .email-grid th:nth-child(8),
        .email-grid td:nth-child(8) {
            min-width: 170px;
            white-space: nowrap;
        }

        .email-grid td:nth-child(8) .btn-soft,
        .email-grid td:nth-child(8) .btn-danger-soft {
            display: inline-block;
            margin-right: 6px;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-shell">
        <section class="admin-panel">
            <h1 class="admin-title">Email Management</h1>

            <asp:Label ID="lbError" runat="server" CssClass="error-message" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="success-message" Visible="false" />

            <div class="row g-3">
                <div class="col-12 col-md-4">
                    <label class="field-label" for="<%= txtSearchCustomer.ClientID %>">Customer Code</label>
                    <asp:TextBox ID="txtSearchCustomer" runat="server" CssClass="field-control" MaxLength="10" />
                </div>
                <div class="col-12 col-md-3">
                    <label class="field-label">&nbsp;</label>
                    <div class="checkbox-row">
                        <asp:CheckBox ID="chkShowInactive" runat="server" />
                        <label for="<%= chkShowInactive.ClientID %>">Show inactive</label>
                    </div>
                </div>
                <div class="col-12 col-md-5">
                    <label class="field-label">&nbsp;</label>
                    <div class="action-row">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-main" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnResetSearch" runat="server" Text="Reset" CssClass="btn-soft" OnClick="btnResetSearch_Click" CausesValidation="false" />
                        <asp:Button ID="btnShowAdd" runat="server" Text="New Email" CssClass="btn-soft" OnClick="btnShowAdd_Click" CausesValidation="false" />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlForm" runat="server" CssClass="admin-section">
                <asp:HiddenField ID="hdMode" runat="server" />
                <asp:HiddenField ID="hdCustomerCode" runat="server" />
                <asp:HiddenField ID="hdRecipientType" runat="server" />
                <asp:HiddenField ID="hdEmailSeq" runat="server" />

                <div class="row g-3">
                    <div class="col-12 col-md-3">
                        <label class="field-label" for="<%= txtCustomerCode.ClientID %>">Customer Code</label>
                        <asp:TextBox ID="txtCustomerCode" runat="server" CssClass="field-control" MaxLength="10" />
                    </div>
                    <div class="col-12 col-md-2">
                        <label class="field-label" for="<%= ddlRecipientType.ClientID %>">Recipient Type</label>
                        <asp:DropDownList ID="ddlRecipientType" runat="server" CssClass="field-control">
                            <asp:ListItem Text="TO" Value="TO" />
                            <asp:ListItem Text="CC" Value="CC" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-4">
                        <label class="field-label" for="<%= txtEmailAddress.ClientID %>">Email Address</label>
                        <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="field-control" MaxLength="254" />
                    </div>
                    <div class="col-12 col-md-3">
                        <label class="field-label">&nbsp;</label>
                        <div class="checkbox-row">
                            <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                            <label for="<%= chkActive.ClientID %>">Active</label>
                        </div>
                    </div>
                </div>

                <div class="action-row mt-3">
                    <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="btn-main" OnClick="btnAdd_Click" />
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn-main" OnClick="btnUpdate_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-soft" OnClick="btnClear_Click" CausesValidation="false" />
                </div>
            </asp:Panel>

            <div class="admin-section">
                <div class="grid-wrap">
                    <asp:GridView ID="gvEmail" runat="server"
                        AutoGenerateColumns="false"
                        CssClass="email-grid"
                        DataKeyNames="CUSTOMER_CODE,RECIPIENT_TYPE,EMAIL_SEQ,EMAIL_ADDRESS,ACTIVE_STATUS"
                        EmptyDataText="ไม่พบข้อมูล"
                        OnRowCommand="gvEmail_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="CUSTOMER_CODE" HeaderText="Customer" />
                            <asp:BoundField DataField="RECIPIENT_TYPE" HeaderText="Type" />
                            <asp:BoundField DataField="EMAIL_SEQ" HeaderText="Seq" />
                            <asp:BoundField DataField="EMAIL_ADDRESS" HeaderText="Email" />
                            <asp:BoundField DataField="ACTIVE_STATUS" HeaderText="Active" />
                            <asp:BoundField DataField="CREATED_DATE" HeaderText="Created" />
                            <asp:BoundField DataField="UPDATED_DATE" HeaderText="Updated" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnEditRow" runat="server" Text="Edit" CssClass="btn-soft"
                                        CommandName="EditEmail" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false" />
                                    <asp:Button ID="btnDeleteRow" runat="server" Text="Delete" CssClass="btn-danger-soft"
                                        CommandName="SoftDeleteEmail" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false"
                                        OnClientClick="return confirm('ยืนยันปิดใช้งาน email นี้?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
