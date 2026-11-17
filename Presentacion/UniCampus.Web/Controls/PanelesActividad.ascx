<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PanelesActividad.ascx.cs" Inherits="UniCampus.Web.Controls.PanelesActividad" %>

<div class="fila-paneles">

    <%-- 1. Detalles de la actividad --%>
    <section class="card panel-detalle">
        <header class="card-header"><h3><svg class="ico ico-azul"><use href="#i-list"/></svg> Detalles de la Actividad</h3></header>
        <dl class="detalle">
            <dt>Título:</dt><dd><asp:Label ID="lblTitulo" runat="server" /></dd>
            <dt>Tipo:</dt><dd><asp:Label ID="lblTipo" runat="server" /></dd>
            <dt>Descripción:</dt><dd><asp:Label ID="lblDescripcion" runat="server" /></dd>
            <dt>Fecha:</dt><dd><asp:Label ID="lblFechas" runat="server" /></dd>
            <dt>Hora:</dt><dd><asp:Label ID="lblHorario" runat="server" /></dd>
            <dt>Modalidad:</dt><dd><asp:Label ID="lblModalidad" runat="server" /></dd>
            <dt>Lugar:</dt><dd><asp:Label ID="lblLugar" runat="server" /></dd>
            <dt>Cupos:</dt><dd><asp:Label ID="lblCupos" runat="server" /></dd>
            <dt>Estado:</dt><dd><asp:Label ID="lblEstado" runat="server" /></dd>
        </dl>
    </section>

    <%-- 2. Estudiantes inscritos --%>
    <section class="card panel-inscritos">
        <header class="card-header">
            <h3><svg class="ico ico-azul"><use href="#i-users"/></svg> Estudiantes Inscritos (<asp:Label ID="lblTotalInscritos" runat="server" Text="0" />)</h3>
            <asp:Panel ID="pnlInscribirEstudiante" runat="server" CssClass="inscribir-estudiante">
                <asp:DropDownList ID="ddlEstudiantes" runat="server" />
                <asp:LinkButton ID="btnInscribirEstudiante" runat="server" CssClass="btn btn-verde" OnClick="btnInscribirEstudiante_Click" CausesValidation="false"><svg class="ico"><use href="#i-plus"/></svg> Inscribir estudiante</asp:LinkButton>
            </asp:Panel>
        </header>
        <div class="tabla-wrap">
            <table class="tabla tabla-compacta">
                <thead>
                    <tr><th>#</th><th>Nombre</th><th>Programa</th><th>Fecha inscripción</th><th>Acciones</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptInscritos" runat="server" OnItemCommand="rptInscritos_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td><%#: Eval("Nombre") %></td>
                                <td><%#: Eval("Programa") %></td>
                                <td><%#: Eval("FechaInscripcion", "{0:dd/MM/yyyy}") %></td>
                                <td class="acciones">
                                    <asp:LinkButton runat="server" CommandName="cancelar" CommandArgument='<%# Eval("InscripcionId") %>' CssClass="btn-accion accion-eliminar-suave" ToolTip="Cancelar inscripción" Visible='<%# PuedeAdministrar %>' OnClientClick="return confirm('¿Cancelar la inscripción de este estudiante?');" CausesValidation="false"><svg class="ico"><use href="#i-trash"/></svg></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phSinInscritos" runat="server" Visible="false">
                        <tr><td colspan="5" class="sin-datos">Aún no hay estudiantes inscritos.</td></tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </section>

    <%-- 3. Vista de la actividad como la ve el estudiante --%>
    <section class="card panel-vista">
        <header class="card-header"><h3><svg class="ico ico-azul"><use href="#i-calendar"/></svg> Vista de la Actividad</h3></header>
        <div class="vista-cuerpo">
            <div class="foto-actividad" aria-hidden="true">
                <svg viewBox="0 0 160 130" preserveAspectRatio="xMidYMid slice">
                    <rect width="160" height="130" fill="#1b2433"/>
                    <rect x="14" y="20" width="132" height="78" rx="5" fill="#0e1522" stroke="#3b4a63" stroke-width="3"/>
                    <g fill="#5ec27f"><rect x="24" y="30" width="46" height="4" rx="2"/><rect x="30" y="40" width="64" height="4" rx="2"/></g>
                    <g fill="#6aa7ff"><rect x="30" y="50" width="38" height="4" rx="2"/><rect x="24" y="60" width="72" height="4" rx="2"/></g>
                    <g fill="#f2c14e"><rect x="30" y="70" width="52" height="4" rx="2"/><rect x="24" y="80" width="30" height="4" rx="2"/></g>
                    <rect x="4" y="100" width="152" height="8" rx="3" fill="#2a3446"/>
                </svg>
            </div>
            <div class="vista-datos">
                <h4><asp:Label ID="lblVistaTitulo" runat="server" /></h4>
                <p class="badges">
                    <span class="badge badge-tipo"><svg class="ico ico-xs"><use href="#i-user"/></svg> <asp:Label ID="lblVistaTipo" runat="server" /></span>
                    <asp:Label ID="lblVistaEstado" runat="server" CssClass="badge" />
                </p>
                <p class="fila-icono"><svg class="ico"><use href="#i-calendar"/></svg> <asp:Label ID="lblVistaFechas" runat="server" /></p>
                <p class="fila-icono"><svg class="ico"><use href="#i-clock"/></svg> <asp:Label ID="lblVistaHorario" runat="server" /></p>
                <p class="fila-icono"><svg class="ico"><use href="#i-pin"/></svg> <asp:Label ID="lblVistaLugar" runat="server" /></p>
                <p class="fila-icono"><svg class="ico"><use href="#i-users"/></svg> Cupos disponibles: <asp:Label ID="lblVistaCupos" runat="server" /></p>
            </div>
        </div>
        <p class="vista-descripcion"><asp:Label ID="lblVistaDescripcion" runat="server" /></p>
        <div class="vista-accion">
            <asp:LinkButton ID="btnInscribirme" runat="server" CssClass="btn btn-primario" OnClick="btnInscribirme_Click" CausesValidation="false"><svg class="ico"><use href="#i-user-plus"/></svg> Inscribirme</asp:LinkButton>
            <asp:Label ID="lblMotivo" runat="server" CssClass="nota" />
        </div>
        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />
    </section>
</div>
