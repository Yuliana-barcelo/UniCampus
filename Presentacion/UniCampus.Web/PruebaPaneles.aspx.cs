using System;
using UniCampus.Web.Presenters;

namespace UniCampus.Web
{
    public partial class PruebaPaneles : System.Web.UI.Page
    {
        private InscripcionesPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se crea en CADA carga (también en postback) para escuchar el clic del botón
            _presenter = new InscripcionesPresenter(ucPaneles);

            if (!IsPostBack)
            {
                ucPaneles.MostrarDetalle(
                    "Taller de Programación Web", "Taller",
                    "Espacio práctico para reforzar conceptos de programación web con ASP.NET.",
                    "15/05/2025 - 16/05/2025", "08:00 - 12:00", "Presencial",
                    "Laboratorio 3", 30, "Activa");

                _presenter.CargarEstudiantes(1);
                _presenter.CargarVistaEstudiante(1);
            }
        }
    }
}