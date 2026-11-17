using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.DAL.Connection;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.DAL.Repositories
{
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly string connectionString;

        public InscripcionRepository() : this(DbConnection.ConnectionString) { }

        public InscripcionRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión es obligatoria.", nameof(connectionString));
            this.connectionString = connectionString;
        }

        public IList<EstudianteInscritoDto> ListarPorActividad(int actividadId)
        {
            const string sql = @"
                SELECT i.InscripcionId, i.ActividadId, i.UsuarioId,
                       u.Nombre, u.Programa, i.FechaInscripcion
                FROM dbo.Inscripcion i
                INNER JOIN dbo.Usuario u ON u.UsuarioId = i.UsuarioId
                WHERE i.ActividadId = @ActividadId AND i.Estado = N'Activa'
                ORDER BY i.FechaInscripcion, i.InscripcionId;";

            var lista = new List<EstudianteInscritoDto>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new EstudianteInscritoDto
                        {
                            InscripcionId = rd.GetInt32(0),
                            ActividadId = rd.GetInt32(1),
                            UsuarioId = rd.GetInt32(2),
                            Nombre = rd.GetString(3),
                            Programa = rd.IsDBNull(4) ? string.Empty : rd.GetString(4),
                            FechaInscripcion = rd.GetDateTime(5)
                        });
                    }
                }
            }
            return lista;
        }

        public IList<MiInscripcionDto> ListarPorUsuario(int usuarioId)
        {
            const string sql = @"
                SELECT i.InscripcionId, a.ActividadId, a.Titulo, t.Nombre,
                       a.FechaInicio, a.FechaFin, a.Lugar, a.Estado, i.FechaInscripcion
                FROM dbo.Inscripcion i
                INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
                INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
                WHERE i.UsuarioId = @UsuarioId AND i.Estado = N'Activa' AND a.Estado <> N'Inactiva'
                ORDER BY a.FechaInicio DESC, i.InscripcionId DESC;";

            var lista = new List<MiInscripcionDto>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new MiInscripcionDto
                        {
                            InscripcionId = rd.GetInt32(0),
                            ActividadId = rd.GetInt32(1),
                            Titulo = rd.GetString(2),
                            TipoNombre = rd.GetString(3),
                            FechaInicio = rd.GetDateTime(4),
                            FechaFin = rd.GetDateTime(5),
                            Lugar = rd.GetString(6),
                            EstadoActividad = rd.GetString(7),
                            FechaInscripcion = rd.GetDateTime(8)
                        });
                    }
                }
            }
            return lista;
        }

        public int ContarInscritos(int actividadId)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.Inscripcion WHERE ActividadId = @ActividadId AND Estado = N'Activa';";

            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Cancelar(int inscripcionId, int usuarioSolicitanteId, bool esAdministrador)
        {
            const string sql = @"
                UPDATE dbo.Inscripcion SET Estado = N'Cancelada'
                WHERE InscripcionId = @InscripcionId AND Estado = N'Activa'
                  AND (@EsAdmin = 1 OR UsuarioId = @UsuarioId);";

            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@InscripcionId", SqlDbType.Int).Value = inscripcionId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioSolicitanteId;
                cmd.Parameters.Add("@EsAdmin", SqlDbType.Bit).Value = esAdministrador;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public ResultadoInscripcionDatos Inscribir(int actividadId, int usuarioId)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // 1) Bloquea la fila de la actividad: dos inscripciones simultáneas se atienden una tras otra.
                        int cupos = 0;
                        string estado = null;
                        bool vigente = false;
                        bool existe = false;
                        using (var cmd = new SqlCommand(@"
                            SELECT Cupos, Estado,
                                   CASE WHEN FechaFin >= CAST(SYSDATETIME() AS DATE) THEN 1 ELSE 0 END
                            FROM dbo.ActividadAcademica WITH (UPDLOCK, ROWLOCK)
                            WHERE ActividadId = @ActividadId;", cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    existe = true;
                                    cupos = rd.GetInt32(0);
                                    estado = rd.GetString(1);
                                    vigente = rd.GetInt32(2) == 1;
                                }
                            }
                        }
                        if (!existe)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.ActividadNoExiste;
                        }

                        // 2) La actividad debe estar Activa o Próxima y no haber terminado.
                        if (!(estado == "Activa" || estado == "Próxima") || !vigente)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.ActividadNoDisponible;
                        }

                        // 3) El usuario debe existir y estar activo.
                        int usuarios;
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM dbo.Usuario WHERE UsuarioId = @UsuarioId AND Estado = N'Activo';", cn, tx))
                        {
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            usuarios = (int)cmd.ExecuteScalar();
                        }
                        if (usuarios == 0)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.UsuarioNoValido;
                        }

                        // 4) Duplicado. UNIQUE (ActividadId, UsuarioId): una inscripción cancelada se reactiva.
                        int inscripcionPrevia = 0;
                        string estadoPrevio = null;
                        using (var cmd = new SqlCommand(
                            "SELECT InscripcionId, Estado FROM dbo.Inscripcion WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;",
                            cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    inscripcionPrevia = rd.GetInt32(0);
                                    estadoPrevio = rd.GetString(1);
                                }
                            }
                        }
                        if (inscripcionPrevia > 0 && estadoPrevio == "Activa")
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.Duplicada;
                        }

                        // 5) Cupo (solo cuentan las inscripciones activas)
                        int inscritos;
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM dbo.Inscripcion WHERE ActividadId = @ActividadId AND Estado = N'Activa';",
                            cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            inscritos = (int)cmd.ExecuteScalar();
                        }
                        if (inscritos >= cupos)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.SinCupo;
                        }

                        // 6) Inscribe o reactiva
                        string sqlFinal = inscripcionPrevia > 0
                            ? @"UPDATE dbo.Inscripcion SET Estado = N'Activa', FechaInscripcion = SYSUTCDATETIME()
                                WHERE InscripcionId = @InscripcionId;"
                            : @"INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId)
                                VALUES (@ActividadId, @UsuarioId);";
                        using (var cmd = new SqlCommand(sqlFinal, cn, tx))
                        {
                            if (inscripcionPrevia > 0)
                            {
                                cmd.Parameters.Add("@InscripcionId", SqlDbType.Int).Value = inscripcionPrevia;
                            }
                            else
                            {
                                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            }
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return ResultadoInscripcionDatos.Exito;
                    }
                    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                    {
                        tx.Rollback();
                        return ResultadoInscripcionDatos.Duplicada;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
