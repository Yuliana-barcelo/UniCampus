using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IMisInscripcionesView
    {
        void MostrarInscripciones(IList<MiInscripcionDto> inscripciones);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
