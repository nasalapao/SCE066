<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066_RPT.aspx.cs" Inherits="SCE066_RPT" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 192px;
        }
        .auto-style3 {
            width: 182px;
        }
        .auto-style6 {
            width: 192px;
            height: 23px;
        }
        .auto-style7 {
            width: 182px;
            height: 23px;
        }
        .auto-style8 {
            height: 23px;
        }
        .auto-style23 {
            width: 91px;
            text-align: center;
            margin-left: 1px;
        }
        .auto-style24 {
            width: 110px;
        }
        .auto-style26 {
            width: 120px;
            text-align: center;
            margin-left: 87px;
        }
        .auto-style28 {
            font-size: x-large;
            color: #FFFFFF;
            background-color: #0000CC;
        }
        .auto-style32 {
            width: 120px;
        }
        .auto-style34 {
            width: 120px;
            height: 29px;
        }
        .auto-style35 {
            width: 110px;
            height: 29px;
        }
        .auto-style36 {
            height: 29px;
        }
        .auto-style37 {
            width: 126px;
        }
        .auto-style38 {
            width: 126px;
            height: 29px;
        }
        .auto-style39 {
            width: 158px;
        }
        .auto-style40 {
            width: 173px;
        }
    </style>
</head>
<body>
     <meta charset="utf-8">
     <meta name="viewport" content="width=device-width, initial-scale=1">
     <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
     <link rel="stylesheet" href="/resources/demos/style.css">
     <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
     <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
     <script>
         $(function () {
             $('#datepicker1').datepicker({ dateFormat: 'dd/mm/yy' }).val();
             $('#datepicker2').datepicker({ dateFormat: 'dd/mm/yy' }).val();
         });
      </script>
    <form id="form1" runat="server">
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong><span class="auto-style28">Performance Upload File </span></strong>
        <br />
        <table class="auto-style1">
            <tr>
                <td class="auto-style37">&nbsp;</td>
                <td class="auto-style32">&nbsp;</td>
                <td class="auto-style24">&nbsp;</td>
                <td class="auto-style24">
                    &nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style37">From Date :</td>
                <td class="auto-style32">&nbsp;<input id="datepicker1" runat="server" type="text" class="auto-style23" /></td>
                <td class="auto-style24">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; To Date :</td>
                <td class="auto-style24">
&nbsp;
                    <input id="datepicker2" runat="server" type="text" class="auto-style23" /></td>
                <td></td>
            </tr>
            <tr>
                <td class="auto-style38">User</td>
                <td class="auto-style34">&nbsp;<asp:DropDownList ID="ddlUSER" runat="server">
                    </asp:DropDownList>
                </td>
                <td class="auto-style35">Customer :</td>
                <td class="auto-style35">
                <asp:DropDownList ID="ddlCUST" runat="server">
                </asp:DropDownList>
                    </td>
                <td class="auto-style36"></td>
            </tr>
            <tr>
                <td class="auto-style38">Type Report :</td>
                <td class="auto-style34">&nbsp;<asp:DropDownList ID="ddlTYPE" runat="server">
                    <asp:ListItem Selected="True">Delay </asp:ListItem>
                    <asp:ListItem>All</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="auto-style35">&nbsp;</td>
                <td class="auto-style35">
                    &nbsp;</td>
                <td class="auto-style36">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style37">&nbsp;</td>
                <td class="auto-style26">&nbsp;</td>
                <td class="auto-style24">
                    <asp:ImageButton ID="ImageButton1" runat="server" Height="42px" ImageUrl="~/Pictures/report-icon-13332.png" OnClick="ImageButton1_Click" ToolTip="Print" Width="44px" />
&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="ImageButton2" runat="server" Height="42px" ImageUrl="~/Pictures/icons8-back-arrow-50.png" style="text-align: center" ToolTip="Back" OnClick="ImageButton2_Click" />
                </td>
                <td class="auto-style24">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                </td>
                <td class="auto-style3">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style6"></td>
                <td class="auto-style7"></td>
                <td class="auto-style8"></td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="rptViewer" runat="server" Width="876px">
        </rsweb:ReportViewer>
        <table class="auto-style1">
            <tr>
                <td class="auto-style39">
                <asp:HiddenField ID="DATEFROM" runat="server" />
                </td>
                <td class="auto-style40">
                <asp:HiddenField ID="DATETO" runat="server" />

                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style39">&nbsp;</td>
                <td class="auto-style40">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </form>
</body>
</html>
