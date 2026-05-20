<%@ Page Title="Program Update - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminProgramUpdate.aspx.cs" Inherits="AdminProgramUpdate" CodePage="65001" ValidateRequest="false" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Program Update - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .program-update-shell {
            max-width: 1120px;
            margin: 0 auto;
            padding: 0 16px;
        }

        .program-update-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 8px;
            box-shadow: 0 10px 24px rgba(15, 35, 55, 0.07);
            padding: 24px;
        }

        .program-update-title {
            margin: 0 0 18px;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .program-update-section {
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

        .detail-control {
            min-height: 130px;
            line-height: 1.6;
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

        .grid-wrap {
            width: 100%;
            overflow-x: auto;
        }

        .program-update-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.86rem;
        }

        .program-update-grid th {
            background: #f3f6fa;
            color: #1f2933;
            font-weight: 700;
            white-space: nowrap;
        }

        .program-update-grid th,
        .program-update-grid td {
            border: 1px solid #e1e8f0;
            padding: 8px 10px;
            vertical-align: top;
        }

        .program-update-grid td {
            max-width: 320px;
            overflow-wrap: anywhere;
        }

        .grid-action-row {
            display: flex;
            align-items: center;
            gap: 8px;
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="program-update-shell">
        <section class="program-update-panel">
            <h1 class="program-update-title">Program Update</h1>

            <asp:Label ID="lbError" runat="server" CssClass="message-error" Visible="false" />
            <asp:Label ID="lbSuccess" runat="server" CssClass="message-success" Visible="false" />

            <asp:HiddenField ID="hdLogId" runat="server" />
            <asp:HiddenField ID="hdMode" runat="server" />

            <div class="row g-3">
                <div class="col-12 col-md-4">
                    <label class="field-label" for="<%= txtChangeDate.ClientID %>">วันที่อัปเดต</label>
                    <asp:TextBox ID="txtChangeDate" runat="server" CssClass="field-control" MaxLength="10" />
                </div>
                <div class="col-12 col-md-8">
                    <label class="field-label" for="<%= txtChangeTitle.ClientID %>">หัวข้อ</label>
                    <asp:TextBox ID="txtChangeTitle" runat="server" CssClass="field-control" MaxLength="200" />
                </div>
                <div class="col-12">
                    <label class="field-label" for="<%= txtChangeDetail.ClientID %>">รายละเอียด</label>
                    <asp:TextBox ID="txtChangeDetail" runat="server" CssClass="field-control detail-control" TextMode="MultiLine" />
                </div>
                <div class="col-12">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                    <label for="<%= chkActive.ClientID %>">Active</label>
                </div>
            </div>

            <div class="action-row">
                <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn-soft" OnClick="btnNew_Click" CausesValidation="false" />
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-main" OnClick="btnSave_Click" />
                <asp:Button ID="btnReload" runat="server" Text="Reload" CssClass="btn-soft" OnClick="btnReload_Click" CausesValidation="false" />
            </div>

            <div class="program-update-section">
                <h2 class="program-update-title">Update List</h2>
                <div class="grid-wrap">
                    <asp:GridView ID="gvProgramUpdate" runat="server" AutoGenerateColumns="false" CssClass="program-update-grid"
                        DataKeyNames="LOG_ID,CHANGE_DATE,CHANGE_TITLE,CHANGE_DETAIL,ACTIVE_STATUS,UPDATED_USER,UPDATED_DATE"
                        OnRowCommand="gvProgramUpdate_RowCommand" EmptyDataText="No program update data.">
                        <Columns>
                            <asp:BoundField HeaderText="วันที่อัปเดต" DataField="CHANGE_DATE" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                            <asp:BoundField HeaderText="หัวข้อ" DataField="CHANGE_TITLE" />
                            <asp:BoundField HeaderText="รายละเอียด" DataField="CHANGE_DETAIL" />
                            <asp:BoundField HeaderText="สถานะ" DataField="ACTIVE_STATUS" />
                            <asp:BoundField HeaderText="แก้ไขล่าสุดโดย" DataField="UPDATED_USER" />
                            <asp:BoundField HeaderText="แก้ไขล่าสุดเมื่อ" DataField="UPDATED_DATE" DataFormatString="{0:dd/MM/yyyy HH:mm}" HtmlEncode="false" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <div class="grid-action-row">
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-soft" CommandName="EditLog" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false" />
                                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-soft" CommandName="DeleteLog" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false" OnClientClick="return confirm('Delete this program update permanently?');" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
