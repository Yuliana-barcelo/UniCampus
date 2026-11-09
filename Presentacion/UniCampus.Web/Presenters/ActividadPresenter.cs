using System;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class ActividadPresenter
    {
        private readonly IActividadView _view;
        private readonly ActividadService _service;

        public ActividadPresenter(IActividadView view, ActividadService service)
        {
            _view = view;
            _service = service;
        }

        public void Guardar()
        {
            // El DTO no admite fechas, horas ni cupos vacíos,
            // así que esto se revisa antes de armarlo.
            if (!_view.FechaInicio.HasValue || !_view.FechaFin.HasValue ||
                !_view.HoraInicio.HasValue || !_view.HoraFin.HasValue ||
                !_view.Cupos.HasValue)
            {
                _view.MostrarMensaje("Complete las fechas, las horas y los cupos.", true);
                return;
            }

            try
            {
                var dto = new ActividadDto
                {
                    Titulo = _view.Titulo,
                    Descripcion = _view.Descripcion,
                    TipoId = _view.TipoId,
                    FechaInicio = _view.FechaInicio.Value,
                    FechaFin = _view.FechaFin.Value,
                    HoraInicio = _view.HoraInicio.Value,
                    HoraFin = _view.HoraFin.Value,
                    Modalidad = _view.Modalidad,
                    Lugar = _view.Lugar,
                    Cupos = _view.Cupos.Value,
                    Estado = _view.Estado
                };

                var resultado = _service.Guardar(dto);

                if (!resultado.Exitoso)
                {
                    _view.MostrarMensaje(string.Join(" ", resultado.Errores), true);
                    return;
                }

                _view.MostrarMensaje("Actividad guardada correctamente.", false);
                _view.LimpiarFormulario();
                _view.NotificarGuardado();
            }
            catch (Exception)
            {
                // Aquí iría el registro del error técnico en un log.
                _view.MostrarMensaje("No fue posible guardar la actividad. Intente nuevamente.", true);
            }
        }

        public void Limpiar()
        {
            _view.LimpiarFormulario();
        }
    }
}