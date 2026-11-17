using System.Collections.Generic;

namespace UniCampus.Entities.Contracts
{
    public interface IResenaRepository
    {
        int Guardar(ResenaDto resena);
        bool ExisteResena(int actividadId, int usuarioId);
        IList<ResenaDto> ListarPorActividad(int actividadId);
    }
}