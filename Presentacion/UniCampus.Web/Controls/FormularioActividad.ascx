<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="FormularioActividad.ascx.cs"
    Inherits="UniCampus.Web.Controls.FormularioActividad" %>

<div class="formulario-actividad">
    <h2>Nueva Actividad Académica</h2>
    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />

    <div>
        <label>Título de la actividad *</label>
        <asp:TextBox ID="txtTitulo" runat="server" MaxLength="120" />
    </div>
    <div>
        <label>Tipo de actividad *</label>
        <asp:DropDownList ID="ddlTipo" runat="server" />
    </div>
    <div>
        <label>Descripción *</label>
        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" />
    </div>
    <div>
        <label>Fecha de inicio *</label>
        <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" />
        <label>Fecha de fin *</label>
        <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" />
    </div>
    <div>
        <label>Hora de inicio *</label>
        <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
        <label>Hora de fin *</label>
        <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
    </div>
    <div>
        <label>Modalidad *</label>
        <asp:DropDownList ID="ddlModalidad" runat="server">
            <asp:ListItem Text="Presencial" Value="Presencial" />
            <asp:ListItem Text="Virtual" Value="Virtual" />
            <asp:ListItem Text="Híbrida" Value="Híbrida" />
        </asp:DropDownList>
    </div>
    <div>
        <label>Cupos disponibles *</label>
        <asp:TextBox ID="txtCupos" runat="server" TextMode="Number" />
        <label>Lugar *</label>
        <asp:TextBox ID="txtLugar" runat="server" MaxLength="150" />
    </div>
    <div>
        <label>Estado *</label>
        <asp:DropDownList ID="ddlEstado" runat="server">
            <asp:ListItem Text="Activa" Value="Activa" />
            <asp:ListItem Text="Próxima" Value="Próxima" />
            <asp:ListItem Text="Finalizada" Value="Finalizada" />
        </asp:DropDownList>
    </div>

    <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
        OnClick="btnGuardar_Click" CausesValidation="false" />
    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"
        OnClick="btnLimpiar_Click" CausesValidation="false" />
</div>