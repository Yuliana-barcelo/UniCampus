<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="FormularioActividad.ascx.cs"
    Inherits="UniCampus.Web.Controls.FormularioActividad" %>

<section class="card card-formulario">
    <header class="card-header">
        <h2><svg class="ico ico-azul"><use href="#i-plus-circle"/></svg> <asp:Label ID="lblTituloForm" runat="server" Text="Nueva Actividad Académica" /></h2>
    </header>

    <div class="card-cuerpo">
        <div class="form-grid">
            <div class="campo c7">
                <label for="<%= txtTitulo.ClientID %>">Título de la actividad <span class="req">*</span></label>
                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="120" />
            </div>
            <div class="campo c5">
                <label for="<%= ddlTipo.ClientID %>">Tipo de actividad <span class="req">*</span></label>
                <asp:DropDownList ID="ddlTipo" runat="server" />
            </div>

            <div class="campo c6 alto">
                <label for="<%= txtDescripcion.ClientID %>">Descripción <span class="req">*</span></label>
                <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="4" MaxLength="1000" />
            </div>
            <div class="campo c3">
                <label for="<%= txtFechaInicio.ClientID %>">Fecha de inicio <span class="req">*</span></label>
                <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" />
            </div>
            <div class="campo c3">
                <label for="<%= txtFechaFin.ClientID %>">Fecha de fin <span class="req">*</span></label>
                <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" />
            </div>

            <div class="campo c4">
                <label for="<%= txtHoraInicio.ClientID %>">Hora de inicio <span class="req">*</span></label>
                <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
            </div>
            <div class="campo c4">
                <label for="<%= txtHoraFin.ClientID %>">Hora de fin <span class="req">*</span></label>
                <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
            </div>
            <div class="campo c4">
                <label for="<%= ddlModalidad.ClientID %>">Modalidad <span class="req">*</span></label>
                <asp:DropDownList ID="ddlModalidad" runat="server">
                    <asp:ListItem Text="Presencial" Value="Presencial" />
                    <asp:ListItem Text="Virtual" Value="Virtual" />
                    <asp:ListItem Text="Híbrida" Value="Híbrida" />
                </asp:DropDownList>
            </div>

            <div class="campo c4">
                <label for="<%= txtCupos.ClientID %>">Cupos disponibles <span class="req">*</span></label>
                <asp:TextBox ID="txtCupos" runat="server" TextMode="Number" />
            </div>
            <div class="campo c4">
                <label for="<%= txtLugar.ClientID %>">Lugar <span class="req">*</span></label>
                <asp:TextBox ID="txtLugar" runat="server" MaxLength="150" />
            </div>
            <div class="campo c4">
                <label for="<%= ddlEstado.ClientID %>">Estado <span class="req">*</span></label>
                <asp:DropDownList ID="ddlEstado" runat="server">
                    <asp:ListItem Text="Activa" Value="Activa" />
                    <asp:ListItem Text="Próxima" Value="Próxima" />
                    <asp:ListItem Text="Finalizada" Value="Finalizada" />
                </asp:DropDownList>
            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />

        <div class="botones">
            <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primario" OnClick="btnGuardar_Click" CausesValidation="false"><svg class="ico"><use href="#i-save"/></svg> <asp:Label ID="lblTextoGuardar" runat="server" Text="Guardar" /></asp:LinkButton>
            <asp:LinkButton ID="btnLimpiar" runat="server" CssClass="btn btn-secundario" OnClick="btnLimpiar_Click" CausesValidation="false"><svg class="ico"><use href="#i-refresh"/></svg> Limpiar</asp:LinkButton>
        </div>
    </div>
</section>
