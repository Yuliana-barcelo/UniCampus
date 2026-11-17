using System.Collections.Generic;

namespace UniCampus.Entities.Contracts
{
    public interface IInscripcionRepository
    {
        IList<EstudianteInscritoDto> ListarPorActividad(int actividadId);

        IList<MiInscripcionDto> ListarPorUsuario(int usuarioId);

        int ContarInscritos(int actividadId);

        /// <summary>
        /// Verifica actividad, estado, usuario, duplicado y cupo, e inscribe,
        /// todo dentro de una sola transacción.
        /// </summary>
        ResultadoInscripcionDatos Inscribir(int actividadId, int usuarioId);

        /// <summary>
        /// Cancela una inscripción activa. Un administrador puede cancelar cualquiera;
        /// un estudiante solo la suya.
        /// </summary>
        bool Cancelar(int inscripcionId, int usuarioSolicitanteId, bool esAdministrador);
    }
}
