using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Inscripciones : PaginaBase, IMisInscripcionesView
    {
        private MisInscripcionesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new MisInscripcionesPresenter(this, new InscripcionService(), Usuario);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        protected void rptMisInscripciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "cancelar" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
                _presenter.Cancelar(id);
        }

        protected string ClaseEstado(string estado)
        {
            switch (estado)
            {
                case "Activa": return "activa";
                case "Próxima": return "proxima";
                case "Finalizada": return "finalizada";
                default: return "inactiva";
            }
        }

        public void MostrarInscripciones(IList<MiInscripcionDto> inscripciones)
        {
            rptMisInscripciones.DataSource = inscripciones;
            rptMisInscripciones.DataBind();
            phSinInscripciones.Visible = inscripciones.Count == 0;
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "mensaje error" : "mensaje exito";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }
    }
}
