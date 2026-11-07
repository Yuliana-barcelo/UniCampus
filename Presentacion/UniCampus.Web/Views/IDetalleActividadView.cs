using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCampus.Web.Views
{
    public  interface IDetalleActividadView
    {
        void MostrarDetalle(string titulo, string tipo, string descripcion,
                            string fechas, string horario, string modalidad,
                            string lugar, int cupos, string estado);
    }
}
