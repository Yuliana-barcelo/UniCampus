using System;
using System.Collections.Generic;
using System.Web.UI;
using uniCampus.BLL.Services;
using uniCampus.BLL.Validations;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web.Controls
{
    public partial class FormularioActividad : UserControl, IActividadView
    {
        private ActividadPresenter _presenter;

        // Lo escuchará Actividades.aspx (Persona 4) para refrescar el listado
        public event EventHandler ActividadGuardada;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ActividadPresenter(this, CrearServicio());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Lista temporal: todavía no existe un servicio de tipos
                CargarTipos(new List<TipoActividadDto>
                {
                    new TipoActividadDto { TipoId = 1, Nombre = "Taller" },
                    new TipoActividadDto { TipoId = 2, Nombre = "Conferencia" },
                    new TipoActividadDto { TipoId = 3, Nombre = "Seminario" },
                    new TipoActividadDto { TipoId = 4, Nombre = "Curso" },
                    new TipoActividadDto { TipoId = 5, Nombre = "Encuentro" }
                });
            }
        }

        private static ActividadService CrearServicio()
        {
            var reglas = new ReglasActividad(
                new[] { "Presencial", "Virtual", "Híbrida" },  // modalidades permitidas
                new[] { "Presencial", "Híbrida" },             // modalidades que exigen lugar
                new[] { "Activa", "Próxima", "Finalizada" });  // estados permitidos

            // Cuando la Persona 1 suba el repositorio real, cambiar por: new ActividadRepository()
            IActividadRepository repositorio = new RepositorioActividadTemporal();

            return new ActividadService(repositorio, reglas);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            _presenter.Guardar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            _presenter.Limpiar();
        }

        // ---------- Implementación de IActividadView ----------

        public string Titulo
        {
            get { return txtTitulo.Text.Trim(); }
            set { txtTitulo.Text = value; }
        }

        public int TipoId
        {
            get { int id; return int.TryParse(ddlTipo.SelectedValue, out id) ? id : 0; }
            set { ddlTipo.SelectedValue = value.ToString(); }
        }

        public string Descripcion
        {
            get { return txtDescripcion.Text.Trim(); }
            set { txtDescripcion.Text = value; }
        }

        public DateTime? FechaInicio
        {
            get { DateTime f; return DateTime.TryParse(txtFechaInicio.Text, out f) ? f : (DateTime?)null; }
            set { txtFechaInicio.Text = value.HasValue ? value.Value.ToString("yyyy-MM-dd") : ""; }
        }

        public DateTime? FechaFin
        {
            get { DateTime f; return DateTime.TryParse(txtFechaFin.Text, out f) ? f : (DateTime?)null; }
            set { txtFechaFin.Text = value.HasValue ? value.Value.ToString("yyyy-MM-dd") : ""; }
        }

        public TimeSpan? HoraInicio
        {
            get { TimeSpan h; return TimeSpan.TryParse(txtHoraInicio.Text, out h) ? h : (TimeSpan?)null; }
            set { txtHoraInicio.Text = value.HasValue ? value.Value.ToString(@"hh\:mm") : ""; }
        }

        public TimeSpan? HoraFin
        {
            get { TimeSpan h; return TimeSpan.TryParse(txtHoraFin.Text, out h) ? h : (TimeSpan?)null; }
            set { txtHoraFin.Text = value.HasValue ? value.Value.ToString(@"hh\:mm") : ""; }
        }

        public string Modalidad
        {
            get { return ddlModalidad.SelectedValue; }
            set { ddlModalidad.SelectedValue = value; }
        }

        public int? Cupos
        {
            get { int c; return int.TryParse(txtCupos.Text, out c) ? c : (int?)null; }
            set { txtCupos.Text = value.HasValue ? value.Value.ToString() : ""; }
        }

        public string Lugar
        {
            get { return txtLugar.Text.Trim(); }
            set { txtLugar.Text = value; }
        }

        public string Estado
        {
            get { return ddlEstado.SelectedValue; }
            set { ddlEstado.SelectedValue = value; }
        }

        public void CargarTipos(IList<TipoActividadDto> tipos)
        {
            ddlTipo.DataSource = tipos;
            ddlTipo.DataValueField = "TipoId";
            ddlTipo.DataTextField = "Nombre";
            ddlTipo.DataBind();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "mensaje error" : "mensaje exito";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        public void LimpiarFormulario()
        {
            txtTitulo.Text = "";
            txtDescripcion.Text = "";
            txtLugar.Text = "";
            txtCupos.Text = "";
            txtFechaInicio.Text = "";
            txtFechaFin.Text = "";
            txtHoraInicio.Text = "";
            txtHoraFin.Text = "";
            ddlTipo.SelectedIndex = 0;
            ddlModalidad.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
        }

        public void NotificarGuardado()
        {
            var manejador = ActividadGuardada;
            if (manejador != null) manejador(this, EventArgs.Empty);
        }
    }
}