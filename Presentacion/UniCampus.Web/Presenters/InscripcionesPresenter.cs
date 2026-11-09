using System;
using System.Collections.Generic;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class InscripcionesPresenter
    {
        private readonly IInscripcionesView _vista;

        public InscripcionesPresenter(IInscripcionesView vista)
        {
            _vista = vista;
            _vista.InscribirmeSolicitado += AlSolicitarInscripcion;
        }

        public void CargarEstudiantes(int actividadId)
        {
            // TEMPORAL: datos de prueba. Luego se reemplaza por InscripcionService.
            var lista = new List<EstudianteInscritoDto>
            {
                new EstudianteInscritoDto { Nombre = "Ana Torres", Programa = "Ingeniería de Sistemas", FechaInscripcion = new DateTime(2025, 5, 10) },
                new EstudianteInscritoDto { Nombre = "Carlos Pérez", Programa = "Diseño Gráfico", FechaInscripcion = new DateTime(2025, 5, 11) },
                new EstudianteInscritoDto { Nombre = "Laura Gómez", Programa = "Administración", FechaInscripcion = new DateTime(2025, 5, 12) }
            };

            _vista.MostrarEstudiantes(lista);
        }

        public void CargarVistaEstudiante(int actividadId)
        {
            // TEMPORAL: datos de prueba.
            _vista.ActividadId = actividadId;
            _vista.MostrarVistaEstudiante(
                "Taller de Programación Web", "Taller", "Activa",
                "15/05/2025 - 16/05/2025", "08:00 - 12:00", "Laboratorio 3",
                27, 30,
                "Espacio práctico para reforzar conceptos de programación web con ASP.NET.");
        }

        private void AlSolicitarInscripcion(object sender, EventArgs e)
        {
            // TEMPORAL: respuesta simulada. Luego aquí se llama a InscripcionService.Inscribir(...)
            _vista.MostrarMensaje(
                "Inscripción simulada para la actividad " + _vista.ActividadId + ". Aún no se guarda en la base de datos.",
                false);
        }
    }
}