using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Controls
{
    public partial class PanelesActividad : System.Web.UI.UserControl, IDetalleActividadView, IInscripcionesView
    {
        public event EventHandler InscribirmeSolicitado;
        public event EventHandler InscribirEstudianteSolicitado;
        public event EventHandler<IdEventArgs> CancelarInscripcionSolicitada;

        public int ActividadId
        {
            get { return ViewState["ActividadId"] == null ? 0 : (int)ViewState["ActividadId"]; }
            set { ViewState["ActividadId"] = value; }
        }

        /// <summary>Solo para ocultar/mostrar botones; la autorización real se valida en el servidor (BLL).</summary>
        protected bool PuedeAdministrar
        {
            get { return ViewState["PuedeAdministrar"] != null && (bool)ViewState["PuedeAdministrar"]; }
        }

        public int EstudianteSeleccionadoId
        {
            get { int id; return int.TryParse(ddlEstudiantes.SelectedValue, out id) ? id : 0; }
        }

        // ---------- IDetalleActividadView ----------

        public void MostrarDetalle(string titulo, string tipo, string descripcion,
                                   string fechas, string horario, string modalidad,
                                   string lugar, int cupos, string estado)
        {
            lblTitulo.Text = Server.HtmlEncode(titulo);
            lblTipo.Text = Server.HtmlEncode(tipo);
            lblDescripcion.Text = Server.HtmlEncode(descripcion);
            lblFechas.Text = Server.HtmlEncode(fechas);
            lblHorario.Text = Server.HtmlEncode(horario);
            lblModalidad.Text = Server.HtmlEncode(modalidad);
            lblLugar.Text = Server.HtmlEncode(lugar);
            lblCupos.Text = cupos.ToString();
            lblEstado.Text = Server.HtmlEncode(estado);
            lblEstado.CssClass = "badge estado-" + ClaseEstado(estado);
        }

        public void LimpiarDetalle()
        {
            MostrarDetalle("Seleccione una actividad", "", "", "", "", "", "", 0, "");
            lblCupos.Text = "";
            lblEstado.CssClass = "";
        }

        // ---------- IInscripcionesView ----------

        public void MostrarEstudiantes(IList<EstudianteInscritoDto> estudiantes, bool puedeAdministrar)
        {
            ViewState["PuedeAdministrar"] = puedeAdministrar;
            pnlInscribirEstudiante.Visible = puedeAdministrar;
            rptInscritos.DataSource = estudiantes;
            rptInscritos.DataBind();
            phSinInscritos.Visible = estudiantes.Count == 0;
            lblTotalInscritos.Text = estudiantes.Count.ToString();
        }

        public void CargarEstudiantesDisponibles(IList<UsuarioDto> estudiantes)
        {
            ddlEstudiantes.DataSource = estudiantes;
            ddlEstudiantes.DataValueField = "UsuarioId";
            ddlEstudiantes.DataTextField = "Nombre";
            ddlEstudiantes.DataBind();
            btnInscribirEstudiante.Enabled = estudiantes.Count > 0;
        }

        public void MostrarVistaEstudiante(string titulo, string tipo, string estado,
                                           string fechas, string horario, string lugar,
                                           int cuposDisponibles, int cupoTotal, string descripcion,
                                           bool puedeInscribirme, string motivo)
        {
            lblVistaTitulo.Text = Server.HtmlEncode(titulo);
            lblVistaTipo.Text = Server.HtmlEncode(tipo);
            lblVistaEstado.Text = Server.HtmlEncode(estado);
            lblVistaEstado.CssClass = "badge estado-" + ClaseEstado(estado);
            lblVistaFechas.Text = Server.HtmlEncode(fechas);
            lblVistaHorario.Text = Server.HtmlEncode(horario);
            lblVistaLugar.Text = Server.HtmlEncode(lugar);
            lblVistaCupos.Text = cuposDisponibles + " de " + cupoTotal;
            lblVistaDescripcion.Text = Server.HtmlEncode(descripcion);
            btnInscribirme.Visible = puedeInscribirme;
            lblMotivo.Text = Server.HtmlEncode(motivo);
        }

        public void LimpiarInscripciones()
        {
            ViewState["PuedeAdministrar"] = false;
            pnlInscribirEstudiante.Visible = false;
            rptInscritos.DataSource = new List<EstudianteInscritoDto>();
            rptInscritos.DataBind();
            phSinInscritos.Visible = true;
            lblTotalInscritos.Text = "0";
            lblVistaTitulo.Text = "Seleccione una actividad";
            lblVistaTipo.Text = "";
            lblVistaEstado.Text = "";
            lblVistaEstado.CssClass = "badge";
            lblVistaFechas.Text = "";
            lblVistaHorario.Text = "";
            lblVistaLugar.Text = "";
            lblVistaCupos.Text = "";
            lblVistaDescripcion.Text = "";
            btnInscribirme.Visible = false;
            lblMotivo.Text = "";
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "mensaje error" : "mensaje exito";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        // ---------- eventos de la interfaz ----------

        protected void btnInscribirme_Click(object sender, EventArgs e)
        {
            if (InscribirmeSolicitado != null) InscribirmeSolicitado(this, EventArgs.Empty);
        }

        protected void btnInscribirEstudiante_Click(object sender, EventArgs e)
        {
            if (InscribirEstudianteSolicitado != null) InscribirEstudianteSolicitado(this, EventArgs.Empty);
        }

        protected void rptInscritos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "cancelar" && int.TryParse(Convert.ToString(e.CommandArgument), out id)
                && CancelarInscripcionSolicitada != null)
            {
                CancelarInscripcionSolicitada(this, new IdEventArgs(id));
            }
        }

        private static string ClaseEstado(string estado)
        {
            switch (estado)
            {
                case "Activa": return "activa";
                case "Próxima": return "proxima";
                case "Finalizada": return "finalizada";
                default: return "inactiva";
            }
        }
    }
}
