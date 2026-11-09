using System;
using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IInscripcionesView
    {
        int ActividadId { get; set; }

        event EventHandler InscribirmeSolicitado;

        void MostrarEstudiantes(IList<EstudianteInscritoDto> estudiantes);
        void MostrarVistaEstudiante(string titulo, string tipo, string estado,
                                    string fechas, string horario, string lugar,
                                    int cuposDisponibles, int cupoTotal, string descripcion);
        void MostrarMensaje(string mensaje, bool esError);
    }
}