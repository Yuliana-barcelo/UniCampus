<%@ Page Title="Mis Inscripciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inscripciones.aspx.cs" Inherits="UniCampus.Web.Inscripciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContenido" runat="server">
    <section class="hero hero-simple">
        <div class="hero-texto">
            <h1>Mis Inscripciones</h1>
            <p>Consulta las actividades en las que estás inscrito y cancela las que ya no necesites.</p>
        </div>
    </section>

    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />

    <section class="card">
        <header class="card-header">
            <h2><svg class="ico"><use href="#i-list"/></svg> Actividades inscritas</h2>
        </header>
        <div class="tabla-wrap">
            <table class="tabla">
                <thead>
                    <tr><th>#</th><th>Actividad</th><th>Tipo</th><th>Fecha</th><th>Lugar</th><th>Estado</th><th>Inscrito el</th><th>Acciones</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptMisInscripciones" runat="server" OnItemCommand="rptMisInscripciones_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td><%#: Eval("Titulo") %></td>
                                <td><%#: Eval("TipoNombre") %></td>
                                <td><%#: Eval("FechaInicio", "{0:dd/MM/yyyy}") %></td>
                                <td><%#: Eval("Lugar") %></td>
                                <td><span class='badge estado-<%# ClaseEstado((string)Eval("EstadoActividad")) %>'><%#: Eval("EstadoActividad") %></span></td>
                                <td><%#: Eval("FechaInscripcion", "{0:dd/MM/yyyy}") %></td>
                                <td class="acciones">
                                    <asp:LinkButton runat="server" CommandName="cancelar" CommandArgument='<%# Eval("InscripcionId") %>' CssClass="btn-accion accion-eliminar" ToolTip="Cancelar inscripción" OnClientClick="return confirm('¿Cancelar tu inscripción a esta actividad?');" CausesValidation="false"><svg class="ico"><use href="#i-trash"/></svg></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phSinInscripciones" runat="server" Visible="false">
                        <tr><td colspan="8" class="sin-datos">Todavía no estás inscrito en ninguna actividad.</td></tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </section>
</asp:Content>
