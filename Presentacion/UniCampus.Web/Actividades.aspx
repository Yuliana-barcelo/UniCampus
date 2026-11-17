<%@ Page Title="Actividades Académicas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="UniCampus.Web.Actividades" %>
<%@ Register Src="~/Controls/FormularioActividad.ascx" TagPrefix="uc" TagName="FormularioActividad" %>
<%@ Register Src="~/Controls/PanelesActividad.ascx" TagPrefix="uc" TagName="Paneles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContenido" runat="server">

    <section class="hero">
        <div class="hero-texto">
            <h1>Registro de Actividades Académicas</h1>
            <p>Administra, registra y consulta las actividades académicas de la institución.</p>

            <div class="kpis">
                <div class="kpi kpi-azul">
                    <svg class="ico ico-xl"><use href="#i-calendar"/></svg>
                    <div><strong><asp:Label ID="lblTotalActividades" runat="server" Text="0" /></strong><span>Total de actividades</span></div>
                </div>
                <div class="kpi kpi-verde">
                    <svg class="ico ico-xl"><use href="#i-users"/></svg>
                    <div><strong><asp:Label ID="lblTotalInscritos" runat="server" Text="0" /></strong><span>Estudiantes inscritos</span></div>
                </div>
                <div class="kpi kpi-amarillo">
                    <svg class="ico ico-xl"><use href="#i-clock"/></svg>
                    <div><strong><asp:Label ID="lblProximas" runat="server" Text="0" /></strong><span>Actividades próximas</span></div>
                </div>
                <div class="kpi kpi-rojo">
                    <svg class="ico ico-xl"><use href="#i-check"/></svg>
                    <div><strong><asp:Label ID="lblFinalizadas" runat="server" Text="0" /></strong><span>Actividades finalizadas</span></div>
                </div>
            </div>
        </div>

        <%-- Ilustración decorativa. Para usar una imagen propia: Content/img/estudiantes.png y descomentar en site.css --%>
        <div class="hero-ilustracion" aria-hidden="true">
            <svg viewBox="0 0 420 200" preserveAspectRatio="xMaxYMax meet">
                <circle cx="70" cy="90" r="60" fill="#dbe8ff"/>
                <g><circle cx="105" cy="62" r="24" fill="#f2c9a5"/><path d="M82 56c4-22 40-26 48-2-10-6-30-8-48 2z" fill="#1c2340"/><path d="M60 200c0-48 20-92 45-92s45 44 45 92z" fill="#2f5bb7"/></g>
                <g><circle cx="215" cy="52" r="26" fill="#f0bf98"/><path d="M188 52c-6-34 52-40 54 0-8-14-40-18-54 0z" fill="#1c2340"/><path d="M175 200c0-52 18-96 40-96s40 44 40 96z" fill="#f6c443"/></g>
                <g><circle cx="335" cy="56" r="26" fill="#e9b48b"/><path d="M310 52c2-30 50-30 52 0-12-10-38-10-52 0z" fill="#1c2340"/><circle cx="335" cy="58" r="14" fill="none" stroke="#1c2340" stroke-width="3"/><path d="M290 200c0-50 20-94 45-94s45 44 45 94z" fill="#2f9e6b"/></g>
            </svg>
        </div>
    </section>

    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />

    <div id="gridPrincipal" runat="server" class="grid-principal">
        <uc:FormularioActividad ID="ucFormulario" runat="server" />

        <section class="card card-listado">
            <header class="card-header">
                <h2><svg class="ico"><use href="#i-list"/></svg> Listado de Actividades Académicas</h2>
                <asp:Panel ID="pnlBuscar" runat="server" DefaultButton="btnBuscar" CssClass="buscador">
                    <asp:TextBox ID="txtBuscar" runat="server" MaxLength="100" placeholder="Buscar actividad..." />
                    <asp:LinkButton ID="btnBuscar" runat="server" CssClass="btn-buscar" ToolTip="Buscar" OnClick="btnBuscar_Click" CausesValidation="false"><svg class="ico"><use href="#i-search"/></svg></asp:LinkButton>
                </asp:Panel>
            </header>

            <div class="tabla-wrap">
                <table class="tabla">
                    <thead>
                        <tr>
                            <th>#</th><th>Título</th><th>Tipo</th><th>Fecha</th><th>Cupos</th><th>Estado</th><th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptActividades" runat="server" OnItemCommand="rptActividades_ItemCommand">
                            <ItemTemplate>
                                <tr class='<%# EsSeleccionada((int)Eval("ActividadId")) ? "fila-seleccionada" : "" %>'>
                                    <td><%# NumeroFila(Container.ItemIndex) %></td>
                                    <td><%#: Eval("Titulo") %></td>
                                    <td><%#: Eval("TipoNombre") %></td>
                                    <td><%#: Eval("FechaInicio", "{0:dd/MM/yyyy}") %></td>
                                    <td><%#: Eval("Cupos") %></td>
                                    <td><span class='badge estado-<%# ClaseEstado((string)Eval("Estado")) %>'><%#: Eval("Estado") %></span></td>
                                    <td class="acciones">
                                        <asp:LinkButton runat="server" CommandName="ver" CommandArgument='<%# Eval("ActividadId") %>' CssClass="btn-accion accion-ver" ToolTip="Ver detalle" CausesValidation="false"><svg class="ico"><use href="#i-eye"/></svg></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="editar" CommandArgument='<%# Eval("ActividadId") %>' CssClass="btn-accion accion-editar" ToolTip="Editar" Visible='<%# PuedeGestionar %>' CausesValidation="false"><svg class="ico"><use href="#i-pencil"/></svg></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="eliminar" CommandArgument='<%# Eval("ActividadId") %>' CssClass="btn-accion accion-eliminar" ToolTip="Desactivar" Visible='<%# PuedeGestionar %>' OnClientClick="return confirm('¿Desactivar esta actividad? Dejará de mostrarse en el listado.');" CausesValidation="false"><svg class="ico"><use href="#i-trash"/></svg></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="phSinResultados" runat="server" Visible="false">
                            <tr><td colspan="7" class="sin-datos">No se encontraron actividades.</td></tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>

            <footer class="tabla-pie">
                <asp:Label ID="lblRango" runat="server" CssClass="rango" />
                <nav class="paginacion">
                    <asp:Repeater ID="rptPaginas" runat="server" OnItemCommand="rptPaginas_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="pagina" CommandArgument='<%# Eval("Pagina") %>' Enabled='<%# !(bool)Eval("Deshabilitada") %>' CssClass='<%# (bool)Eval("Activa") ? "pag activa" : "pag" %>' CausesValidation="false"><%#: Eval("Texto") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </nav>
            </footer>
        </section>
    </div>

    <uc:Paneles ID="ucPaneles" runat="server" />

</asp:Content>
