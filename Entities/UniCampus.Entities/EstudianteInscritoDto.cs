using System;

namespace UniCampus.Entities
{
    public class EstudianteInscritoDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Programa { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}