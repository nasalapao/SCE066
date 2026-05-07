<%@ Page Title="Home - Shipping Document Control" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Home - Shipping Document Control
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .home-shell {
            max-width: 960px;
            margin: 0 auto;
            padding: 0 24px;
        }

        .home-panel {
            background: #fff;
            border: 1px solid #e8eef5;
            border-radius: 12px;
            box-shadow: 0 12px 30px rgba(15, 35, 55, 0.08);
            padding: 28px;
        }

        .home-title {
            margin: 0 0 8px;
            font-size: 1.55rem;
            font-weight: 700;
        }

        .home-text {
            margin: 0 0 22px;
            color: #5b6b7a;
            font-size: 0.95rem;
        }

        .home-link {
            display: inline-flex;
            align-items: center;
            min-height: 42px;
            padding: 9px 14px;
            border-radius: 8px;
            background: #1f4e79;
            color: #fff;
            font-weight: 700;
            text-decoration: none;
        }

        .home-link:hover {
            background: #173a59;
            color: #fff;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="home-shell">
        <section class="home-panel">
            <h1 class="home-title">Shipping Document Control</h1>
            <p class="home-text">เลือกเมนูเพื่อเข้าใช้งานระบบ</p>
            <a class="home-link" href="<%= ResolveUrl("~/SCE066.aspx") %>">เปิดหน้า SCE066</a>
        </section>
    </div>
</asp:Content>
