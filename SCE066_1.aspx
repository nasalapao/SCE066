<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCE066_1.aspx.cs" Inherits="SCE066_1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

        .auto-style2 {
            width: 100%;
            height: 146px;
        }
        .auto-style14 {
            height: 24px;
            width: 109px;
        }
        .auto-style19 {
            height: 24px;
            text-align: center;
            font-size: small;
            background-color: #0000CC;
        }
        .auto-style8 {
            height: 24px;
            text-align: left;
            width: 223px;
        }
        .auto-style3 {
            height: 24px;
        }
        #datepicker1 {
            width: 85px;
        }
        #datepicker2 {
            width: 85px;
        }
        .auto-style25 {
            height: 107px;
        }
        .auto-style44 {
            width: 133px;
            height: 25px;
            font-size: small;
            text-align: right;
        }
        .auto-style52 {
            width: 21px;
            height: 14px;
        }
        .auto-style53 {
            width: 100%;
        }
        .auto-style54 {
            width: 146px;
        }
        .auto-style55 {
            width: 133px;
        }
        .auto-style56 {
            width: 142px;
            font-style: italic;
        }
        .auto-style57 {
            height: 24px;
            width: 102px;
        }
        .auto-style62 {
            height: 24px;
            width: 50px;
        }
        .auto-style65 {
            width: 102px;
            height: 25px;
        }
        .auto-style66 {
            width: 109px;
            height: 25px;
            font-size: small;
        }
        .auto-style67 {
            height: 25px;
        }
        .auto-style68 {
            width: 83px;
            height: 25px;
            font-size: small;
        }
        .auto-style69 {
            width: 77px;
            height: 25px;
            font-size: small;
        }
        .auto-style70 {
            width: 104px;
            height: 25px;
        }
        .auto-style71 {
            width: 223px;
            height: 25px;
            font-size: small;
        }
        .auto-style72 {
            height: 25px;
            width: 50px;
        }
        .auto-style73 {
            width: 109px;
            height: 14px;
            font-size: small;
        }
        .auto-style74 {
            width: 83px;
            height: 14px;
            font-size: small;
        }
        .auto-style75 {
            width: 77px;
            height: 14px;
            font-size: small;
        }
        .auto-style76 {
            width: 104px;
            height: 14px;
        }
        .auto-style77 {
            width: 223px;
            height: 14px;
            font-size: small;
        }
        .auto-style78 {
            height: 14px;
            width: 102px;
        }
        .auto-style79 {
            height: 14px;
        }
        .auto-style80 {
            height: 14px;
            width: 50px;
        }
        .auto-style81 {
            font-size: small;
        }
        .auto-style82 {
            width: 133px;
            height: 14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    
    <table class="auto-style2">
        <tr>
            <td class="auto-style57">
            </td>
            <td class="auto-style14"></td>
            <td class="auto-style19" colspan="5">
                <strong style="color: #FFFFFF">SHIPPING DOCUMENT CONTROL - DETAIL PDF</strong></td>
            <td class="auto-style8">
                </td>
            <td class="auto-style62"></td>
            <td class="auto-style3"></td>
        </tr>
        <tr>
            <td class="auto-style65">
            </td>
            <td class="auto-style66">Invoice#</td>
            <td class="auto-style67">
                <asp:TextBox ID="txtIVNO" runat="server" Width="84px" CssClass="auto-style81"></asp:TextBox>
            </td>
            <td class="auto-style44">
                &nbsp;Doc Type :</td>
            <td class="auto-style68">
                <asp:DropDownList ID="ddlDATU" runat="server" Enabled="False" CssClass="auto-style81">
                    <asp:ListItem Value="1">Shipping Doc.</asp:ListItem>
                    <asp:ListItem Value="2">QA Doc.</asp:ListItem>
                    <asp:ListItem Value="3">Form Doc.</asp:ListItem>
                    <asp:ListItem Value="4">H/C Doc.</asp:ListItem>
                    <asp:ListItem Value="5">DHL Doc.</asp:ListItem>
                    <asp:ListItem Value="6">P/L</asp:ListItem>
                    <asp:ListItem Value="7">Custom</asp:ListItem>
                    <asp:ListItem Value="8">Invoice</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="auto-style69"></td>
            <td class="auto-style70">
            </td>
            <td class="auto-style71"></td>
            <td class="auto-style72"></td>
            <td class="auto-style67"></td>
        </tr>
        <tr>
            <td class="auto-style78">
            </td>
            <td class="auto-style73">Select File :</td>
            <td class="auto-style79" colspan="2">
                <asp:FileUpload ID="FileUpload1" runat="server" Width="177px" />
            </td>
            <td class="auto-style74"></td>
            <td class="auto-style75"></td>
            <td class="auto-style76">
                &nbsp;</td>
            <td class="auto-style77">&nbsp;</td>
            <td class="auto-style80"></td>
            <td class="auto-style79"></td>
        </tr>
        <tr>
            <td class="auto-style78">
            </td>
            <td class="auto-style73">
            </td>
            <td class="auto-style52">

            </td>
            <td class="auto-style82">
                <asp:ImageButton ID="imgAddPDF" runat="server" Height="26px" ImageUrl="~/Pictures/add.ico" OnClick="imgAddPDF_Click" ToolTip="ADD Pdf File" Width="39px" />
            </td>
            <td class="auto-style74">
                <asp:ImageButton ID="imgHome" runat="server" ImageUrl="~/Pictures/icons8-back-arrow-50.png" OnClick="imgHome_Click" style="margin-bottom: 0px; margin-top: 4px;" ToolTip="Back" Height="25px" Width="47px" />
            </td>
            <td class="auto-style75">
                &nbsp;</td>
            <td class="auto-style76">
                </td>
            <td class="auto-style77"></td>
            <td class="auto-style80"></td>
            <td class="auto-style79"></td>
        </tr>
        </table>
        <table class="auto-style2">
            <tr>
                <td class="auto-style25"></td>
                <td class="auto-style25">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating">
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
                                    <asp:TextBox ID="txtREMK" runat="server" Height="16px" Text='<%# Eval("SLREMK") %>' Width="302px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblREMK" runat="server" Text='<%# Eval("SLREMK") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:ButtonField ButtonType="Image" CommandName="VIEW" HeaderText="View" ImageUrl="~/Pictures/view.ico" Text="Button" />
                            <asp:CommandField ShowEditButton="True" />
                        </Columns>
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                        <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" ForeColor="#330099" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                        <SortedAscendingCellStyle BackColor="#FEFCEB" />
                        <SortedAscendingHeaderStyle BackColor="#AF0101" />
                        <SortedDescendingCellStyle BackColor="#F6F0C0" />
                        <SortedDescendingHeaderStyle BackColor="#7E0000" />
                    </asp:GridView>
                </td>
                <td class="auto-style25"></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <table class="auto-style53">
            <tr>
                <td class="auto-style54">
                <asp:HiddenField ID="STATUSD" runat="server" />

                </td>
                <td class="auto-style55">
                <asp:HiddenField ID="PASSW" runat="server" />
                </td>
                <td class="auto-style54">
                <asp:HiddenField ID="DATEFROM" runat="server" />
                </td>
                <td class="auto-style56">
                <asp:HiddenField ID="DATETO" runat="server" />
                </td>
                <td>
                <asp:HiddenField ID="DATU" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="auto-style54">
                <asp:HiddenField ID="INVNO" runat="server" />
                </td>
                <td class="auto-style55">
                <asp:HiddenField ID="CUNO" runat="server" />
                </td>
                <td class="auto-style54">&nbsp;</td>
                <td class="auto-style56">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </form>
</body>
</html>
