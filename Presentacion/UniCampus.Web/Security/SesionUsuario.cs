using System.Web;
using UniCampus.Entities;

namespace UniCampus.Web.Security
{
    /// <summary>
    /// Identidad del usuario guardada en la sesión del servidor.
    /// El UsuarioId y el rol salen SIEMPRE de aquí, nunca de un control de la página.
    /// </summary>
    public static class SesionUsuario
    {
        private const string Clave = "UniCampus.UsuarioActual";

        public static UsuarioDto Actual
        {
            get
            {
                var ctx = HttpContext.Current;
                return ctx == null || ctx.Session == null ? null : ctx.Session[Clave] as UsuarioDto;
            }
        }

        public static void Iniciar(UsuarioDto usuario)
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session[Clave] = usuario;
        }

        public static void Cerrar()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}

namespace UniCampus.Web
{
    /// <summary>Página que exige sesión iniciada; si no hay usuario, redirige al login.</summary>
    public abstract class PaginaBase : System.Web.UI.Page
    {
        protected UniCampus.Entities.UsuarioDto Usuario
        {
            get { return UniCampus.Web.Security.SesionUsuario.Actual; }
        }

        protected override void OnPreInit(System.EventArgs e)
        {
            if (UniCampus.Web.Security.SesionUsuario.Actual == null)
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }
            base.OnPreInit(e);
        }
    }
}
