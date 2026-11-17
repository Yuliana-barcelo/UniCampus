using System;
using System.Diagnostics;
using uniCampus.BLL.Services;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>Presenter de Actividades.aspx: KPIs, búsqueda, paginación y baja lógica.</summary>
    public class ActividadesPresenter
    {
        public const int TamPagina = 5;

        private readonly IActividadesView _view;
        private readonly ActividadService _service;

        public ActividadesPresenter(IActividadesView view, ActividadService service)
        {
            _view = view;
            _service = service;
        }

        public void Cargar()
        {
            CargarResumen();
            CargarListado();
        }

        public void CargarResumen()
        {
            try
            {
                _view.MostrarResumen(_service.ObtenerResumen(DateTime.Today));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadesPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible cargar el resumen de actividades.", true);
            }
        }

        public void CargarListado()
        {
            try
            {
                var pagina = _service.Listar(_view.Busqueda, _view.PaginaActual, TamPagina);

                // Si se desactivó el último registro de la última página, retrocede una.
                int totalPaginas = Math.Max(1, (int)Math.Ceiling(pagina.Total / (double)TamPagina));
                if (_view.PaginaActual > totalPaginas)
                {
                    _view.PaginaActual = totalPaginas;
                    pagina = _service.Listar(_view.Busqueda, totalPaginas, TamPagina);
                }

                if (_view.ActividadSeleccionadaId == 0 && pagina.Items.Count > 0)
                    _view.ActividadSeleccionadaId = pagina.Items[0].ActividadId;

                _view.MostrarActividades(pagina.Items, pagina.Total, _view.PaginaActual, TamPagina);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadesPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible cargar las actividades. Intente nuevamente.", true);
            }
        }

        public void Buscar()
        {
            _view.PaginaActual = 1;
            CargarListado();
        }

        public void CambiarPagina(int pagina)
        {
            _view.PaginaActual = pagina < 1 ? 1 : pagina;
            CargarListado();
        }

        public void Seleccionar(int actividadId)
        {
            _view.ActividadSeleccionadaId = actividadId;
            CargarListado();
        }

        public void Desactivar(int actividadId)
        {
            if (!Permisos.PuedeGestionarActividades(_view.RolUsuario))
            {
                _view.MostrarMensaje("No tienes permisos para desactivar actividades.", true);
                return;
            }

            var r = _service.Desactivar(actividadId);
            _view.MostrarMensaje(r.Mensaje, !r.Exito);

            if (r.Exito)
            {
                if (_view.ActividadSeleccionadaId == actividadId)
                    _view.ActividadSeleccionadaId = 0;
                CargarResumen();
                CargarListado();
            }
        }
    }
}
