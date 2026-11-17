<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UniCampus.Web.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>UniCampus | Ingresar</title>
    <link href="~/Content/site.css?v=4" rel="stylesheet" runat="server" />
</head>
<body class="pagina-login">
    <form id="form1" runat="server">
        <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnIngresar" CssClass="login-card">
            <svg class="logo logo-grande" viewBox="0 0 64 48" aria-hidden="true">
                <polygon points="32,4 62,18 32,32 2,18" fill="#1d3a8a"/>
                <path d="M14 26v10c0 4 8 8 18 8s18-4 18-8V26l-18 9z" fill="#4a6cc7"/>
                <line x1="58" y1="20" x2="58" y2="36" stroke="#1d3a8a" stroke-width="3" stroke-linecap="round"/>
            </svg>
            <h1>UniCampus</h1>
            <p class="subtitulo">Sistema de Gestión Académica</p>

            <label for="<%= txtCorreo.ClientID %>">Correo institucional</label>
            <asp:TextBox ID="txtCorreo" runat="server" MaxLength="150" TextMode="Email" />

            <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje error" />

            <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn btn-primario btn-bloque" OnClick="btnIngresar_Click" />
        </asp:Panel>
    </form>
</body>
</html>
