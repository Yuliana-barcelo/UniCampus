using System;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Security;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Login : System.Web.UI.Page, ILoginView
    {
        private LoginPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new LoginPresenter(this, new UsuarioService());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && SesionUsuario.Actual != null)
                Redirigir("~/Actividades.aspx");
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            _presenter.Ingresar();
        }

        public string Correo
        {
            get { return txtCorreo.Text.Trim(); }
        }

        public void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        public void IniciarSesion(UsuarioDto usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            SesionUsuario.Iniciar(usuario);
            Redirigir("~/Actividades.aspx");
        }

        private void Redirigir(string ruta)
        {
            Response.Redirect(ResolveUrl(ruta), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
