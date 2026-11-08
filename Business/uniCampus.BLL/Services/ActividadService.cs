using System;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;
using uniCampus.BLL.Validations;

namespace uniCampus.BLL.Services
{
    public class ActividadService
    {
        private const string ErrorPersistencia = "No fue posible guardar la actividad. Consulte con el responsable antes de intentar nuevamente.";
        private readonly IActividadRepository repository;
        private readonly ActividadValidator validator;

        public ActividadService(IActividadRepository repository, ReglasActividad reglas)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (reglas == null)
                throw new ArgumentNullException(nameof(reglas));

            this.repository = repository;
            validator = new ActividadValidator(reglas);
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
            catch (Exception)
            {
                // El diagnóstico técnico corresponde a DAL/infraestructura, no al mensaje de la vista.
                return new ResultadoGuardarActividad(null, new[] { ErrorPersistencia });
            }

            if (actividadId <= 0)
                return new ResultadoGuardarActividad(null, new[] { ErrorPersistencia });

            return new ResultadoGuardarActividad(actividadId, new string[0]);
        }
    }
}