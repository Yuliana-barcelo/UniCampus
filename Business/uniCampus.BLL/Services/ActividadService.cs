using System;
using System.Diagnostics;
using System.Collections.Generic;
using UniCampus.DAL.Repositories;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;
using uniCampus.BLL.Validations;

namespace uniCampus.BLL.Services
{
    public class ActividadService : IResumenActividadService
    {
        private const string ErrorPersistencia = "No fue posible guardar la actividad. Consulte con el responsable antes de intentar nuevamente.";
        private readonly IActividadRepository repository;
        private readonly ActividadValidator validator;

        /// <summary>Constructor usado por la aplicación: arma el repositorio real y las reglas del curso.</summary>
        public ActividadService() : this(new ActividadRepository(), ReglasPorDefecto()) { }

        /// <summary>Constructor usado por las pruebas unitarias (repositorio falso).</summary>
        public ActividadService(IActividadRepository repository, ReglasActividad reglas)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (reglas == null)
                throw new ArgumentNullException(nameof(reglas));

            this.repository = repository;
            validator = new ActividadValidator(reglas);
        }

        public static ReglasActividad ReglasPorDefecto()
        {
            return new ReglasActividad(
                new[] { "Presencial", "Virtual", "Híbrida" },   // modalidades permitidas
                new[] { "Presencial", "Virtual", "Híbrida" },   // el lugar es obligatorio (columna NOT NULL)
                new[] { "Activa", "Próxima", "Finalizada" });   // estados que se pueden elegir
        }

        public ResultadoGuardarActividad Guardar(ActividadDto actividad)
        {
            var errores = validator.Validar(actividad);
            if (errores.Count > 0)
                return new ResultadoGuardarActividad(null, errores);

            int actividadId;
            try
            {
                // No reintentar: una excepción no garantiza que DAL no haya persistido.
                actividadId = repository.Guardar(actividad);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadService.cs: {0}", ex);
                return new ResultadoGuardarActividad(null, new[] { ErrorPersistencia });
            }

            if (actividadId <= 0)
                return new ResultadoGuardarActividad(null, new[] { ErrorPersistencia });

            return new ResultadoGuardarActividad(actividadId, new string[0]);
        }

        public ResultadoOperacion Actualizar(ActividadDto actividad)
        {
            var errores = validator.Validar(actividad);
            if (errores.Count > 0)
                return ResultadoOperacion.Fallo(string.Join(" ", errores));
            if (actividad.ActividadId <= 0)
                return ResultadoOperacion.Fallo("La actividad a actualizar no es válida.");

            try
            {
                int inscritos = repository.ContarInscritosActivos(actividad.ActividadId);
                if (actividad.Cupos < inscritos)
                    return ResultadoOperacion.Fallo("Los cupos no pueden ser menores que los " + inscritos + " estudiantes ya inscritos.");

                if (!repository.Actualizar(actividad))
                    return ResultadoOperacion.Fallo("La actividad no existe o ya fue desactivada.");

                return ResultadoOperacion.Ok("Actividad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadService.cs: {0}", ex);
                return ResultadoOperacion.Fallo("No fue posible actualizar la actividad. Intente nuevamente.");
            }
        }

        public ResultadoOperacion Desactivar(int actividadId)
        {
            if (actividadId <= 0)
                return ResultadoOperacion.Fallo("La actividad no es válida.");
            try
            {
                return repository.Desactivar(actividadId)
                    ? ResultadoOperacion.Ok("La actividad fue desactivada.")
                    : ResultadoOperacion.Fallo("La actividad no existe o ya estaba desactivada.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ActividadService.cs: {0}", ex);
                return ResultadoOperacion.Fallo("No fue posible desactivar la actividad. Intente nuevamente.");
            }
        }

        public ActividadDto ObtenerPorId(int actividadId)
        {
            return actividadId <= 0 ? null : repository.ObtenerPorId(actividadId);
        }

        public PaginaActividadesDto Listar(string busqueda, int pagina, int tamPagina)
        {
            return repository.Listar(busqueda, pagina < 1 ? 1 : pagina, tamPagina < 1 ? 5 : tamPagina);
        }

        public IList<TipoActividadDto> ObtenerTipos()
        {
            return repository.ObtenerTipos();
        }

        /// <summary>El resumen se calcula con el Estado de cada actividad; fechaReferencia queda reservada.</summary>
        public ResumenDto ObtenerResumen(DateTime fechaReferencia)
        {
            return repository.ObtenerResumen();
        }
    }
}
