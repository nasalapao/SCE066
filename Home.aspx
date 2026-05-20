<%@ Page Title="Home - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" CodePage="65001" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Home - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .home-shell {
            max-width: 1120px;
            margin: 0 auto;
            padding: 0 24px;
        }

        .home-panel {
            background: #fff;
            border: 1px solid #e4ebf2;
            border-radius: 8px;
            box-shadow: 0 12px 28px rgba(15, 35, 55, 0.06);
            overflow: hidden;
        }

        .home-heading {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto;
            align-items: end;
            gap: 24px;
            padding: 28px 32px 22px;
            border-bottom: 1px solid #e8eef5;
            background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
        }

        .home-title {
            margin: 0;
            font-size: 1.75rem;
            font-weight: 700;
            color: #17212b;
        }

        .home-caption {
            margin: 8px 0 0;
            max-width: 680px;
            color: #5b6b7a;
            font-size: 0.96rem;
            line-height: 1.65;
        }

        .home-meta {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 6px 10px;
            border: 1px solid #dbe5ee;
            border-radius: 8px;
            background: #fff;
            color: #1f4e79;
            font-size: 0.82rem;
            font-weight: 700;
            white-space: nowrap;
        }

        .update-board {
            padding: 26px 32px 30px;
        }

        .update-list {
            position: relative;
            display: grid;
            gap: 0;
        }

        .update-list::before {
            content: "";
            position: absolute;
            top: 8px;
            bottom: 8px;
            left: 118px;
            width: 1px;
            background: #dbe5ee;
        }

        .update-item {
            position: relative;
            display: grid;
            grid-template-columns: 120px minmax(0, 1fr);
            gap: 24px;
            padding: 0 0 22px;
        }

        .update-item:last-child {
            padding-bottom: 0;
        }

        .update-date {
            color: #1f4e79;
            font-size: 0.88rem;
            font-weight: 700;
            line-height: 1.4;
            white-space: nowrap;
        }

        .update-content {
            position: relative;
            min-width: 0;
            padding-left: 22px;
        }

        .update-content::before {
            content: "";
            position: absolute;
            top: 6px;
            left: -29px;
            width: 11px;
            height: 11px;
            border: 2px solid #1f4e79;
            border-radius: 50%;
            background: #fff;
        }

        .update-item-title {
            margin: 0 0 6px;
            color: #17212b;
            font-size: 1.02rem;
            font-weight: 700;
            line-height: 1.45;
        }

        .update-detail {
            margin: 0;
            color: #425466;
            font-size: 0.94rem;
            line-height: 1.75;
            overflow-wrap: anywhere;
        }

        .update-message {
            display: block;
            padding: 14px 16px;
            border-radius: 8px;
            font-size: 0.9rem;
            background: #f7f9fb;
            border: 1px solid #e1e8f0;
            color: #5b6b7a;
        }

        .update-message.is-error {
            background: #fff3f3;
            border-color: #ffd4d4;
            color: #a61b1b;
        }

        @media (max-width: 767.98px) {
            .home-shell {
                padding: 0 16px;
            }

            .home-heading {
                grid-template-columns: 1fr;
                gap: 14px;
                padding: 24px 22px 20px;
            }

            .home-title {
                font-size: 1.45rem;
            }

            .home-meta {
                justify-self: start;
            }

            .update-board {
                padding: 22px;
            }

            .update-list::before {
                left: 5px;
            }

            .update-item {
                grid-template-columns: 1fr;
                gap: 6px;
                padding-left: 26px;
            }

            .update-content {
                padding-left: 0;
            }

            .update-content::before {
                left: -25px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="home-shell">
        <section class="home-panel" aria-labelledby="programUpdateTitle">
            <div class="home-heading">
                <div>
                    <h1 id="programUpdateTitle" class="home-title">รายการอัปเดตโปรแกรม</h1>
                    <p class="home-caption">สรุปรายการปรับปรุงล่าสุด เพื่อให้ผู้ใช้งานทราบว่าโปรแกรมมีการแก้ไขอะไร และมีผลตั้งแต่วันที่ใด</p>
                </div>
                <div class="home-meta">Latest updates</div>
            </div>

            <div class="update-board">
                <asp:Label ID="lbUpdateMessage" runat="server" CssClass="update-message" Visible="false" />
                <asp:Repeater ID="rptProgramUpdate" runat="server">
                    <HeaderTemplate>
                        <div class="update-list">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <article class="update-item">
                            <div class="update-date"><%# FormatChangeDate(Eval("CHANGE_DATE")) %></div>
                            <div class="update-content">
                                <h2 class="update-item-title"><%# Server.HtmlEncode(Convert.ToString(Eval("CHANGE_TITLE"))) %></h2>
                                <p class="update-detail"><%# Server.HtmlEncode(Convert.ToString(Eval("CHANGE_DETAIL"))) %></p>
                            </div>
                        </article>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </section>
    </div>
</asp:Content>
