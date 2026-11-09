using System;

namespace UniCampus.Entities
{
    public class InscripcionDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Estado { get; set; }
    }
}