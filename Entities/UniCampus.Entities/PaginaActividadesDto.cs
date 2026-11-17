using System.Collections.Generic;

namespace UniCampus.Entities
{
    /// <summary>Una página del listado de actividades más el total de registros que cumplen el filtro.</summary>
    public class PaginaActividadesDto
    {
        public IList<ActividadDto> Items { get; set; }
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamPagina { get; set; }
    }
}
