using System;
using UniCampus.Web.Security;

namespace UniCampus.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = SesionUsuario.Actual;
            if (usuario != null)
            {
                // Label no codifica el texto: se codifica aquí para evitar XSS.
                lblNombreUsuario.Text = Server.HtmlEncode(usuario.Nombre);
                lblRolUsuario.Text = Server.HtmlEncode(usuario.Rol);
                lblIniciales.Text = Server.HtmlEncode(Iniciales(usuario.Nombre));
            }

            string ruta = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            if (ruta.EndsWith("Actividades.aspx", StringComparison.OrdinalIgnoreCase))
                lnkActividades.CssClass = "menu-item activo";
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionUsuario.Cerrar();
            Response.Redirect("~/Login.aspx");
        }

        private static string Iniciales(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";
            var partes = nombre.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string r = partes[0].Substring(0, 1);
            if (partes.Length > 1) r += partes[1].Substring(0, 1);
            return r.ToUpper();
        }
    }
}