using System;
using System.Collections.Generic;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace uniCampus.BLL.Services
{
    public class ResenaService
    {
        private const string ErrorGeneral =
            "No fue posible guardar la reseña. Intenta nuevamente más tarde.";
        private const int LargoMaximoComentario = 500;

        private readonly IResenaRepository repository;

        public ResenaService(IResenaRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            this.repository = repository;
        }

        public IList<ResenaDto> Listar(int actividadId)
        {
            if (actividadId <= 0)
                return new List<ResenaDto>();
            return repository.ListarPorActividad(actividadId);
        }

        public ResultadoOperacion Guardar(ResenaDto resena)
        {
            if (resena == null)
                return ResultadoOperacion.Fallo("Debes proporcionar una reseña.");
            if (resena.ActividadId <= 0 || resena.UsuarioId <= 0)
                return ResultadoOperacion.Fallo("Los datos de la reseña no son válidos.");
            if (resena.Calificacion < 1 || resena.Calificacion > 5)
                return ResultadoOperacion.Fallo("La calificación debe estar entre 1 y 5.");
            if (resena.Comentario != null && resena.Comentario.Length > LargoMaximoComentario)
                return ResultadoOperacion.Fallo("El comentario no puede superar " + LargoMaximoComentario + " caracteres.");

            try
            {
                if (repository.ExisteResena(resena.ActividadId, resena.UsuarioId))
                    return ResultadoOperacion.Fallo("Ya calificaste esta actividad.");

                int id = repository.Guardar(resena);
                if (id <= 0)
                    return ResultadoOperacion.Fallo(ErrorGeneral);
                return ResultadoOperacion.Ok("Reseña guardada correctamente.");
            }
            catch (Exception)
            {
                return ResultadoOperacion.Fallo(ErrorGeneral);
            }
        }
    }
}