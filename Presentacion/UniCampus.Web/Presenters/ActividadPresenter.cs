using System;
using System.Diagnostics;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>Presenter del formulario: crear, editar y limpiar.</summary>
    public class ActividadPresenter
    {
        private const string SinPermiso = "No tienes permisos para gestionar actividades.";
        private readonly IActividadView _view;
        private readonly ActividadService _service;

        public ActividadPresenter(IActividadView view, ActividadService service)
        {
            _view = view;
            _service = service;
        }

        public void CargarTipos()
        {
            try
            {
                _view.CargarTipos(_service.ObtenerTipos());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible cargar los tipos de actividad.", true);
            }
        }

        public void CargarParaEdicion(int actividadId)
        {
            if (!Permisos.PuedeGestionarActividades(_view.RolUsuario))
            {
                _view.MostrarMensaje(SinPermiso, true);
                return;
            }

            try
            {
                var a = _service.ObtenerPorId(actividadId);
                if (a == null)
                {
                    _view.MostrarMensaje("La actividad no existe o fue desactivada.", true);
                    return;
                }

                _view.ActividadId = a.ActividadId;
                _view.Titulo = a.Titulo;
                _view.TipoId = a.TipoId;
                _view.Descripcion = a.Descripcion;
                _view.FechaInicio = a.FechaInicio;
                _view.FechaFin = a.FechaFin;
                _view.HoraInicio = a.HoraInicio;
                _view.HoraFin = a.HoraFin;
                _view.Modalidad = a.Modalidad;
                _view.Cupos = a.Cupos;
                _view.Lugar = a.Lugar;
                _view.Estado = a.Estado;
                _view.MostrarMensaje("Editando la actividad seleccionada.", false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible cargar la actividad para editarla.", true);
            }
        }

        public void Guardar()
        {
            if (!Permisos.PuedeGestionarActividades(_view.RolUsuario))
            {
                _view.MostrarMensaje(SinPermiso, true);
                return;
            }

            // El DTO no admite fechas, horas ni cupos vacíos: se revisa antes de armarlo.
            if (!_view.FechaInicio.HasValue || !_view.FechaFin.HasValue ||
                !_view.HoraInicio.HasValue || !_view.HoraFin.HasValue ||
                !_view.Cupos.HasValue)
            {
                _view.MostrarMensaje("Complete las fechas, las horas y los cupos con valores válidos.", true);
                return;
            }

            try
            {
                var dto = new ActividadDto
                {
                    ActividadId = _view.ActividadId,
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

                int idGuardado;
                if (dto.ActividadId > 0)
                {
                    var r = _service.Actualizar(dto);
                    if (!r.Exito)
                    {
                        _view.MostrarMensaje(r.Mensaje, true);
                        return;
                    }
                    idGuardado = dto.ActividadId;
                    _view.LimpiarFormulario();
                    _view.MostrarMensaje(r.Mensaje, false);
                }
                else
                {
                    var r = _service.Guardar(dto);
                    if (!r.Exitoso)
                    {
                        _view.MostrarMensaje(string.Join(" ", r.Errores), true);
                        return;
                    }
                    idGuardado = r.ActividadId.Value;
                    _view.LimpiarFormulario();
                    _view.MostrarMensaje("Actividad guardada correctamente.", false);
                }

                _view.NotificarGuardado(idGuardado);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible guardar la actividad. Intente nuevamente.", true);
            }
        }

        public void Limpiar()
        {
            _view.LimpiarFormulario();
            _view.MostrarMensaje(string.Empty, false);
        }
    }
}
