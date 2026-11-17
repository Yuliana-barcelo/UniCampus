using System.Collections.Generic;

namespace UniCampus.Entities.Contracts
{
    public interface IActividadRepository
    {
        /// <summary>Persiste la actividad y devuelve un identificador positivo.</summary>
        int Guardar(ActividadDto actividad);

        /// <summary>Actualiza una actividad existente. False si no existe o está inactiva.</summary>
        bool Actualizar(ActividadDto actividad);

        /// <summary>Baja lógica: Estado = 'Inactiva'. False si no existe o ya estaba inactiva.</summary>
        bool Desactivar(int actividadId);

        /// <summary>Devuelve la actividad (no inactiva) o null.</summary>
        ActividadDto ObtenerPorId(int actividadId);

        /// <summary>Listado paginado; la búsqueda filtra por título, tipo, estado, lugar o fecha (dd/MM/yyyy).</summary>
        PaginaActividadesDto Listar(string busqueda, int pagina, int tamPagina);

        IList<TipoActividadDto> ObtenerTipos();

        ResumenDto ObtenerResumen();

        int ContarInscritosActivos(int actividadId);
    }
}
