using System;
using System.Diagnostics;
using System.Threading;
using uniCampus.BLL.Services;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly UsuarioService _service;

        public LoginPresenter(ILoginView view, UsuarioService service)
        {
            _view = view;
            _service = service;
        }

        public void Ingresar()
        {
            try
            {
                var usuario = _service.Identificar(_view.Correo);
                if (usuario == null)
                {
                    _view.MostrarMensaje("No encontramos un usuario activo con ese correo.");
                    return;
                }
                _view.IniciarSesion(usuario);
            }
            catch (ThreadAbortException)
            {
                // No se debe convertir una interrupción legítima de ASP.NET en un error de autenticación.
                throw;
            }
            catch (Exception ex)
            {
                // El detalle técnico queda en el registro del servidor; nunca se muestra al usuario.
                Trace.TraceError("Error durante el inicio de sesión de UniCampus: {0}", ex);
                _view.MostrarMensaje("No fue posible iniciar sesión. Intente nuevamente.");
            }
        }
    }
}
