using System;
using UniCampus.Entities;

namespace uniCampus.BLL.Services
{
    /// <summary>
    /// Contrato para Persona 4. Implementación pendiente de consultas DAL y
    /// acuerdo sobre inscritos, estados administrativos y límites temporales.
    /// </summary>
    public interface IResumenActividadService
    {
        ResumenDto ObtenerResumen(DateTime fechaReferencia);
    }
}