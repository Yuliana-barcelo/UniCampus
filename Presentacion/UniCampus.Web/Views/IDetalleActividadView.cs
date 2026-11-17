namespace UniCampus.Web.Views
{
    public interface IDetalleActividadView
    {
        void MostrarDetalle(string titulo, string tipo, string descripcion,
                            string fechas, string horario, string modalidad,
                            string lugar, int cupos, string estado);

        void LimpiarDetalle();
    }
}
