using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>
    /// Presenter de los tres paneles inferiores (detalle, inscritos y vista del estudiante).
    /// La identidad (_actual) viene de la sesión: nunca de un control de la página.
    /// </summary>
    public class InscripcionesPresenter
    {
        private readonly IInscripcionesView _vista;
        private readonly IDetalleActividadView _detalle;
        private readonly InscripcionService _inscripciones;
        private readonly ActividadService _actividades;
        private readonly UsuarioService _usuarios;
        private readonly UsuarioDto _actual;

        /// <summary>Se dispara cuando cambian las inscripciones, para que la página refresque los KPIs.</summary>
        public event EventHandler Cambio;

        public InscripcionesPresenter(IInscripcionesView vista, IDetalleActividadView detalle,
                                      InscripcionService inscripciones, ActividadService actividades,
                                      UsuarioService usuarios, UsuarioDto actual)
        {
            _vista = vista;
            _detalle = detalle;
            _inscripciones = inscripciones;
            _actividades = actividades;
            _usuarios = usuarios;
            _actual = actual;

            _vista.InscribirmeSolicitado += AlInscribirme;
            _vista.InscribirEstudianteSolicitado += AlInscribirEstudiante;
            _vista.CancelarInscripcionSolicitada += AlCancelar;
        }

        public void Mostrar(int actividadId)
        {
            try
            {
                var a = _actividades.ObtenerPorId(actividadId);
                if (a == null)
                {
                    _vista.ActividadId = 0;
                    _detalle.LimpiarDetalle();
                    _vista.LimpiarInscripciones();
                    return;
                }

                bool admin = Permisos.PuedeInscribirOtros(_actual.Rol);
                var inscritos = _inscripciones.ListarInscritos(actividadId);
                int disponibles = Math.Max(0, a.Cupos - inscritos.Count);

                _vista.ActividadId = actividadId;

                _detalle.MostrarDetalle(a.Titulo, a.TipoNombre, a.Descripcion,
                    Fechas(a), Horario(a), a.Modalidad, a.Lugar, a.Cupos, a.Estado);

                _vista.MostrarEstudiantes(inscritos, admin);

                if (admin)
                {
                    var ya = new HashSet<int>(inscritos.Select(i => i.UsuarioId));
                    var libres = _usuarios.ListarEstudiantes().Where(u => !ya.Contains(u.UsuarioId)).ToList();
                    _vista.CargarEstudiantesDisponibles(libres);
                }
                else
                {
                    _vista.CargarEstudiantesDisponibles(new List<UsuarioDto>());
                }

                bool abierta = a.Estado == "Activa" || a.Estado == "Próxima";
                bool yaInscrito = inscritos.Any(i => i.UsuarioId == _actual.UsuarioId);
                string motivo = string.Empty;
                if (!abierta) motivo = "Esta actividad ya no admite inscripciones.";
                else if (yaInscrito) motivo = "Ya estás inscrito en esta actividad.";
                else if (disponibles == 0) motivo = "No hay cupos disponibles.";

                _vista.MostrarVistaEstudiante(a.Titulo, a.TipoNombre, a.Estado,
                    Fechas(a), Horario(a), a.Lugar, disponibles, a.Cupos, a.Descripcion,
                    motivo.Length == 0, motivo);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en InscripcionesPresenter.cs: {0}", ex);
                _vista.MostrarMensaje("No fue posible cargar los datos de la actividad.", true);
            }
        }

        private void AlInscribirme(object sender, EventArgs e)
        {
            Ejecutar(_inscripciones.Inscribir(_vista.ActividadId, _actual.UsuarioId));
        }

        private void AlInscribirEstudiante(object sender, EventArgs e)
        {
            Ejecutar(_inscripciones.InscribirEstudiante(
                _vista.ActividadId, _vista.EstudianteSeleccionadoId, _actual.Rol));
        }

        private void AlCancelar(object sender, IdEventArgs e)
        {
            Ejecutar(_inscripciones.Cancelar(e.Id, _actual.UsuarioId, _actual.Rol));
        }

        private void Ejecutar(ResultadoOperacion r)
        {
            int id = _vista.ActividadId;
            Mostrar(id);
            _vista.MostrarMensaje(r.Mensaje, !r.Exito);
            if (r.Exito && Cambio != null) Cambio(this, EventArgs.Empty);
        }

        private static string Fechas(ActividadDto a)
        {
            return a.FechaInicio == a.FechaFin
                ? a.FechaInicio.ToString("dd/MM/yyyy")
                : a.FechaInicio.ToString("dd/MM/yyyy") + " - " + a.FechaFin.ToString("dd/MM/yyyy");
        }

        private static string Horario(ActividadDto a)
        {
            return a.HoraInicio.ToString(@"hh\:mm") + " - " + a.HoraFin.ToString(@"hh\:mm");
        }
    }
}
