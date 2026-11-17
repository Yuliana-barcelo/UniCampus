<%@ Page Title="Perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="UniCampus.Web.Perfil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContenido" runat="server">
    <section class="hero hero-simple">
        <div class="hero-texto">
            <h1>Mi Perfil</h1>
            <p>Datos de la cuenta con la que iniciaste sesión.</p>
        </div>
    </section>

    <section class="card perfil-card">
        <header class="card-header">
            <h2><svg class="ico"><use href="#i-user"/></svg> Información personal</h2>
        </header>
        <div class="perfil-cuerpo">
            <span class="avatar avatar-grande"><asp:Label ID="lblIniciales" runat="server" /></span>
            <dl class="detalle perfil-datos">
                <dt>Nombre:</dt><dd><asp:Label ID="lblNombre" runat="server" /></dd>
                <dt>Correo:</dt><dd><asp:Label ID="lblCorreo" runat="server" /></dd>
                <dt>Programa:</dt><dd><asp:Label ID="lblPrograma" runat="server" /></dd>
                <dt>Rol:</dt><dd><asp:Label ID="lblRol" runat="server" /></dd>
            </dl>
        </div>
    </section>
</asp:Content>
