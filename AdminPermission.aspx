<%@ Page Title="Permission Management - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminPermission.aspx.cs" Inherits="AdminPermission" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Permission Management - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .permission-shell {
            max-width: 1320px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .permission-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 24px;
        }

        .permission-title {
            margin: 0 0 18px;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .permission-workspace {
            display: grid;
            grid-template-columns: minmax(300px, 380px) minmax(0, 1fr);
            gap: 18px;
            align-items: start;
        }

        .employee-panel,
        .editor-panel {
            border: 1px solid #e1e8f0;
            border-radius: 8px;
            background: #f8fafc;
            padding: 16px;
        }

        .editor-panel {
            background: #fff;
        }

        .section-title {
            margin: 0 0 12px;
            color: #1f2933;
            font-size: 1rem;
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

        .action-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            align-items: flex-end;
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

        .employee-summary {
            margin-top: 14px;
            border: 1px solid #dbe7f1;
            border-radius: 8px;
            background: #fff;
            padding: 12px;
        }

        .employee-name {
            display: block;
            color: #1f2933;
            font-size: 1rem;
            font-weight: 700;
            line-height: 1.45;
        }

        .employee-code {
            display: block;
            margin-top: 4px;
            color: #475569;
            font-size: 0.88rem;
        }

        .employee-group {
            display: inline-flex;
            margin-top: 10px;
            border: 1px solid #c8d4df;
            border-radius: 999px;
            background: #fff;
            color: #1f4e79;
            padding: 4px 10px;
            font-size: 0.82rem;
            font-weight: 700;
        }

        .grid-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .employee-grid,
        .permission-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.88rem;
        }

        .employee-grid th,
        .permission-table th {
            background: #eef3f8;
            color: #1f2933;
            font-weight: 700;
            white-space: nowrap;
        }

        .employee-grid th,
        .employee-grid td,
        .permission-table th,
        .permission-table td {
            border: 1px solid #dde7f0;
            padding: 8px 10px;
            vertical-align: middle;
        }

        .employee-grid td:nth-child(3),
        .employee-grid th:nth-child(3) {
            width: 130px;
            text-align: center;
        }

        .permission-table td:nth-child(1),
        .permission-table th:nth-child(1) {
            width: 76px;
            text-align: center;
        }

        .permission-page-code {
            color: #64748b;
            font-size: 0.8rem;
        }

        .permission-table-wrap {
            margin-top: 18px;
        }

        @media (max-width: 900px) {
            .permission-workspace {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="permission-shell">
        <section class="permission-panel">
            <h1 class="permission-title">Permission Management</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />
            <asp:HiddenField ID="hdMode" runat="server" />
            <asp:HiddenField ID="hdSelectedPersonCode" runat="server" />
            <asp:HiddenField ID="hdPermissionGroup" runat="server" />

            <div class="permission-workspace">
                <aside class="employee-panel">
                    <h2 class="section-title">Employees</h2>
                    <div class="action-row">
                        <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn-main" OnClick="btnNew_Click" CausesValidation="false" />
                    </div>

                    <div class="grid-wrap" style="margin-top: 12px;">
                        <asp:GridView ID="gvEmployees" runat="server"
                            AutoGenerateColumns="false"
                            DataKeyNames="PersonCode"
                            CssClass="employee-grid"
                            GridLines="None"
                            OnRowCommand="gvEmployees_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="DisplayName" HeaderText="Employee" />
                                <asp:BoundField DataField="PermissionGroup" HeaderText="Group" />
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Button ID="btnSelectEmployee" runat="server"
                                            Text="Select"
                                            CssClass="btn-soft"
                                            CommandName="SelectEmployee"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CausesValidation="false" />
                                        <asp:Button ID="btnEditEmployee" runat="server"
                                            Text="Edit"
                                            CssClass="btn-soft"
                                            CommandName="EditEmployee"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <span class="permission-page-code">No permission records.</span>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </aside>

                <section class="editor-panel">
                    <h2 class="section-title">Employee Form</h2>
                    <div class="row g-3">
                        <div class="col-12 col-md-4">
                            <label class="field-label" for="<%= txtPersonCode.ClientID %>">Person Code</label>
                            <asp:TextBox ID="txtPersonCode" runat="server" CssClass="field-control" MaxLength="20" />
                        </div>
                        <div class="col-12 col-md-4">
                            <label class="field-label" for="<%= ddlPermissionGroup.ClientID %>">Group</label>
                            <asp:DropDownList ID="ddlPermissionGroup" runat="server" CssClass="field-control" />
                        </div>
                        <div class="col-12 col-md-4">
                            <label class="field-label" for="<%= txtManualPermissionGroup.ClientID %>">Manual Group</label>
                            <asp:TextBox ID="txtManualPermissionGroup" runat="server" CssClass="field-control" MaxLength="20" />
                        </div>
                        <div class="col-12">
                            <div class="action-row">
                                <asp:Button ID="btnInsertEmployee" runat="server" Text="Insert" CssClass="btn-main" OnClick="btnInsertEmployee_Click" CausesValidation="false" />
                                <asp:Button ID="btnUpdateEmployee" runat="server" Text="Update" CssClass="btn-soft" OnClick="btnUpdateEmployee_Click" CausesValidation="false" />
                                <asp:Button ID="btnDeleteEmployee" runat="server" Text="Delete" CssClass="btn-danger-soft" OnClick="btnDeleteEmployee_Click" CausesValidation="false" OnClientClick="return confirm('Delete all permission records for this employee?');" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-soft" OnClick="btnCancel_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>

                    <div class="employee-summary">
                        <asp:Literal ID="litEmployeeName" runat="server" />
                        <asp:Literal ID="litEmployeeCode" runat="server" />
                        <asp:Literal ID="litPermissionGroup" runat="server" />
                    </div>

                    <asp:Panel ID="pnlPermissions" runat="server" CssClass="permission-table-wrap" Visible="false">
                        <h2 class="section-title">Page Permissions</h2>
                        <div class="grid-wrap">
                            <asp:GridView ID="gvPermissions" runat="server"
                                AutoGenerateColumns="false"
                                DataKeyNames="PageCode"
                                CssClass="permission-table"
                                GridLines="None">
                                <Columns>
                                    <asp:TemplateField HeaderText="Allow">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkActive" runat="server" Checked='<%# Eval("IsActive") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="PageName" HeaderText="Program Page" />
                                    <asp:BoundField DataField="GroupName" HeaderText="Group" />
                                    <asp:TemplateField HeaderText="Page Code">
                                        <ItemTemplate>
                                            <span class="permission-page-code"><%# Eval("PageCode") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="action-row" style="margin-top: 12px;">
                            <asp:Button ID="btnSavePermission" runat="server" Text="Save Permission" CssClass="btn-main" OnClick="btnSavePermission_Click" />
                        </div>
                    </asp:Panel>
                </section>
            </div>
        </section>
    </div>
</asp:Content>
