using System;
using System.Collections.Generic;
using UniCampus.Entities;

namespace uniCampus.BLL.Validations
{
    internal sealed class ActividadValidator
    {
        private readonly ReglasActividad reglas;

        internal ActividadValidator(ReglasActividad reglas)
        {
            this.reglas = reglas;
        }

        internal List<string> Validar(ActividadDto actividad)
        {
            var errores = new List<string>();
            if (actividad == null)
            {
                errores.Add("Debe proporcionar una actividad.");
                return errores;
            }

            ValidarDatosObligatorios(actividad, errores);
            ValidarIntervalo(actividad, errores);
            ValidarModalidadYLugar(actividad, errores);
            if (!reglas.EstadoPermitido(actividad.Estado))
                errores.Add("El estado debe pertenecer al catálogo permitido.");

            return errores;
        }

        private static void ValidarDatosObligatorios(ActividadDto actividad, List<string> errores)
        {
            if (string.IsNullOrWhiteSpace(actividad.Titulo))
                errores.Add("El título es obligatorio.");
            if (string.IsNullOrWhiteSpace(actividad.Descripcion))
                errores.Add("La descripción es obligatoria.");
            if (actividad.TipoId <= 0)
                errores.Add("El identificador del tipo de actividad debe ser mayor que cero.");
            if (actividad.Cupos <= 0)
                errores.Add("Los cupos deben ser mayores que cero.");
        }

        private static void ValidarIntervalo(ActividadDto actividad, List<string> errores)
        {
            bool inicioValido = HoraValida(actividad.HoraInicio);
            bool finValido = HoraValida(actividad.HoraFin);
            if (!inicioValido)
                errores.Add("La hora de inicio debe estar entre 00:00 y antes de 24:00.");
            if (!finValido)
                errores.Add("La hora de fin debe estar entre 00:00 y antes de 24:00.");

            // Las fechas se interpretan por Date; reservar la fecha default evita
            // que una hora interna oculte una fecha no proporcionada.
            bool fechaInicioProporcionada = actividad.FechaInicio.Date != default(DateTime);
            bool fechaFinProporcionada = actividad.FechaFin.Date != default(DateTime);
            if (!fechaInicioProporcionada)
                errores.Add("La fecha de inicio es obligatoria.");
            if (!fechaFinProporcionada)
                errores.Add("La fecha de fin es obligatoria.");
            if (!fechaInicioProporcionada || !fechaFinProporcionada)
                return;
            if (actividad.FechaFin.Date < actividad.FechaInicio.Date)
            {
                errores.Add("La fecha de fin no puede ser anterior a la fecha de inicio.");
                return;
            }

            // DateTime.Date y horas en [0, 24h) permiten combinar incluso la fecha máxima sin desbordar.
            if (inicioValido && finValido)
            {
                DateTime inicio = actividad.FechaInicio.Date.Add(actividad.HoraInicio);
                DateTime fin = actividad.FechaFin.Date.Add(actividad.HoraFin);
                if (fin <= inicio)
                    errores.Add("El intervalo debe finalizar después de su inicio.");
            }
        }

        private static bool HoraValida(TimeSpan hora)
        {
            return hora >= TimeSpan.Zero && hora < TimeSpan.FromDays(1);
        }

        private void ValidarModalidadYLugar(ActividadDto actividad, List<string> errores)
        {
            if (!reglas.ModalidadPermitida(actividad.Modalidad))
                errores.Add("La modalidad debe pertenecer al catálogo permitido.");
            else if (reglas.RequiereLugar(actividad.Modalidad) && string.IsNullOrWhiteSpace(actividad.Lugar))
                errores.Add("El lugar es obligatorio para esta modalidad.");
        }
    }
}