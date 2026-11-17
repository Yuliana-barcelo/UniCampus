using System.Collections.Generic;

namespace UniCampus.Entities.Contracts
{
    public interface IUsuarioRepository
    {
        /// <summary>Usuario activo con ese correo, o null.</summary>
        UsuarioDto ObtenerPorCorreo(string correo);

        IList<UsuarioDto> ListarEstudiantesActivos();
    }
}
