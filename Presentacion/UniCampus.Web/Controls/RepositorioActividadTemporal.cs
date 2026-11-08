using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.Web.Controls
{
    // TEMPORAL: borrar cuando ActividadRepository (DAL) sea public.
    // No guarda nada en SQL Server.
    public class RepositorioActividadTemporal : IActividadRepository
    {
        public int Guardar(ActividadDto actividad)
        {
            return 1;
        }
    }
}