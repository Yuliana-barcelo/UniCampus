using System.Collections.Generic;
using System.Linq;

namespace uniCampus.BLL.Services
{
    public sealed class ResultadoGuardarActividad
    {
        public bool Exitoso { get { return ActividadId.HasValue; } }
        public int? ActividadId { get; private set; }
        public IReadOnlyList<string> Errores { get; private set; }

        internal ResultadoGuardarActividad(int? actividadId, IEnumerable<string> errores)
        {
            ActividadId = actividadId;
            Errores = errores.ToList().AsReadOnly();
        }
    }
}