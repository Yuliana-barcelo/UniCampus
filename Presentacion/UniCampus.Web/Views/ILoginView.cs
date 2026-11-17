using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface ILoginView
    {
        string Correo { get; }
        void MostrarMensaje(string mensaje);
        void IniciarSesion(UsuarioDto usuario);
    }
}
