using System;
using System.Configuration;
using System.Data.SqlClient;

namespace UniCampus.DAL.Connection
{
    /// <summary>
    /// Punto único de acceso a la conexión con SQL Server.
    /// Lee la cadena "UniCampusDb" del Web.config; los repositorios no
    /// deben leer la configuración ni conocer la cadena directamente.
    /// </summary>
    public static class DbConnection
    {
        private const string NombreCadena = "UniCampusDb";

        public static string ConnectionString
        {
            get
            {
                var settings = ConfigurationManager.ConnectionStrings[NombreCadena];

                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    // Mensaje genérico: no revela datos del servidor ni de la cadena.
                    throw new InvalidOperationException(
                        "No se encontró la configuración de la base de datos '" + NombreCadena + "'.");
                }

                return settings.ConnectionString;
            }
        }

        /// <summary>Crea una conexión nueva (cerrada). Quien la use debe cerrarla con 'using'.</summary>
        public static SqlConnection Crear()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
