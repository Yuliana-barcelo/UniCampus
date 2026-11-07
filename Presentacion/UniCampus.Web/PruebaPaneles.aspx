<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PruebaPaneles.aspx.cs" Inherits="UniCampus.Web.PruebaPaneles" %>
<%@ Register Src="~/Controls/PanelesActividad.ascx" TagPrefix="uc" TagName="Paneles" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Prueba de paneles</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <uc:Paneles ID="ucPaneles" runat="server" />
        </div>
    </form>
</body>
</html>