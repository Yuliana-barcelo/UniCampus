using System;

namespace UniCampus.Entities
{
    [Serializable]
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Programa { get; set; }
        public string Rol { get; set; }   // Estudiante | Administrador | Organizador
    }
}