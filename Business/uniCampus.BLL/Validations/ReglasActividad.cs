using System;
using System.Collections.Generic;

namespace uniCampus.BLL.Validations
{
    /// <summary>
    /// Catálogos explícitos del equipo. No define transiciones de estado.
    /// Se copian las colecciones para evitar cambios externos posteriores.
    /// </summary>
    public sealed class ReglasActividad
    {
        private readonly HashSet<string> modalidades;
        private readonly HashSet<string> modalidadesConLugar;
        private readonly HashSet<string> estados;

        public ReglasActividad(IEnumerable<string> modalidadesPermitidas,
                              IEnumerable<string> modalidadesConLugarObligatorio,
                              IEnumerable<string> estadosPermitidos)
        {
            modalidades = CrearCatalogo(modalidadesPermitidas, nameof(modalidadesPermitidas), false);
            modalidadesConLugar = CrearCatalogo(modalidadesConLugarObligatorio, nameof(modalidadesConLugarObligatorio), true);
            estados = CrearCatalogo(estadosPermitidos, nameof(estadosPermitidos), false);

            if (!modalidadesConLugar.IsSubsetOf(modalidades))
                throw new ArgumentException("Las modalidades con lugar obligatorio deben pertenecer al catálogo permitido.",
                                            nameof(modalidadesConLugarObligatorio));
        }

        internal bool ModalidadPermitida(string modalidad)
        {
            return modalidad != null && modalidades.Contains(modalidad);
        }

        internal bool RequiereLugar(string modalidad)
        {
            return modalidad != null && modalidadesConLugar.Contains(modalidad);
        }

        internal bool EstadoPermitido(string estado)
        {
            return estado != null && estados.Contains(estado);
        }

        private static HashSet<string> CrearCatalogo(IEnumerable<string> valores, string parametro, bool permiteVacio)
        {
            if (valores == null)
                throw new ArgumentNullException(parametro);

            var catalogo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var valor in valores)
            {
                if (string.IsNullOrWhiteSpace(valor))
                    throw new ArgumentException("Los catálogos no pueden contener valores nulos o en blanco.", parametro);
                catalogo.Add(valor);
            }

            if (!permiteVacio && catalogo.Count == 0)
                throw new ArgumentException("El catálogo debe contener al menos un valor permitido.", parametro);

            return catalogo;
        }
    }
}