using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.DAL.Connection;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.DAL.Repositories
{
    /// <summary>
    /// Acceso a datos de actividades. Solo ejecuta SQL parametrizado:
    /// no valida reglas de negocio ni conoce controles de la interfaz.
    /// </summary>
    public class ActividadRepository : IActividadRepository
    {
        private const string Columnas = @"
            a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre,
            a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
            a.Modalidad, a.Lugar, a.Cupos, a.Estado";

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
                AgregarParametros(cmd, actividad);
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Actualizar(ActividadDto actividad)
        {
            const string sql = @"
                UPDATE dbo.ActividadAcademica
                   SET Titulo = @Titulo, Descripcion = @Descripcion, TipoId = @TipoId,
                       FechaInicio = @FechaInicio, FechaFin = @FechaFin,
                       HoraInicio = @HoraInicio, HoraFin = @HoraFin,
                       Modalidad = @Modalidad, Lugar = @Lugar, Cupos = @Cupos, Estado = @Estado
                 WHERE ActividadId = @ActividadId AND Estado <> N'Inactiva';";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                AgregarParametros(cmd, actividad);
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividad.ActividadId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Desactivar(int actividadId)
        {
            const string sql = @"
                UPDATE dbo.ActividadAcademica SET Estado = N'Inactiva'
                 WHERE ActividadId = @ActividadId AND Estado <> N'Inactiva';";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public ActividadDto ObtenerPorId(int actividadId)
        {
            string sql = @"SELECT " + Columnas + @"
                FROM dbo.ActividadAcademica a
                INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
                WHERE a.ActividadId = @ActividadId AND a.Estado <> N'Inactiva';";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? Mapear(reader) : null;
                }
            }
        }

        public PaginaActividadesDto Listar(string busqueda, int pagina, int tamPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamPagina < 1) tamPagina = 5;

            // El texto del usuario SOLO viaja como parámetro; además se escapan los comodines de LIKE.
            const string filtro = @"
                WHERE a.Estado <> N'Inactiva'
                  AND (@Patron IS NULL
                       OR a.Titulo LIKE @Patron ESCAPE N'\'
                       OR t.Nombre LIKE @Patron ESCAPE N'\'
                       OR a.Estado LIKE @Patron ESCAPE N'\'
                       OR a.Lugar  LIKE @Patron ESCAPE N'\'
                       OR CONVERT(NVARCHAR(10), a.FechaInicio, 103) LIKE @Patron ESCAPE N'\')";

            string sql =
                @"SELECT COUNT(*) FROM dbo.ActividadAcademica a
                  INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId " + filtro + @";
                  SELECT " + Columnas + @"
                  FROM dbo.ActividadAcademica a
                  INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId " + filtro + @"
                  ORDER BY a.FechaInicio DESC, a.ActividadId DESC
                  OFFSET @Offset ROWS FETCH NEXT @Tam ROWS ONLY;";

            var resultado = new PaginaActividadesDto
            {
                Items = new List<ActividadDto>(),
                Pagina = pagina,
                TamPagina = tamPagina
            };

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Patron", SqlDbType.NVarChar, 450).Value = CrearPatron(busqueda);
                cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = (pagina - 1) * tamPagina;
                cmd.Parameters.Add("@Tam", SqlDbType.Int).Value = tamPagina;

                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) resultado.Total = reader.GetInt32(0);
                    reader.NextResult();
                    while (reader.Read()) resultado.Items.Add(Mapear(reader));
                }
            }
            return resultado;
        }

        public IList<TipoActividadDto> ObtenerTipos()
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

        public ResumenDto ObtenerResumen()
        {
            const string sql = @"
                SELECT
                  (SELECT COUNT(*) FROM dbo.ActividadAcademica WHERE Estado <> N'Inactiva'),
                  (SELECT COUNT(DISTINCT i.UsuarioId)
                     FROM dbo.Inscripcion i
                     INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
                    WHERE i.Estado = N'Activa' AND a.Estado <> N'Inactiva'),
                  (SELECT COUNT(*) FROM dbo.ActividadAcademica WHERE Estado = N'Próxima'),
                  (SELECT COUNT(*) FROM dbo.ActividadAcademica WHERE Estado = N'Finalizada');";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    return new ResumenDto
                    {
                        TotalActividades = reader.GetInt32(0),
                        TotalInscritos = reader.GetInt32(1),
                        ActividadesProximas = reader.GetInt32(2),
                        ActividadesFinalizadas = reader.GetInt32(3)
                    };
                }
            }
        }

        public int ContarInscritosActivos(int actividadId)
        {
            const string sql = @"SELECT COUNT(*) FROM dbo.Inscripcion
                                 WHERE ActividadId = @ActividadId AND Estado = N'Activa';";
            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ---------------- auxiliares ----------------

        private static void AgregarParametros(SqlCommand cmd, ActividadDto a)
        {
            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 120).Value = a.Titulo;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value =
                string.IsNullOrWhiteSpace(a.Descripcion) ? (object)DBNull.Value : a.Descripcion;
            cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = a.TipoId;
            cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = a.FechaInicio.Date;
            cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = a.FechaFin.Date;
            cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = a.HoraInicio;
            cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = a.HoraFin;
            cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = a.Modalidad;
            // La columna Lugar es NOT NULL: nunca se envía null.
            cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = a.Lugar ?? string.Empty;
            cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = a.Cupos;
            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = a.Estado;
        }

        private static object CrearPatron(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda)) return DBNull.Value;
            string t = busqueda.Trim();
            if (t.Length > 100) t = t.Substring(0, 100);
            t = t.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
            return "%" + t + "%";
        }

        private static ActividadDto Mapear(SqlDataReader reader)
        {
            return new ActividadDto
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
            };
        }
    }
}
