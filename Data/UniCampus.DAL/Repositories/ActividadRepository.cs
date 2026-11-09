using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.DAL.Connection;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>
    /// Acceso a datos de actividades. Solo ejecuta SQL parametrizado:
    /// no valida reglas de negocio ni conoce controles de la interfaz.
    /// </summary>
    public class ActividadRepository
    {
        /// <summary>Inserta una actividad y devuelve su nuevo ActividadId.</summary>
        public int Guardar(ActividadDto actividad)
        {
            const string sql = @"
                INSERT INTO dbo.ActividadAcademica
                    (Titulo, Descripcion, TipoId, FechaInicio, FechaFin,
                     HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado)
                OUTPUT INSERTED.ActividadId
                VALUES
                    (@Titulo, @Descripcion, @TipoId, @FechaInicio, @FechaFin,
                     @HoraInicio, @HoraFin, @Modalidad, @Lugar, @Cupos, @Estado);";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 120).Value = actividad.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(actividad.Descripcion)
                        ? (object)DBNull.Value : actividad.Descripcion;
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = actividad.TipoId;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = actividad.FechaInicio.Date;
                cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = actividad.FechaFin.Date;
                cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = actividad.HoraInicio;
                cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = actividad.HoraFin;
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = actividad.Modalidad;
                cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = actividad.Lugar;
                cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = actividad.Cupos;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = actividad.Estado;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Lista las actividades no desactivadas. Si 'busqueda' trae texto,
        /// filtra por coincidencia en el título.
        /// </summary>
        public List<ActividadDto> Listar(string busqueda = null)
        {
            const string sql = @"
                SELECT a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre,
                       a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
                       a.Modalidad, a.Lugar, a.Cupos, a.Estado
                FROM dbo.ActividadAcademica a
                INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
                WHERE a.Estado <> N'Inactiva'
                  AND (@Busqueda IS NULL OR a.Titulo LIKE '%' + @Busqueda + '%')
                ORDER BY a.FechaInicio DESC, a.ActividadId DESC;";

            var resultado = new List<ActividadDto>();

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Busqueda", SqlDbType.NVarChar, 120).Value =
                    string.IsNullOrWhiteSpace(busqueda)
                        ? (object)DBNull.Value : busqueda.Trim();

                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultado.Add(new ActividadDto
                        {
                            ActividadId = reader.GetInt32(0),
                            Titulo = reader.GetString(1),
                            Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            TipoId = reader.GetInt32(3),
                            TipoNombre = reader.GetString(4),
                            FechaInicio = reader.GetDateTime(5),
                            FechaFin = reader.GetDateTime(6),
                            HoraInicio = reader.GetTimeSpan(7),
                            HoraFin = reader.GetTimeSpan(8),
                            Modalidad = reader.GetString(9),
                            Lugar = reader.GetString(10),
                            Cupos = reader.GetInt32(11),
                            Estado = reader.GetString(12)
                        });
                    }
                }
            }
            return resultado;
        }

        /// <summary>Tipos de actividad para llenar el desplegable del formulario.</summary>
        public List<TipoActividadDto> ObtenerTipos()
        {
            const string sql = "SELECT TipoId, Nombre FROM dbo.TipoActividad ORDER BY Nombre;";
            var resultado = new List<TipoActividadDto>();

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultado.Add(new TipoActividadDto
                        {
                            TipoId = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }
            return resultado;
        }
    }
}
