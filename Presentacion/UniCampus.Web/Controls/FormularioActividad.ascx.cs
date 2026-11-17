using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Security;
using UniCampus.Web.Views;

namespace UniCampus.Web.Controls
{
    public partial class FormularioActividad : UserControl, IActividadView
    {
        private ActividadPresenter _presenter;

        /// <summary>Actividades.aspx lo escucha para refrescar KPIs, listado y paneles.</summary>
        public event EventHandler<IdEventArgs> ActividadGuardada;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ActividadPresenter(this, new ActividadService());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                _presenter.CargarTipos();   // los tipos vienen de SQL Server, no de una lista fija
        }

        /// <summary>La llama la página cuando se pulsa el lápiz del listado.</summary>
        public void Editar(int actividadId)
        {
            _presenter.CargarParaEdicion(actividadId);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            _presenter.Guardar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            _presenter.Limpiar();
        }

        // ---------- IActividadView ----------

        public int ActividadId
        {
            get { return ViewState["ActividadId"] == null ? 0 : (int)ViewState["ActividadId"]; }
            set
            {
                ViewState["ActividadId"] = value;
                lblTituloForm.Text = value > 0 ? "Editar Actividad Académica" : "Nueva Actividad Académica";
                lblTextoGuardar.Text = value > 0 ? "Actualizar" : "Guardar";
            }
        }

        public string RolUsuario
        {
            get
            {
                var u = SesionUsuario.Actual;
                return u == null ? string.Empty : u.Rol;
            }
        }

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
            get { return LeerFecha(txtFechaInicio.Text); }
            set { txtFechaInicio.Text = value.HasValue ? value.Value.ToString("yyyy-MM-dd") : ""; }
        }

        public DateTime? FechaFin
        {
            get { return LeerFecha(txtFechaFin.Text); }
            set { txtFechaFin.Text = value.HasValue ? value.Value.ToString("yyyy-MM-dd") : ""; }
        }

        public TimeSpan? HoraInicio
        {
            get { return LeerHora(txtHoraInicio.Text); }
            set { txtHoraInicio.Text = value.HasValue ? value.Value.ToString(@"hh\:mm") : ""; }
        }

        public TimeSpan? HoraFin
        {
            get { return LeerHora(txtHoraFin.Text); }
            set { txtHoraFin.Text = value.HasValue ? value.Value.ToString(@"hh\:mm") : ""; }
        }

        public string Modalidad
        {
            get { return ddlModalidad.SelectedValue; }
            set { ddlModalidad.SelectedValue = value; }
        }

        public int? Cupos
        {
            get { int c; return int.TryParse(txtCupos.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out c) ? c : (int?)null; }
            set { txtCupos.Text = value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : ""; }
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
            ActividadId = 0;
            txtTitulo.Text = "";
            txtDescripcion.Text = "";
            txtLugar.Text = "";
            txtCupos.Text = "";
            txtFechaInicio.Text = "";
            txtFechaFin.Text = "";
            txtHoraInicio.Text = "";
            txtHoraFin.Text = "";
            if (ddlTipo.Items.Count > 0) ddlTipo.SelectedIndex = 0;
            ddlModalidad.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
        }

        public void NotificarGuardado(int actividadId)
        {
            var manejador = ActividadGuardada;
            if (manejador != null) manejador(this, new IdEventArgs(actividadId));
        }

        // ---------- lectura segura de fechas y horas (formato fijo del navegador) ----------

        private static DateTime? LeerFecha(string texto)
        {
            DateTime f;
            return DateTime.TryParseExact((texto ?? "").Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out f) ? f : (DateTime?)null;
        }

        private static TimeSpan? LeerHora(string texto)
        {
            TimeSpan h;
            return TimeSpan.TryParseExact((texto ?? "").Trim(), new[] { @"hh\:mm", @"hh\:mm\:ss" },
                CultureInfo.InvariantCulture, out h) ? h : (TimeSpan?)null;
        }
    }
}
