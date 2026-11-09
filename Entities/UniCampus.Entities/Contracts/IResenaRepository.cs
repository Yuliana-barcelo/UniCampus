using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.DAL.Repositories
{
    public class ResenaRepository : IResenaRepository
    {
        private readonly string connectionString;

        public ResenaRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión es obligatoria.", nameof(connectionString));
            this.connectionString = connectionString;
        }

        public int Guardar(ResenaDto resena)
        {
            const string sql = @"
                INSERT INTO Resena (ActividadId, UsuarioId, Calificacion, Comentario, Fecha)
                VALUES (@ActividadId, @UsuarioId, @Calificacion, @Comentario, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = resena.ActividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = resena.UsuarioId;
                cmd.Parameters.Add("@Calificacion", SqlDbType.Int).Value = resena.Calificacion;
                cmd.Parameters.Add("@Comentario", SqlDbType.NVarChar, 500).Value =
                    (object)resena.Comentario ?? DBNull.Value;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool ExisteResena(int actividadId, int usuarioId)
        {
            const string sql = "SELECT COUNT(*) FROM Resena WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public IList<ResenaDto> ListarPorActividad(int actividadId)
        {
            const string sql = @"
                SELECT r.ResenaId, r.ActividadId, r.UsuarioId, u.Nombre,
                       r.Calificacion, r.Comentario, r.Fecha
                FROM Resena r
                INNER JOIN Usuario u ON u.UsuarioId = r.UsuarioId
                WHERE r.ActividadId = @ActividadId
                ORDER BY r.Fecha DESC;";

            var lista = new List<ResenaDto>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ResenaDto
                        {
                            ResenaId = rd.GetInt32(0),
                            ActividadId = rd.GetInt32(1),
                            UsuarioId = rd.GetInt32(2),
                            NombreUsuario = rd.GetString(3),
                            Calificacion = rd.GetInt32(4),
                            Comentario = rd.IsDBNull(5) ? string.Empty : rd.GetString(5),
                            Fecha = rd.GetDateTime(6)
                        });
                    }
                }
            }
            return lista;
        }
    }
}