using System;
using System.Diagnostics;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class MisInscripcionesPresenter
    {
        private readonly IMisInscripcionesView _view;
        private readonly InscripcionService _service;
        private readonly UsuarioDto _actual;

        public MisInscripcionesPresenter(IMisInscripcionesView view, InscripcionService service, UsuarioDto actual)
        {
            _view = view;
            _service = service;
            _actual = actual;
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarInscripciones(_service.ListarMisInscripciones(_actual.UsuarioId));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en MisInscripcionesPresenter.cs: {0}", ex);
                _view.MostrarMensaje("No fue posible cargar tus inscripciones.", true);
            }
        }

        public void Cancelar(int inscripcionId)
        {
            var r = _service.Cancelar(inscripcionId, _actual.UsuarioId, _actual.Rol);
            Cargar();
            _view.MostrarMensaje(r.Mensaje, !r.Exito);
        }
    }
}
