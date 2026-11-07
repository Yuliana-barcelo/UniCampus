using System;
using System.Web.UI;
using UniCampus.Web.Views;

namespace UniCampus.Web.Controls
{
    public partial class PanelesActividad : System.Web.UI.UserControl, IDetalleActividadView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void MostrarDetalle(string titulo, string tipo, string descripcion,
                                   string fechas, string horario, string modalidad,
                                   string lugar, int cupos, string estado)
        {
            lblTitulo.Text = titulo;
            lblTipo.Text = tipo;
            lblDescripcion.Text = descripcion;
            lblFechas.Text = fechas;
            lblHorario.Text = horario;
            lblModalidad.Text = modalidad;
            lblLugar.Text = lugar;
            lblCupos.Text = cupos.ToString();
            lblEstado.Text = estado;
        }
    }
}