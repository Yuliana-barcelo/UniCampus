using System;
using System.Collections.Generic;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace uniCampus.BLL.Services
{
    public class InscripcionService
    {
        private const string ErrorGeneral =
            "No fue posible completar la inscripción. Intenta nuevamente más tarde.";

        private readonly IInscripcionRepository repository;

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

        public int ContarInscritos(int actividadId)
        {
            return actividadId <= 0 ? 0 : repository.ContarInscritos(actividadId);
        }

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
            catch (Exception)
            {
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
                default:
                    return ResultadoOperacion.Fallo(ErrorGeneral);
            }
        }
    }
}