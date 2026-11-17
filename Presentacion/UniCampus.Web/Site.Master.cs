using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using uniCampus.BLL.Services;
using UniCampus.Entities;
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
            else if (ruta.EndsWith("Inscripciones.aspx", StringComparison.OrdinalIgnoreCase))
                lnkInscripciones.CssClass = "menu-item activo";
            else if (ruta.EndsWith("Perfil.aspx", StringComparison.OrdinalIgnoreCase))
                lnkPerfil.CssClass = "menu-item activo";
        }

        /// <summary>Se ejecuta después de los eventos de la página, así la campana ya refleja los cambios.</summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            var usuario = SesionUsuario.Actual;
            if (usuario == null) return;

            IList<MiInscripcionDto> proximas = new List<MiInscripcionDto>();
            try
            {
                proximas = new InscripcionService().ListarMisInscripciones(usuario.UsuarioId)
                    .Where(i => i.EstadoActividad == "Activa" || i.EstadoActividad == "Próxima")
                    .OrderBy(i => i.FechaInicio)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("No fue posible cargar las notificaciones de UniCampus: {0}", ex);
                // Si falla la consulta, la campana queda sin notificaciones; no se interrumpe la página.
            }

            lblNotificaciones.Text = proximas.Count > 0 ? proximas.Count.ToString() : string.Empty;
            rptNotificaciones.DataSource = proximas.Take(5).ToList();
            rptNotificaciones.DataBind();
            phSinNotificaciones.Visible = proximas.Count == 0;
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionUsuario.Cerrar();
            Response.Redirect(ResolveUrl("~/Login.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
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
