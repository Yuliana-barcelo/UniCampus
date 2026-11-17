using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Actividades : PaginaBase, IActividadesView
    {
        private ActividadesPresenter _presenter;
        private InscripcionesPresenter _inscripcionesPresenter;

        public bool PuedeGestionar
        {
            get { return Permisos.PuedeGestionarActividades(Usuario.Rol); }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            var actividades = new ActividadService();
            _presenter = new ActividadesPresenter(this, actividades);
            _inscripcionesPresenter = new InscripcionesPresenter(
                ucPaneles, ucPaneles, new InscripcionService(), actividades, new UsuarioService(), Usuario);

            ucFormulario.ActividadGuardada += ucFormulario_ActividadGuardada;
            _inscripcionesPresenter.Cambio += (s, ev) => _presenter.CargarResumen();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Los estudiantes no ven el formulario de administración.
            ucFormulario.Visible = PuedeGestionar;
            if (!PuedeGestionar)
                gridPrincipal.Attributes["class"] = "grid-principal sin-formulario";

            if (!IsPostBack)
            {
                _presenter.Cargar();
                _inscripcionesPresenter.Mostrar(ActividadSeleccionadaId);
            }
        }

        // ---------- Eventos de la página ----------

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            _presenter.Buscar();
        }

        protected void rptActividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            switch (e.CommandName)
            {
                case "ver":
                    _presenter.Seleccionar(id);
                    _inscripcionesPresenter.Mostrar(id);
                    break;
                case "editar":
                    _presenter.Seleccionar(id);
                    _inscripcionesPresenter.Mostrar(id);
                    ucFormulario.Editar(id);
                    break;
                case "eliminar":
                    _presenter.Desactivar(id);
                    _inscripcionesPresenter.Mostrar(ActividadSeleccionadaId);
                    break;
            }
        }

        protected void rptPaginas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int pagina;
            if (int.TryParse(Convert.ToString(e.CommandArgument), out pagina))
                _presenter.CambiarPagina(pagina);
        }

        private void ucFormulario_ActividadGuardada(object sender, IdEventArgs e)
        {
            ActividadSeleccionadaId = e.Id;
            _presenter.CargarResumen();
            _presenter.CargarListado();
            _inscripcionesPresenter.Mostrar(e.Id);
        }

        // ---------- Ayudas para el markup ----------

        protected bool EsSeleccionada(int id)
        {
            return id == ActividadSeleccionadaId;
        }

        protected int NumeroFila(int indice)
        {
            return (PaginaActual - 1) * ActividadesPresenter.TamPagina + indice + 1;
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

        // ---------- IActividadesView ----------

        public string RolUsuario
        {
            get { return Usuario.Rol; }
        }

        public string Busqueda
        {
            get { return txtBuscar.Text.Trim(); }
            set { txtBuscar.Text = value; }
        }

        public int PaginaActual
        {
            get { return ViewState["PaginaActual"] == null ? 1 : (int)ViewState["PaginaActual"]; }
            set { ViewState["PaginaActual"] = value; }
        }

        public int ActividadSeleccionadaId
        {
            get { return ViewState["ActividadSeleccionadaId"] == null ? 0 : (int)ViewState["ActividadSeleccionadaId"]; }
            set { ViewState["ActividadSeleccionadaId"] = value; }
        }

        public void MostrarResumen(ResumenDto resumen)
        {
            lblTotalActividades.Text = resumen.TotalActividades.ToString();
            lblTotalInscritos.Text = resumen.TotalInscritos.ToString();
            lblProximas.Text = resumen.ActividadesProximas.ToString();
            lblFinalizadas.Text = resumen.ActividadesFinalizadas.ToString();
        }

        public void MostrarActividades(IList<ActividadDto> actividades, int total, int pagina, int tamPagina)
        {
            rptActividades.DataSource = actividades;
            rptActividades.DataBind();
            phSinResultados.Visible = actividades.Count == 0;

            int desde = total == 0 ? 0 : (pagina - 1) * tamPagina + 1;
            int hasta = total == 0 ? 0 : desde + actividades.Count - 1;
            lblRango.Text = "Mostrando " + desde + " a " + hasta + " de " + total + " registros";

            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamPagina));
            var botones = new List<ItemPaginacion>();
            botones.Add(new ItemPaginacion { Texto = "«", Pagina = 1, Deshabilitada = pagina <= 1 });
            botones.Add(new ItemPaginacion { Texto = "‹", Pagina = pagina - 1, Deshabilitada = pagina <= 1 });

            int ini = Math.Max(1, pagina - 2);
            int fin = Math.Min(totalPaginas, ini + 4);
            ini = Math.Max(1, fin - 4);
            for (int p = ini; p <= fin; p++)
                botones.Add(new ItemPaginacion { Texto = p.ToString(), Pagina = p, Activa = p == pagina });

            botones.Add(new ItemPaginacion { Texto = "›", Pagina = pagina + 1, Deshabilitada = pagina >= totalPaginas });
            botones.Add(new ItemPaginacion { Texto = "»", Pagina = totalPaginas, Deshabilitada = pagina >= totalPaginas });
            rptPaginas.DataSource = botones;
            rptPaginas.DataBind();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "mensaje error" : "mensaje exito";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }
    }
}
