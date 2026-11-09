using System;
using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IActividadView
    {
        string Titulo { get; set; }
        int TipoId { get; set; }
        string Descripcion { get; set; }
        DateTime? FechaInicio { get; set; }
        DateTime? FechaFin { get; set; }
        TimeSpan? HoraInicio { get; set; }
        TimeSpan? HoraFin { get; set; }
        string Modalidad { get; set; }
        int? Cupos { get; set; }
        string Lugar { get; set; }
        string Estado { get; set; }

        void CargarTipos(IList<TipoActividadDto> tipos);
        void MostrarMensaje(string mensaje, bool esError);
        void LimpiarFormulario();
        void NotificarGuardado();
    }
}