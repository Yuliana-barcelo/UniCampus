<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PanelesActividad.ascx.cs" Inherits="UniCampus.Web.Controls.PanelesActividad" %>

<div class="panel-detalle">
    <h3>Detalles de la Actividad</h3>
    <p><strong>Título:</strong> <asp:Label ID="lblTitulo" runat="server" /></p>
    <p><strong>Tipo:</strong> <asp:Label ID="lblTipo" runat="server" /></p>
    <p><strong>Descripción:</strong> <asp:Label ID="lblDescripcion" runat="server" /></p>
    <p><strong>Fecha:</strong> <asp:Label ID="lblFechas" runat="server" /></p>
    <p><strong>Hora:</strong> <asp:Label ID="lblHorario" runat="server" /></p>
    <p><strong>Modalidad:</strong> <asp:Label ID="lblModalidad" runat="server" /></p>
    <p><strong>Lugar:</strong> <asp:Label ID="lblLugar" runat="server" /></p>
    <p><strong>Cupos:</strong> <asp:Label ID="lblCupos" runat="server" /></p>
    <p><strong>Estado:</strong> <asp:Label ID="lblEstado" runat="server" /></p>
</div>