using System;

namespace UniCampus.Web
{
    /// <summary>Muestra los datos del usuario de la sesión (solo lectura; no consulta ni modifica la base de datos).</summary>
    public partial class Perfil : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var u = Usuario;
            lblNombre.Text = Server.HtmlEncode(u.Nombre);
            lblCorreo.Text = Server.HtmlEncode(u.Correo);
            lblPrograma.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(u.Programa) ? "No registrado" : u.Programa);
            lblRol.Text = Server.HtmlEncode(u.Rol);

            string iniciales = "?";
            if (!string.IsNullOrWhiteSpace(u.Nombre))
            {
                var partes = u.Nombre.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                iniciales = partes[0].Substring(0, 1) + (partes.Length > 1 ? partes[1].Substring(0, 1) : "");
            }
            lblIniciales.Text = Server.HtmlEncode(iniciales.ToUpper());
        }
    }
}
