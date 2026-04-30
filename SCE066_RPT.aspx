<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066_RPT.aspx.cs" Inherits="SCE066_RPT" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Shipping Document Report</title>
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
            --text-main: #1f2933;
            --text-soft: #5b6b7a;
            --line-soft: #e8eef5;
            --shadow-lg: 0 12px 30px rgba(15, 35, 55, 0.08);
            --shadow-sm: 0 4px 16px rgba(15, 35, 55, 0.05);
        }

        html,
        body,
        form {
            min-height: 100%;
            margin: 0;
        }

        body {
            font-family: 'Noto Sans Thai', 'Segoe UI', Tahoma, sans-serif;
            color: var(--text-main);
            background: var(--page-bg);
        }

        .page-shell {
            min-height: 100vh;
            padding: 24px 0;
        }

        .hero-card,
        .report-card {
            background: var(--surface);
            border: 1px solid var(--line-soft);
            border-radius: 14px;
            box-shadow: var(--shadow-lg);
        }

        .hero-card {
            padding: 24px;
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
            margin: 14px 0 4px;
            font-size: 1.7rem;
            line-height: 1.2;
            font-weight: 700;
            letter-spacing: 0;
        }

        .hero-subtitle {
            margin: 0;
            color: var(--text-soft);
            font-size: 0.9rem;
            line-height: 1.6;
        }

        .filter-panel {
            margin-top: 20px;
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

        .icon-action.report-action {
            background: #5a6f85;
        }

        .icon-action.back-action {
            background: #1f4e79;
        }

        .icon-action img {
            max-width: 22px;
            max-height: 22px;
        }

        .report-card {
            margin-top: 24px;
            padding: 14px;
            height: calc(100vh - 260px);
            min-height: calc(100vh - 260px);
        }

        .report-viewer-wrap {
            width: 100%;
            height: 100%;
            min-height: calc(100vh - 290px);
            overflow: auto;
            border-radius: 10px;
            border: 1px solid var(--line-soft);
            background: #fff;
        }

        .report-viewer-wrap table {
            max-width: none;
        }

        @media (max-width: 767.98px) {
            .page-shell {
                padding: 12px 0;
            }

            .hero-card {
                padding: 18px;
                border-radius: 12px;
            }

            .hero-title {
                font-size: 1.35rem;
            }

            .filter-panel {
                padding: 16px;
            }

            .action-stack {
                width: 100%;
            }

            .icon-action {
                width: 52px;
                height: 52px;
            }

            .report-card {
                padding: 8px;
                height: calc(100vh - 420px);
                min-height: calc(100vh - 420px);
            }

            .report-viewer-wrap {
                min-height: calc(100vh - 440px);
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <div class="page-shell">
            <div class="container-fluid px-3 px-md-4 px-xxl-5">
                <div class="hero-card">
                    <span class="hero-kicker">Export Operation Hub</span>
                    <h1 class="hero-title">Shipping Document Report</h1>
                    <p class="hero-subtitle">Report summary by date, user, customer, and document delay status.</p>

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
                                <label class="panel-label" for="<%= ddlUSER.ClientID %>">User</label>
                                <asp:DropDownList ID="ddlUSER" runat="server" CssClass="field-control">
                                </asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="panel-label" for="<%= ddlCUST.ClientID %>">Customer</label>
                                <asp:DropDownList ID="ddlCUST" runat="server" CssClass="field-control">
                                </asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="panel-label" for="<%= ddlTYPE.ClientID %>">Type Report</label>
                                <asp:DropDownList ID="ddlTYPE" runat="server" CssClass="field-control">
                                    <asp:ListItem Selected="True">Delay </asp:ListItem>
                                    <asp:ListItem>All</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-12 col-md-6 col-xl-1">
                                <div class="action-stack">
                                    <asp:ImageButton ID="ImageButton1" runat="server" CssClass="icon-action report-action" ImageUrl="~/Pictures/report-icon-13332.png" OnClick="ImageButton1_Click" ToolTip="Print" />
                                    <asp:ImageButton ID="ImageButton2" runat="server" CssClass="icon-action back-action" ImageUrl="~/Pictures/icons8-back-arrow-50.png" ToolTip="Back" OnClick="ImageButton2_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="report-card">
                    <div class="report-viewer-wrap">
                        <rsweb:ReportViewer ID="rptViewer" runat="server" Width="100%" Height="100%">
                        </rsweb:ReportViewer>
                    </div>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="DATEFROM" runat="server" />
        <asp:HiddenField ID="DATETO" runat="server" />
    </form>
</body>
</html>
