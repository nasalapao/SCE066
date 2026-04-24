<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066_1.aspx.cs" Inherits="SCE066_1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Shipping Document Control - Detail PDF</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Thai:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        :root {
            --page-bg: #f3f6fa;
            --surface: #ffffff;
            --surface-strong: #ffffff;
            --primary: #1f4e79;
            --primary-deep: #173a59;
            --accent: #2f6b2f;
            --text-main: #1f2933;
            --text-soft: #5b6b7a;
            --line-soft: #e8eef5;
            --shadow-lg: 0 12px 30px rgba(15, 35, 55, 0.08);
            --shadow-sm: 0 4px 16px rgba(15, 35, 55, 0.05);
        }

        body {
            min-height: 100vh;
            margin: 0;
            font-family: 'Noto Sans Thai', 'Segoe UI', Tahoma, sans-serif;
            color: var(--text-main);
            background: var(--page-bg);
        }

        .page-shell {
            padding: 24px 0 40px;
        }

        .hero-card,
        .content-card {
            background: var(--surface);
            border: 1px solid var(--line-soft);
            border-radius: 14px;
            box-shadow: var(--shadow-lg);
        }

        .hero-card {
            padding: 24px 24px 20px;
        }

        .hero-kicker {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 4px 10px;
            border-radius: 999px;
            background: #eaf1f7;
            color: var(--primary);
            font-size: 0.72rem;
            font-weight: 600;
            letter-spacing: 0.04em;
            text-transform: uppercase;
        }

        .hero-title {
            margin: 14px 0 0;
            font-size: 1.7rem;
            line-height: 1.2;
            font-weight: 700;
            letter-spacing: 0;
        }

        .filter-panel {
            margin-top: 24px;
            padding: 20px;
            border-radius: 12px;
            background: var(--surface-strong);
            box-shadow: var(--shadow-sm);
            border: 1px solid #e6edf4;
        }

        .panel-label {
            display: block;
            margin-bottom: 6px;
            font-size: 0.8rem;
            font-weight: 600;
            color: var(--text-main);
        }

        .field-control,
        input[type="text"].field-control,
        select.field-control,
        input[type="file"].field-control {
            width: 100%;
            min-height: 42px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            background: #fff;
            color: var(--text-main);
            padding: 8px 12px;
            font-size: 0.88rem;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

        .field-control:focus,
        input[type="text"].field-control:focus,
        select.field-control:focus,
        input[type="file"].field-control:focus {
            outline: none;
            border-color: rgba(31, 78, 121, 0.55);
            box-shadow: 0 0 0 0.15rem rgba(31, 78, 121, 0.12);
        }

        input[type="file"].field-control {
            padding: 7px 10px;
        }

        .action-stack {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
            justify-content: flex-end;
            height: 100%;
        }

        .icon-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 46px;
            height: 46px;
            border: 0;
            border-radius: 10px;
            box-shadow: none;
            transition: transform 0.15s ease, box-shadow 0.15s ease;
        }

        .icon-action:hover {
            transform: translateY(-1px);
        }

        .icon-action.action-disabled,
        .icon-action.action-disabled:hover,
        .icon-action.aspNetDisabled,
        .icon-action.aspNetDisabled:hover {
            opacity: 0.45;
            transform: none;
            cursor: not-allowed;
            box-shadow: none;
        }

        .icon-action.upload-action {
            background: #1f4e79;
        }

        .icon-action.back-action {
            background: #5a6f85;
        }

        .icon-action img {
            max-width: 20px;
            max-height: 20px;
        }

        .content-card {
            margin-top: 24px;
            padding: 20px;
        }

        .error-banner {
            margin-top: 16px;
            padding: 12px 14px;
            display: block;
            width: 100%;
            border-radius: 10px;
            border: 1px solid #f3c7c7;
            background: #fff1f1;
            color: #b42318;
            font-size: 0.86rem;
            font-weight: 600;
            line-height: 1.5;
        }

        .section-title {
            margin: 0;
            font-size: 1.1rem;
            font-weight: 700;
            letter-spacing: 0;
        }

        .grid-frame {
            margin-top: 18px;
            padding: 10px;
            border-radius: 12px;
            background: #fff;
            border: 1px solid var(--line-soft);
        }

        .grid-wrap {
            overflow-x: auto;
            border-radius: 10px;
        }

        .data-grid {
            width: 100%;
            margin-bottom: 0;
            border-collapse: separate;
            border-spacing: 0;
            color: var(--text-main);
            font-size: 0.86rem;
        }

        .data-grid th {
            padding: 11px 10px;
            border: 0;
            border-right: 1px solid rgba(255, 255, 255, 0.18);
            background: #3256b3;
            color: #fff;
            font-size: 0.8rem;
            font-weight: 600;
            line-height: 1.35;
            white-space: nowrap;
        }

        .data-grid td {
            padding: 10px 10px;
            vertical-align: middle;
            border-bottom: 1px solid rgba(31, 41, 51, 0.05);
            border-right: 1px solid rgba(31, 41, 51, 0.05);
            line-height: 1.35;
            background: rgba(255, 255, 255, 0.98);
        }

        .data-grid th:last-child,
        .data-grid td:last-child {
            border-right: 0;
        }

        .data-grid tr:nth-child(even) td {
            background: #f7fbff;
        }

        .data-grid a,
        .data-grid input[type="image"] {
            transition: transform 0.2s ease;
        }

        .data-grid input[type="image"]:hover,
        .data-grid a:hover {
            transform: translateY(-1px);
        }

        .data-grid td:nth-child(6) input[type="image"] {
            width: 34px;
            height: 34px;
        }

        .data-grid td:nth-child(6) a {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 58px;
            min-height: 32px;
            padding: 5px 10px;
            border-radius: 8px;
            font-size: 0.82rem;
            font-weight: 700;
            line-height: 1;
            text-decoration: none;
            color: #fff;
            background: #5b6b7a;
            box-shadow: 0 1px 2px rgba(15, 23, 42, 0.08);
        }

        .data-grid .edit-doc-btn,
        .data-grid .edit-doc-btn:visited,
        .data-grid .edit-doc-btn:hover,
        .data-grid .edit-doc-btn:focus {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 58px;
            min-height: 32px;
            padding: 5px 10px;
            border-radius: 8px;
            font-size: 0.82rem;
            font-weight: 700;
            line-height: 1;
            text-decoration: none;
            color: #4a3a00 !important;
            background: #facc15 !important;
            box-shadow: 0 1px 2px rgba(15, 23, 42, 0.08);
        }

        .data-grid .delete-doc-btn,
        .data-grid .delete-doc-btn:visited,
        .data-grid .delete-doc-btn:hover,
        .data-grid .delete-doc-btn:focus {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 58px;
            min-height: 32px;
            padding: 5px 10px;
            border-radius: 8px;
            font-size: 0.82rem;
            font-weight: 700;
            line-height: 1;
            text-decoration: none;
            color: #fff !important;
            background: #dc2626 !important;
            box-shadow: 0 1px 2px rgba(15, 23, 42, 0.08);
            border: 0 !important;
            cursor: pointer;
        }

        .data-grid .action-disabled,
        .data-grid .action-disabled:visited,
        .data-grid .action-disabled:hover,
        .data-grid .action-disabled:focus {
            background: #cbd5e1 !important;
            color: #64748b !important;
            pointer-events: none;
            cursor: not-allowed !important;
            box-shadow: none;
        }

        .data-grid td:nth-child(5) input[type="text"] {
            width: 100%;
            min-height: 36px;
            border: 1px solid #c8d4df;
            border-radius: 8px;
            padding: 7px 10px;
            font-size: 0.84rem;
        }

        .data-grid td:nth-child(3) {
            min-width: 420px;
            word-break: break-all;
        }

        .data-grid .aspNetDisabled,
        .field-disabled {
            background: #eef3f8 !important;
            color: #51606e !important;
            cursor: not-allowed;
        }

        .last-update {
            margin-top: 14px;
            text-align: right;
            color: var(--text-soft);
            font-size: 0.76rem;
        }

        @media (max-width: 991.98px) {
            .action-stack {
                justify-content: flex-start;
                margin-top: 6px;
            }
        }

        @media (max-width: 767.98px) {
            .page-shell {
                padding: 16px 0 28px;
            }

            .hero-card,
            .content-card {
                border-radius: 12px;
            }

            .hero-card,
            .filter-panel,
            .content-card {
                padding: 16px;
            }

            .hero-title {
                font-size: 1.35rem;
            }

            .data-grid {
                font-size: 0.8rem;
            }

            .data-grid th,
            .data-grid td {
                padding: 9px 8px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-shell">
            <div class="container-fluid px-3 px-lg-4">
                <div class="hero-card">
                    <span class="hero-kicker">Detail PDF</span>
                    <h1 class="hero-title">SHIPPING DOCUMENT CONTROL</h1>

                    <div class="filter-panel">
                        <div class="row g-3 align-items-end">
                            <div class="col-12 col-md-3 col-lg-2">
                                <label class="panel-label" for="<%= txtIVNO.ClientID %>">Invoice#</label>
                                <asp:TextBox ID="txtIVNO" runat="server" CssClass="field-control"></asp:TextBox>
                            </div>
                            <div class="col-12 col-md-4 col-lg-3">
                                <label class="panel-label" for="<%= ddlDATU.ClientID %>">Doc Type</label>
                                <asp:DropDownList ID="ddlDATU" runat="server" Enabled="False" CssClass="field-control">
                                    <asp:ListItem Value="1">Shipping Doc.</asp:ListItem>
                                    <asp:ListItem Value="2">QA Doc.</asp:ListItem>
                                    <asp:ListItem Value="3">Form Doc.</asp:ListItem>
                                    <asp:ListItem Value="4">H/C Doc.</asp:ListItem>
                                    <asp:ListItem Value="5">DHL Doc.</asp:ListItem>
                                    <asp:ListItem Value="6">P/L</asp:ListItem>
                                    <asp:ListItem Value="7">Custom</asp:ListItem>
                                    <asp:ListItem Value="8">Invoice</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-5 col-lg-4">
                                <label class="panel-label" for="<%= FileUpload1.ClientID %>">Select File</label>
                                <asp:FileUpload ID="FileUpload1" runat="server" CssClass="field-control" />
                            </div>
                            <div class="col-12 col-lg-3">
                                <div class="action-stack">
                                    <asp:ImageButton ID="imgAddPDF" runat="server" CssClass="icon-action upload-action" ImageUrl="~/Pictures/add.ico" OnClick="imgAddPDF_Click" ToolTip="ADD Pdf File" />
                                    <asp:ImageButton ID="imgHome" runat="server" CssClass="icon-action back-action" ImageUrl="~/Pictures/icons8-back-arrow-50.png" OnClick="imgHome_Click" ToolTip="Back" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:Label ID="lbError" runat="server" CssClass="error-banner" Visible="False"></asp:Label>
                </div>

                <div class="content-card">
                    <h2 class="section-title">Document List</h2>

                    <div class="grid-frame">
                        <div class="grid-wrap">
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                CssClass="table data-grid"
                                OnRowCommand="GridView1_RowCommand"
                                OnRowDataBound="GridView1_RowDataBound"
                                OnPageIndexChanging="GridView1_PageIndexChanging"
                                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                OnRowEditing="GridView1_RowEditing"
                                OnRowUpdating="GridView1_RowUpdating">
                                <Columns>
                                    <asp:TemplateField HeaderText="Revision">
                                        <ItemTemplate>
                                            <asp:Label ID="lblREVI" runat="server" Text='<%# Eval("SLREVI") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date.">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDATE" runat="server" Text='<%# Eval("SLDATE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="File Name.">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNAME" runat="server" Text='<%# Eval("SLFNAM") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ext." Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFEXT" runat="server" Text='<%# Eval("SLFEXT") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remark">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtREMK" runat="server" Text='<%# Eval("SLREMK") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblREMK" runat="server" Text='<%# Eval("SLREMK") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="lnkView" runat="server" NavigateUrl='<%# GetViewUrl(Eval("SLREVI")) %>' Target="_blank">
                                                <asp:Image ID="imgView" runat="server" ImageUrl="~/Pictures/view.ico" AlternateText="View" />
                                            </asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="DELDOC" CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="False" CssClass="delete-doc-btn">Delete</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ShowEditButton="True" />
                                </Columns>
                                <PagerStyle CssClass="small" />
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="last-update">@@ Last Update : 02/04/2026 13:41</div>
                </div>

                <div class="d-none">
                    <asp:HiddenField ID="STATUSD" runat="server" />
                    <asp:HiddenField ID="PASSW" runat="server" />
                    <asp:HiddenField ID="DATEFROM" runat="server" />
                    <asp:HiddenField ID="DATETO" runat="server" />
                    <asp:HiddenField ID="DATU" runat="server" />
                    <asp:HiddenField ID="INVNO" runat="server" />
                    <asp:HiddenField ID="CUNO" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
