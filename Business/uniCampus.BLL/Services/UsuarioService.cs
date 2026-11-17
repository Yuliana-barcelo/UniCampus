using System;
using System.Collections.Generic;
using UniCampus.DAL.Repositories;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace uniCampus.BLL.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository repository;

        public UsuarioService() : this(new UsuarioRepository()) { }

        public UsuarioService(IUsuarioRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            this.repository = repository;
        }

        /// <summary>Identifica un usuario activo por correo. Devuelve null si no existe o el correo no es válido.</summary>
        public UsuarioDto Identificar(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return null;
            correo = correo.Trim();
            if (correo.Length > 150 || correo.IndexOf('@') < 1) return null;
            return repository.ObtenerPorCorreo(correo);
        }

        public IList<UsuarioDto> ListarEstudiantes()
        {
            return repository.ListarEstudiantesActivos();
        }
    }

    /// <summary>Reglas de rol centralizadas en negocio. El rol siempre viene de la sesión, nunca de un control.</summary>
    public static class Permisos
    {
        public const string Estudiante = "Estudiante";
        public const string Administrador = "Administrador";
        public const string Organizador = "Organizador";

        public static bool PuedeGestionarActividades(string rol)
        {
            return Es(rol, Administrador) || Es(rol, Organizador);
        }

        public static bool PuedeInscribirOtros(string rol)
        {
            return PuedeGestionarActividades(rol);
        }

        private static bool Es(string rol, string esperado)
        {
            return string.Equals(rol, esperado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
