using System;

namespace UniCampus.Entities
{
    public class MiInscripcionDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string TipoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Lugar { get; set; }
        public string EstadoActividad { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
