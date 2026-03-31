<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066.aspx.cs" Inherits="SCE066" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style2 {
            width: 100%;
        }
        .auto-style3 {
            height: 23px;
        }
        .auto-style6 {
            height: 23px;
            width: 101px;
            font-size: small;
        }
        #datepicker1 {
            width: 85px;
        }
        .auto-style8 {
            height: 23px;
            text-align: left;
        }
        #datepicker2 {
            width: 85px;
        }
        .auto-style10 {
            height: 22px;
        }
        .auto-style11 {
            width: 95px;
            height: 22px;
            font-size: small;
        }
        .auto-style12 {
            width: 119px;
            height: 22px;
        }
        .auto-style13 {
            width: 101px;
            height: 22px;
        }
        .auto-style14 {
            height: 23px;
            width: 95px;
        }
        .auto-style15 {
            height: 23px;
            width: 104px;
        }
        .auto-style16 {
            width: 104px;
            height: 22px;
        }
        .auto-style19 {
            height: 23px;
            text-align: center;
            font-size: large;
            background-color: #0000CC;
            color: #FFFFFF;
        }
        .auto-style22 {
            width: 104px;
            height: 22px;
            font-size: small;
        }
        .auto-style23 {
            font-size: small;
        }
        .auto-style24 {
            height: 23px;
            width: 95px;
            font-size: small;
        }
        .auto-style25 {
            width: 116px;
        }
        .auto-style26 {
            width: 140px;
        }
        .auto-style27 {
            width: 129px;
        }
        .auto-style28 {
            width: 139px;
        }
        .auto-style29 {
            height: 23px;
            width: 99px;
        }
        .auto-style30 {
            width: 99px;
            height: 22px;
        }
        .auto-style31 {
            width: 101px;
            height: 22px;
            font-size: small;
        }
        .auto-style32 {
            height: 23px;
            width: 172px;
        }
        .auto-style33 {
            width: 172px;
            height: 22px;
            font-size: small;
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
    <div>
    
    <table class="auto-style2">
        <tr>
            <td class="auto-style29">
                &nbsp;</td>
            <td class="auto-style14">&nbsp;</td>
            <td class="auto-style19" colspan="3">
                <strong>SHIPPING DOCUMENT CONTROL</strong></td>
            <td class="auto-style8">
                &nbsp;</td>
            <td class="auto-style8">
                &nbsp;</td>
            <td class="auto-style3">&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style29">
                &nbsp;</td>
            <td class="auto-style24">Date from : </td>
            <td class="auto-style8">
                <input id="datepicker1" runat="server" type="text" class="auto-style23" /></td>
            <td class="auto-style6">Date To :</td>
            <td class="auto-style32">
                <input id="datepicker2" runat="server" type="text" class="auto-style23" /></td>
            <td class="auto-style15">
                </td>
            <td class="auto-style15">
                </td>
            <td class="auto-style3"></td>
        </tr>
        <tr>
            <td class="auto-style30">
                </td>
            <td class="auto-style11">Invoice No</td>
            <td class="auto-style12">
                <asp:TextBox ID="txtIVNO" runat="server" Width="84px"></asp:TextBox>
            </td>
            <td class="auto-style31">
                Customer </td>
            <td class="auto-style33">
                <asp:DropDownList ID="ddlCUST" runat="server">
                </asp:DropDownList>
            </td>
            <td class="auto-style16">
                </td>
            <td class="auto-style22">&nbsp;</td>
            <td class="auto-style10"></td>
        </tr>
        <tr>
            <td class="auto-style30">
                &nbsp;</td>
            <td class="auto-style11">Status Doc.</td>
            <td class="auto-style12">
                <asp:DropDownList ID="ddlSTATUS" runat="server" CssClass="auto-style23">
                    <asp:ListItem Value="0">Un-Complete</asp:ListItem>
                    <asp:ListItem Value="1">Complete</asp:ListItem>
                    <asp:ListItem Value="3" Selected="True">ALL</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="auto-style13">
                <asp:ImageButton ID="ImageButton1" runat="server" Height="28px" ImageUrl="~/Pictures/view.ico" OnClick="ImageButton1_Click" ToolTip="Search" Width="52px" CssClass="auto-style23" />
            </td>
            <td class="auto-style33">password:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txtPASS" runat="server" Width="72px" CssClass="auto-style23"></asp:TextBox>
            </td>
            <td class="auto-style16">
                &nbsp;</td>
            <td class="auto-style22">&nbsp;</td>
            <td class="auto-style10"></td>
        </tr>
        <tr>
            <td class="auto-style30">
                &nbsp;</td>
            <td class="auto-style11">
                &nbsp;</td>
            <td class="auto-style12">
                &nbsp;</td>
            <td class="auto-style13">
                <asp:ImageButton ID="imgbtnPrint" runat="server" Height="37px" ImageUrl="~/Pictures/report-icon-13338.jpg" ToolTip="Report" Width="86px" OnClick="imgbtnPrint_Click" />
            </td>
            <td class="auto-style33">
                &nbsp;</td>
            <td class="auto-style16">
                &nbsp;</td>
            <td class="auto-style22">&nbsp;</td>
            <td class="auto-style10">&nbsp;</td>
        </tr>
        </table>
        </div>
        <table class="auto-style2">
            <tr>
                <td>&nbsp;</td>
                <td>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" Width="1000px" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowCommand="GridView1_RowCommand" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" PageSize="9" OnRowDataBound="GridView1_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="InvoiceNo">
                            <ItemTemplate>
                                <asp:Label ID="lblIVNO" runat="server" Text='<%# Eval("SHIVNO") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="STS.">
                            <ItemTemplate>
                                <asp:Label ID="lblORST" runat="server" Text='<%# Eval("OAORST") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer">
                            <ItemTemplate>
                                <asp:Label ID="lblCUNM" runat="server" Text='<%# Eval("CUNM") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle VerticalAlign="Bottom" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ETD Date.">
                            <ItemTemplate>
                                <asp:Label ID="lblETDD" runat="server" Text='<%# Eval("SHETDD") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ETA Date.">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtETAD" runat="server" Height="16px" Text='<%# Eval("SHETAD") %>' Width="74px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblETAD" runat="server" Text='<%# Eval("SHETAD") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status Doc.">
                            <ItemTemplate>
                                <asp:Label ID="lblSTSD" runat="server" Text='<%# Eval("SHSTSD") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtUSID" runat="server" Height="16px" Text='<%# Eval("SHUSID") %>' Width="63px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblUSID" runat="server" Text='<%# Eval("SHUSID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipping Doc.">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT1" runat="server" Text='<%# Eval("SHDAT1") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" HeaderText="Up. Ship" ImageUrl="~/Pictures/report.ico" Text="Up." CommandName="UPDAT1" />
                        <asp:TemplateField HeaderText="QA Doc.">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT2" runat="server" Text='<%# Eval("SHDAT2") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" CommandName="UPDAT2" HeaderText="Up. QA" ImageUrl="~/Pictures/report.ico" Text="Up." />
                        <asp:TemplateField HeaderText="Form Doc.">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT3" runat="server" Text='<%# Eval("SHDAT3") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" CommandName="UPDAT3" HeaderText="Up Form" ImageUrl="~/Pictures/report.ico" Text="Up." />
                        <asp:TemplateField HeaderText="H/C Doc.">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT4" runat="server" Text='<%# Eval("SHDAT4") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" CommandName="UPDAT4" ImageUrl="~/Pictures/report.ico" Text="Up." HeaderText="Up H/C" />
                        <asp:TemplateField HeaderText="Fed/DHL.">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT5" runat="server" Text='<%# Eval("SHDAT5") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" HeaderText="UP DHL" ImageUrl="~/Pictures/report.ico" Text="Up." CommandName="UPDAT5" />
                        <asp:TemplateField HeaderText="P/L">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT6" runat="server" Text='<%# Eval("SHDAT6") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField CommandName="UPDAT6" HeaderText="UP P/L" ImageUrl="~/Pictures/report.ico" Text="Up." ButtonType="Image" />
                        <asp:TemplateField HeaderText="Custom">
                            <ItemTemplate>
                                <asp:Label ID="lblDAT7" runat="server" Text='<%# Eval("SHDAT7") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" CommandName="UPDAT7" HeaderText="Up Custom" ImageUrl="~/Pictures/report.ico" Text="Up." />
                        <asp:CommandField ShowEditButton="True" />
                        <asp:ButtonField ButtonType="Image" CommandName="Appr" ImageUrl="~/Pictures/iconfinder_Thumbs up_32563.png" Text="Approve" />
                        <asp:ButtonField ButtonType="Image" CommandName="NotApr" ImageUrl="~/Pictures/iconfinder_Bad mark_32436.png" Text="Not_Approve" />
                        <asp:ButtonField ButtonType="Image" CommandName="Explorer" ImageUrl="~/Pictures/explorer-icon.png" Text="Explorer." Visible="False" />
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
                    <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                    <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
                    <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                    <RowStyle BackColor="White" ForeColor="#003399" />
                    <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
        </table>
        <table class="auto-style2">
            <tr>
                <td class="auto-style25">
                <asp:HiddenField ID="INVNO" runat="server" />

                </td>
                <td class="auto-style26">
                <asp:HiddenField ID="DATEFROM" runat="server" />
                </td>
                <td class="auto-style27">
                <asp:HiddenField ID="DATETO" runat="server" />

                </td>
                <td class="auto-style28">
                <asp:HiddenField ID="STATUSD" runat="server" />

                </td>
                <td>
                <asp:HiddenField ID="PASSW" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="auto-style25">
                <asp:HiddenField ID="CUNO" runat="server" />
                </td>
                <td class="auto-style26">&nbsp;</td>
                <td class="auto-style27">&nbsp;</td>
                <td class="auto-style28">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </form>
     <p>
         @@ Last Update : 14/02/2024 10:00</p>
    </body>
</html>
