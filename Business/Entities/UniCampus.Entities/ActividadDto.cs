using System;

namespace UniCampus.Entities
{
    // PROPUESTA de la Persona 1. La Persona 2 es dueña de este DTO:
    // cualquier cambio de campos debe avisarse a todo el equipo.
    public class ActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int TipoId { get; set; }
        public string TipoNombre { get; set; }      // solo lectura, viene del JOIN
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Modalidad { get; set; }       // Presencial | Virtual | Híbrida
        public string Lugar { get; set; }
        public int Cupos { get; set; }
        public string Estado { get; set; }          // Activa | Próxima | Finalizada | Inactiva
    }
}
