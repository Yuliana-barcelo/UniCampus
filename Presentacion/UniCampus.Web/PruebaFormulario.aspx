<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PruebaFormulario.aspx.cs"
    Inherits="UniCampus.Web.PruebaFormulario" %>
<%@ Register Src="~/Controls/FormularioActividad.ascx"
    TagPrefix="uc" TagName="FormularioActividad" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Prueba formulario</title>
</head>
<body>
    <form id="form1" runat="server">
        <uc:FormularioActividad ID="formulario" runat="server" />
    </form>
</body>
</html>