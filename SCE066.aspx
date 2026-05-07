<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066.aspx.cs" Inherits="SCE066" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Shipping Document Control</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Thai:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />
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
            margin: 14px 0 6px;
            font-size: 1.7rem;
            line-height: 1.2;
            font-weight: 700;
            letter-spacing: 0;
        }

        .hero-subtitle {
            margin: 0;
            max-width: 880px;
            color: var(--text-soft);
            font-size: 0.9rem;
            line-height: 1.6;
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
        select.field-control {
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
        select.field-control:focus {
            outline: none;
            border-color: rgba(31, 78, 121, 0.55);
            box-shadow: 0 0 0 0.15rem rgba(31, 78, 121, 0.12);
        }

        .action-stack {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
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

        .icon-action.search-action {
            background: #1f4e79;
        }

        .icon-action.report-action {
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

        .section-copy {
            margin: 4px 0 0;
            color: var(--text-soft);
            font-size: 0.86rem;
            line-height: 1.5;
        }

        .grid-frame {
            margin-top: 18px;
            padding: 10px;
            border-radius: 12px;
            background: #fff;
            border: 1px solid var(--line-soft);
        }

        .grid-wrap {
            border-radius: 10px;
            overflow-x: visible;
        }

        .data-grid {
            width: 100%;
            margin-bottom: 0;
            table-layout: fixed;
            border-collapse: separate;
            border-spacing: 0;
            color: var(--text-main);
            font-size: 0.76rem;
            min-width: 0;
        }

        .data-grid th {
            position: sticky;
            top: 0;
            z-index: 1;
            padding: 9px 4px;
            border: 0;
            border-right: 1px solid rgba(255, 255, 255, 0.18);
            background: #3256b3;
            color: #fff;
            font-size: 0.72rem;
            font-weight: 600;
            line-height: 1.35;
            white-space: normal;
            word-break: break-word;
        }

        .data-grid td {
            padding: 8px 4px;
            vertical-align: middle;
            border-bottom: 1px solid rgba(31, 41, 51, 0.05);
            border-right: 1px solid rgba(31, 41, 51, 0.05);
            white-space: nowrap;
            word-break: normal;
            line-height: 1.35;
            background: rgba(255, 255, 255, 0.98);
        }

        .data-grid th:nth-child(3),
        .data-grid td:nth-child(3) {
            width: 17%;
            white-space: normal;
            word-break: normal;
        }

        .data-grid th:nth-child(1),
        .data-grid td:nth-child(1),
        .data-grid th:nth-child(4),
        .data-grid td:nth-child(4),
        .data-grid th:nth-child(5),
        .data-grid td:nth-child(5),
        .data-grid th:nth-child(7),
        .data-grid td:nth-child(7),
        .data-grid th:nth-child(8),
        .data-grid td:nth-child(8),
        .data-grid th:nth-child(10),
        .data-grid td:nth-child(10),
        .data-grid th:nth-child(12),
        .data-grid td:nth-child(12),
        .data-grid th:nth-child(14),
        .data-grid td:nth-child(14),
        .data-grid th:nth-child(16),
        .data-grid td:nth-child(16),
        .data-grid th:nth-child(18),
        .data-grid td:nth-child(18),
        .data-grid th:nth-child(20),
        .data-grid td:nth-child(20),
        .data-grid th:nth-child(22),
        .data-grid td:nth-child(22),
        .data-grid th:nth-child(24),
        .data-grid td:nth-child(24),
        .data-grid th:nth-child(25),
        .data-grid td:nth-child(25),
        .data-grid th:nth-child(26),
        .data-grid td:nth-child(26) {
            white-space: nowrap;
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

        .data-grid td:nth-child(24) a {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 54px;
            min-height: 30px;
            padding: 4px 9px;
            border-radius: 8px;
            font-size: 0.8rem;
            font-weight: 700;
            line-height: 1;
            text-decoration: none;
            color: #fff;
            background: #6b7280;
            box-shadow: 0 1px 2px rgba(15, 23, 42, 0.08);
        }

        .data-grid td:nth-child(24) a:only-child,
        .data-grid td:nth-child(24) a:first-child {
            background: #facc15;
            color: #4a3a00;
        }

        .data-grid td:nth-child(24) a:first-child:not(:last-child) {
            background: #15803d;
            color: #fff;
        }

        .data-grid td:nth-child(24) a:last-child:not(:first-child) {
            background: #dc2626;
        }

        .data-grid input[type="image"]:hover,
        .data-grid a:hover {
            transform: translateY(-1px);
        }

        .data-grid td:nth-child(24) a + a {
            margin-left: 4px;
        }

        .data-grid input[type="image"] {
            max-width: 13px;
            max-height: 13px;
            width: 13px;
            height: 13px;
        }

        .data-grid td:nth-child(25) input[type="image"],
        .data-grid td:nth-child(26) input[type="image"] {
            width: 22px !important;
            height: 22px !important;
            max-width: 22px !important;
            max-height: 22px !important;
        }

        .data-grid input[type="text"] {
            width: 100%;
            min-width: 0;
            padding: 7px 8px;
            border: 1px solid rgba(18, 48, 77, 0.15);
            border-radius: 6px;
            font-size: 0.86rem;
        }

        .data-grid .aspNetDisabled {
            opacity: 0.75;
        }

        .last-update {
            margin-top: 16px;
            text-align: right;
            color: var(--text-soft);
            font-size: 0.78rem;
        }

        @media (max-width: 991.98px) {
            .page-shell {
                padding-top: 20px;
            }

            .hero-card,
            .content-card {
                border-radius: 12px;
            }

            .filter-panel {
                padding: 18px;
            }

            .grid-frame {
                padding: 10px;
            }
        }

        @media (max-width: 575.98px) {
            .hero-card {
                padding: 20px;
            }

            .content-card {
                padding: 18px;
            }

            .action-stack {
                width: 100%;
            }

            .icon-action {
                width: 52px;
                height: 52px;
            }
        }
    </style>
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script>
        $(function () {
            $('#<%= datepicker1.ClientID %>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
            $('#<%= datepicker2.ClientID %>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
        });

        function openPrintReport() {
            var dateFrom = document.getElementById('<%= datepicker1.ClientID %>').value;
            var dateTo = document.getElementById('<%= datepicker2.ClientID %>').value;
            var url = 'SCE066_RPT.aspx?DATEFROM=' + encodeURIComponent(dateFrom) + '&DATETO=' + encodeURIComponent(dateTo);

            window.open(url, '_blank');
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-shell">
            <div class="container-fluid px-3 px-md-4 px-xxl-5">
                <div class="hero-card">
                    <span class="hero-kicker">Export Operation Hub</span>
                    <h1 class="hero-title">Shipping Document Control</h1>
                   

                    <div class="filter-panel">
                        <div class="row g-3 align-items-end">
                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="panel-label" for="<%= datepicker1.ClientID %>">Date From</label>
                                <input id="datepicker1" runat="server" type="text" class="field-control" autocomplete="off" />
                            </div>
                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="panel-label" for="<%= datepicker2.ClientID %>">Date To</label>
                                <input id="datepicker2" runat="server" type="text" class="field-control" autocomplete="off" />
                            </div>
                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="panel-label" for="<%= txtIVNO.ClientID %>">Invoice No</label>
                                <asp:TextBox ID="txtIVNO" runat="server" CssClass="field-control"></asp:TextBox>
                            </div>
                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="panel-label" for="<%= ddlCUST.ClientID %>">Customer</label>
                                <asp:DropDownList ID="ddlCUST" runat="server" CssClass="field-control"></asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="panel-label" for="<%= ddlSTATUS.ClientID %>">Status Doc.</label>
                                <asp:DropDownList ID="ddlSTATUS" runat="server" CssClass="field-control">
                                    <asp:ListItem Value="0">Un-Complete</asp:ListItem>
                                    <asp:ListItem Value="1">Complete</asp:ListItem>
                                    <asp:ListItem Value="2">Not Approve</asp:ListItem>
                                    <asp:ListItem Value="3" Selected="True">ALL</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-6 col-xl-1">
                                <label class="panel-label" for="<%= txtPASS.ClientID %>">Password</label>
                                <asp:TextBox ID="txtPASS" runat="server" CssClass="field-control"></asp:TextBox>
                            </div>
                            <div class="col-12">
                                <div class="action-stack">
                                    <asp:ImageButton ID="ImageButton1" runat="server" CssClass="icon-action search-action" ImageUrl="~/Pictures/view.ico" OnClick="ImageButton1_Click" ToolTip="Search" />
                                    <asp:ImageButton ID="imgbtnPrint" runat="server" CssClass="icon-action report-action" ImageUrl="~/Pictures/report-icon-13338.jpg" ToolTip="Report" OnClientClick="return openPrintReport();" />
                                </div>
                            </div>
                        </div>
                        <asp:Label ID="lbError" runat="server" CssClass="error-banner" Visible="False"></asp:Label>
                    </div>
                </div>

                <div class="content-card">
                    <div class="d-flex flex-column flex-lg-row justify-content-between align-items-lg-center gap-2">
                        <div>
                            <h2 class="section-title">Document Status Grid</h2>
                          
                        </div>
                    </div>

                    <div class="grid-frame">
                        <div class="grid-wrap">
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="9"
                                CssClass="table data-grid"
                                OnPageIndexChanging="GridView1_PageIndexChanging"
                                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                OnRowCommand="GridView1_RowCommand"
                                OnRowEditing="GridView1_RowEditing"
                                OnRowUpdating="GridView1_RowUpdating"
                                OnRowDataBound="GridView1_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Invoice">
                                        <HeaderStyle Width="57px" />
                                        <ItemStyle Width="57px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblIVNO" runat="server" Text='<%# Eval("SHIVNO") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ST">
                                        <HeaderStyle Width="25px" />
                                        <ItemStyle Width="25px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblORST" runat="server" Text='<%# Eval("OAORST") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer">
                                        <HeaderStyle Width="240px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblCUNM" runat="server" Text='<%# Eval("CUNM") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="280px" VerticalAlign="Bottom" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ETD Date.">
                                        <HeaderStyle Width="64px" />
                                        <ItemStyle Width="64px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblETDD" runat="server" Text='<%# Eval("SHETDD") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ETA Date.">
                                        <HeaderStyle Width="64px" />
                                        <ItemStyle Width="64px" />
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtETAD" runat="server" Text='<%# Eval("SHETAD") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblETAD" runat="server" Text='<%# Eval("SHETAD") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status Doc.">
                                        <HeaderStyle Width="80px" />
                                        <ItemStyle Width="80px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTSD" runat="server" Text='<%# Eval("SHSTSD") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User">
                                        <HeaderStyle Width="44px" />
                                        <ItemStyle Width="44px" />
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtUSID" runat="server" Text='<%# Eval("SHUSID") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblUSID" runat="server" Text='<%# Eval("SHUSID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipping Doc.">
                                        <HeaderStyle Width="56px" />
                                        <ItemStyle Width="56px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT1" runat="server" Text='<%# Eval("SHDAT1") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Up. Ship" ImageUrl="~/Pictures/report.ico" Text="Up." CommandName="UPDAT1">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="INV Doc.">
                                        <HeaderStyle Width="56px" />
                                        <ItemStyle Width="56px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT8" runat="server" Text='<%# Eval("SHDAT8") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Up. INV" ImageUrl="~/Pictures/report.ico" Text="Up." CommandName="UPDAT8">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="QA Doc.">
                                        <HeaderStyle Width="56px" />
                                        <ItemStyle Width="56px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT2" runat="server" Text='<%# Eval("SHDAT2") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" CommandName="UPDAT2" HeaderText="Up. QA" ImageUrl="~/Pictures/report.ico" Text="Up.">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="Form Doc.">
                                        <HeaderStyle Width="56px" />
                                        <ItemStyle Width="56px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT3" runat="server" Text='<%# Eval("SHDAT3") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" CommandName="UPDAT3" HeaderText="Up Form" ImageUrl="~/Pictures/report.ico" Text="Up.">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="H/C Doc.">
                                        <HeaderStyle Width="65px" />
                                        <ItemStyle Width="65px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT4" runat="server" Text='<%# Eval("SHDAT4") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" CommandName="UPDAT4" ImageUrl="~/Pictures/report.ico" Text="Up." HeaderText="Up H/C">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="Fed/DHL.">
                                        <HeaderStyle Width="56px" />
                                        <ItemStyle Width="56px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT5" runat="server" Text='<%# Eval("SHDAT5") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="UP DHL" ImageUrl="~/Pictures/report.ico" Text="Up." CommandName="UPDAT5">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="P/L">
                                        <HeaderStyle Width="65px" />
                                        <ItemStyle Width="65px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT6" runat="server" Text='<%# Eval("SHDAT6") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField CommandName="UPDAT6" HeaderText="UP P/L" ImageUrl="~/Pictures/report.ico" Text="Up." ButtonType="Image">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:TemplateField HeaderText="Custom">
                                        <HeaderStyle Width="70px" />
                                        <ItemStyle Width="70px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDAT7" runat="server" Text='<%# Eval("SHDAT7") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" CommandName="UPDAT7" HeaderText="Up Custom" ImageUrl="~/Pictures/report.ico" Text="Up.">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:CommandField ShowEditButton="True">
                                        <HeaderStyle Width="82px" />
                                        <ItemStyle Width="82px" HorizontalAlign="Center" />
                                        <ControlStyle Font-Size="11px" />
                                    </asp:CommandField>
                                    <asp:ButtonField ButtonType="Image" CommandName="Appr" ImageUrl="~/Pictures/iconfinder_Thumbs up_32563.png" Text="Approve">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" CommandName="NotApr" ImageUrl="~/Pictures/iconfinder_Bad mark_32436.png" Text="Not_Approve">
                                        <HeaderStyle Width="40px" />
                                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                                        <ControlStyle Width="40px" Height="40px" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" CommandName="Explorer" ImageUrl="~/Pictures/explorer-icon.png" Text="Explorer." Visible="False" />
                                    <asp:TemplateField HeaderText="STS8" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS8" runat="server" Text='<%# Eval("SHSTS8") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS1" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS1" runat="server" Text='<%# Eval("SHSTS1") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS2" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS2" runat="server" Text='<%# Eval("SHSTS2") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS3" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS3" runat="server" Text='<%# Eval("SHSTS3") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS4" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS4" runat="server" Text='<%# Eval("SHSTS4") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS5" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSTS5" runat="server" Text='<%# Eval("SHSTS5") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS6" Visible="False"></asp:TemplateField>
                                    <asp:TemplateField HeaderText="STS7" Visible="False"></asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="small" />
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="last-update">@@ Last Update : 24/04/2026 14:53</div>
                </div>

                <div class="d-none">
                    <asp:HiddenField ID="INVNO" runat="server" />
                    <asp:HiddenField ID="DATEFROM" runat="server" />
                    <asp:HiddenField ID="DATETO" runat="server" />
                    <asp:HiddenField ID="STATUSD" runat="server" />
                    <asp:HiddenField ID="PASSW" runat="server" />
                    <asp:HiddenField ID="CUNO" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
