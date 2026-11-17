using System;
using System.Diagnostics;
using System.Collections.Generic;
using UniCampus.DAL.Repositories;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace uniCampus.BLL.Services
{
    public class InscripcionService
    {
        private const string ErrorGeneral =
            "No fue posible completar la inscripción. Intenta nuevamente más tarde.";

        private readonly IInscripcionRepository repository;

        public InscripcionService() : this(new InscripcionRepository()) { }

        public InscripcionService(IInscripcionRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            this.repository = repository;
        }

        public IList<EstudianteInscritoDto> ListarInscritos(int actividadId)
        {
            if (actividadId <= 0)
                return new List<EstudianteInscritoDto>();
            return repository.ListarPorActividad(actividadId);
        }

        public IList<MiInscripcionDto> ListarMisInscripciones(int usuarioId)
        {
            if (usuarioId <= 0)
                return new List<MiInscripcionDto>();
            return repository.ListarPorUsuario(usuarioId);
        }

        public int ContarInscritos(int actividadId)
        {
            return actividadId <= 0 ? 0 : repository.ContarInscritos(actividadId);
        }

        /// <summary>Inscribe al usuario de la sesión (usuarioId NO debe venir de un control de la página).</summary>
        public ResultadoOperacion Inscribir(int actividadId, int usuarioId)
        {
            if (actividadId <= 0 || usuarioId <= 0)
                return ResultadoOperacion.Fallo("Los datos de la inscripción no son válidos.");

            ResultadoInscripcionDatos resultado;
            try
            {
                // No se reintenta: la transacción de DAL ya garantiza cupo y duplicados.
                resultado = repository.Inscribir(actividadId, usuarioId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en InscripcionService.cs: {0}", ex);
                return ResultadoOperacion.Fallo(ErrorGeneral);
            }

            switch (resultado)
            {
                case ResultadoInscripcionDatos.Exito:
                    return ResultadoOperacion.Ok("Inscripción realizada correctamente.");
                case ResultadoInscripcionDatos.Duplicada:
                    return ResultadoOperacion.Fallo("Ya estás inscrito en esta actividad.");
                case ResultadoInscripcionDatos.SinCupo:
                    return ResultadoOperacion.Fallo("No hay cupos disponibles.");
                case ResultadoInscripcionDatos.ActividadNoExiste:
                    return ResultadoOperacion.Fallo("La actividad no existe.");
                case ResultadoInscripcionDatos.ActividadNoDisponible:
                    return ResultadoOperacion.Fallo("La actividad ya no admite inscripciones.");
                case ResultadoInscripcionDatos.UsuarioNoValido:
                    return ResultadoOperacion.Fallo("El usuario no existe o no está activo.");
                default:
                    return ResultadoOperacion.Fallo(ErrorGeneral);
            }
        }

        /// <summary>Un administrador/organizador inscribe a un estudiante. El rol viene de la sesión.</summary>
        public ResultadoOperacion InscribirEstudiante(int actividadId, int estudianteId, string rolSolicitante)
        {
            if (!Permisos.PuedeInscribirOtros(rolSolicitante))
                return ResultadoOperacion.Fallo("No tienes permisos para inscribir a otros estudiantes.");
            return Inscribir(actividadId, estudianteId);
        }

        public ResultadoOperacion Cancelar(int inscripcionId, int usuarioSolicitanteId, string rolSolicitante)
        {
            if (inscripcionId <= 0 || usuarioSolicitanteId <= 0)
                return ResultadoOperacion.Fallo("Los datos de la inscripción no son válidos.");
            try
            {
                bool admin = Permisos.PuedeInscribirOtros(rolSolicitante);
                return repository.Cancelar(inscripcionId, usuarioSolicitanteId, admin)
                    ? ResultadoOperacion.Ok("La inscripción fue cancelada.")
                    : ResultadoOperacion.Fallo("No se encontró la inscripción o no tienes permiso para cancelarla.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en InscripcionService.cs: {0}", ex);
                return ResultadoOperacion.Fallo("No fue posible cancelar la inscripción. Intenta nuevamente.");
            }
        }
    }
}
