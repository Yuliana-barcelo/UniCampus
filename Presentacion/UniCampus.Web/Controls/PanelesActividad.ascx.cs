using System;
using System.Collections.Generic;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Controls
{
    public partial class PanelesActividad : System.Web.UI.UserControl, IDetalleActividadView, IInscripcionesView
    {
        public event EventHandler InscribirmeSolicitado;

        public int ActividadId
        {
            get { return ViewState["ActividadId"] == null ? 0 : (int)ViewState["ActividadId"]; }
            set { ViewState["ActividadId"] = value; }
        }

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

        public void MostrarEstudiantes(IList<EstudianteInscritoDto> estudiantes)
        {
            rptInscritos.DataSource = estudiantes;
            rptInscritos.DataBind();
            lblTotalInscritos.Text = estudiantes.Count.ToString();
        }

        public void MostrarVistaEstudiante(string titulo, string tipo, string estado,
                                           string fechas, string horario, string lugar,
                                           int cuposDisponibles, int cupoTotal, string descripcion)
        {
            lblVistaTitulo.Text = titulo;
            lblVistaTipo.Text = tipo;
            lblVistaEstado.Text = estado;
            lblVistaEstado.CssClass = "badge estado-" + estado.ToLower().Replace("ó", "o");
            lblVistaFechas.Text = fechas;
            lblVistaHorario.Text = horario;
            lblVistaLugar.Text = lugar;
            lblVistaCupos.Text = cuposDisponibles + " de " + cupoTotal;
            lblVistaDescripcion.Text = descripcion;
            btnInscribirme.Enabled = cuposDisponibles > 0;
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "mensaje-error" : "mensaje-exito";
        }

        protected void btnInscribirme_Click(object sender, EventArgs e)
        {
            if (InscribirmeSolicitado != null)
            {
                InscribirmeSolicitado(this, EventArgs.Empty);
            }
        }
    }
}