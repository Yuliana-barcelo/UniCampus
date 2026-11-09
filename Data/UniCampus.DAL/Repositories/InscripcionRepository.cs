using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.DAL.Repositories
{
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly string connectionString;

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
                FROM Inscripcion i
                INNER JOIN Usuario u ON u.UsuarioId = i.UsuarioId
                WHERE i.ActividadId = @ActividadId
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

        public int ContarInscritos(int actividadId)
        {
            const string sql = "SELECT COUNT(*) FROM Inscripcion WHERE ActividadId = @ActividadId;";
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return (int)cmd.ExecuteScalar();
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
                        object cuposObj;
                        using (var cmd = new SqlCommand(
                            "SELECT Cupos FROM Actividad WITH (UPDLOCK, ROWLOCK) WHERE ActividadId = @ActividadId;", cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cuposObj = cmd.ExecuteScalar();
                        }
                        if (cuposObj == null)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.ActividadNoExiste;
                        }

                        // 2) Duplicado
                        int existe;
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM Inscripcion WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;", cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            existe = (int)cmd.ExecuteScalar();
                        }
                        if (existe > 0)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.Duplicada;
                        }

                        // 3) Cupo
                        int inscritos;
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM Inscripcion WHERE ActividadId = @ActividadId;", cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            inscritos = (int)cmd.ExecuteScalar();
                        }
                        if (inscritos >= (int)cuposObj)
                        {
                            tx.Rollback();
                            return ResultadoInscripcionDatos.SinCupo;
                        }

                        // 4) Inscribe
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO Inscripcion (ActividadId, UsuarioId, FechaInscripcion, Estado)
                              VALUES (@ActividadId, @UsuarioId, GETDATE(), 'Activa');", cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return ResultadoInscripcionDatos.Exito;
                    }
                    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                    {
                        // Violación de la restricción UNIQUE (ActividadId, UsuarioId)
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