using System;
using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IInscripcionesView
    {
        int ActividadId { get; set; }

        /// <summary>Estudiante elegido en la lista desplegable de "Inscribir estudiante".</summary>
        int EstudianteSeleccionadoId { get; }

        event EventHandler InscribirmeSolicitado;
        event EventHandler InscribirEstudianteSolicitado;
        event EventHandler<IdEventArgs> CancelarInscripcionSolicitada;

        void MostrarEstudiantes(IList<EstudianteInscritoDto> estudiantes, bool puedeAdministrar);
        void CargarEstudiantesDisponibles(IList<UsuarioDto> estudiantes);
        void MostrarVistaEstudiante(string titulo, string tipo, string estado,
                                    string fechas, string horario, string lugar,
                                    int cuposDisponibles, int cupoTotal, string descripcion,
                                    bool puedeInscribirme, string motivo);
        void MostrarMensaje(string mensaje, bool esError);
        void LimpiarInscripciones();
    }
}
