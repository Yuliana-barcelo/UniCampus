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

<div class="panel-inscritos">
    <h3>Estudiantes Inscritos (<asp:Label ID="lblTotalInscritos" runat="server" />)</h3>
    <table>
        <thead>
            <tr>
                <th>#</th>
                <th>Nombre</th>
                <th>Programa</th>
                <th>Fecha inscripción</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptInscritos" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# Container.ItemIndex + 1 %></td>
                        <td><%# Eval("Nombre") %></td>
                        <td><%# Eval("Programa") %></td>
                        <td><%# Eval("FechaInscripcion", "{0:dd/MM/yyyy}") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>

<div class="panel-vista-estudiante">
    <h3>Vista de la Actividad (como la ve el estudiante)</h3>
    <h4><asp:Label ID="lblVistaTitulo" runat="server" /></h4>
    <p>
        <asp:Label ID="lblVistaTipo" runat="server" CssClass="badge badge-tipo" />
        <asp:Label ID="lblVistaEstado" runat="server" CssClass="badge" />
    </p>
    <p><strong>Fechas:</strong> <asp:Label ID="lblVistaFechas" runat="server" /></p>
    <p><strong>Horario:</strong> <asp:Label ID="lblVistaHorario" runat="server" /></p>
    <p><strong>Lugar:</strong> <asp:Label ID="lblVistaLugar" runat="server" /></p>
    <p><strong>Cupos disponibles:</strong> <asp:Label ID="lblVistaCupos" runat="server" /></p>
    <p><asp:Label ID="lblVistaDescripcion" runat="server" /></p>
    <asp:Button ID="btnInscribirme" runat="server" Text="Inscribirme"
                CssClass="btn-inscribirme" OnClick="btnInscribirme_Click" />
    <asp:Label ID="lblMensaje" runat="server" />
</div>