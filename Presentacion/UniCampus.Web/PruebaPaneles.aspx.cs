using System;

namespace UniCampus.Web
{
    public partial class PruebaPaneles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ucPaneles.MostrarDetalle(
                    "Taller de Programación Web", "Taller",
                    "Espacio práctico para reforzar conceptos de programación web con ASP.NET.",
                    "15/05/2025 - 16/05/2025", "08:00 - 12:00", "Presencial",
                    "Laboratorio 3", 30, "Activa");
            }
        }
    }
}