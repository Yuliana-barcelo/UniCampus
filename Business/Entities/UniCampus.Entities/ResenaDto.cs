using System;

namespace UniCampus.Entities
{
    public class ResenaDto
    {
        public int ResenaId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int Calificacion { get; set; }   // 1 a 5
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}