using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>Contrato de la página Actividades.aspx (KPIs, buscador y listado paginado).</summary>
    public interface IActividadesView
    {
        string RolUsuario { get; }
        string Busqueda { get; set; }
        int PaginaActual { get; set; }
        int ActividadSeleccionadaId { get; set; }

        void MostrarResumen(ResumenDto resumen);
        void MostrarActividades(IList<ActividadDto> actividades, int total, int pagina, int tamPagina);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
