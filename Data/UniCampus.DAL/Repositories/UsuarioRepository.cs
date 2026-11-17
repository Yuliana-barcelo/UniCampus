using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.DAL.Connection;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.DAL.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public UsuarioDto ObtenerPorCorreo(string correo)
        {
            const string sql = @"
                SELECT UsuarioId, Nombre, Correo, Programa, Rol
                FROM dbo.Usuario
                WHERE Correo = @Correo AND Estado = N'Activo';";

            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    return rd.Read() ? Mapear(rd) : null;
                }
            }
        }

        public IList<UsuarioDto> ListarEstudiantesActivos()
        {
            const string sql = @"
                SELECT UsuarioId, Nombre, Correo, Programa, Rol
                FROM dbo.Usuario
                WHERE Rol = N'Estudiante' AND Estado = N'Activo'
                ORDER BY Nombre;";

            var lista = new List<UsuarioDto>();
            using (var cn = DbConnection.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) lista.Add(Mapear(rd));
                }
            }
            return lista;
        }

        private static UsuarioDto Mapear(SqlDataReader rd)
        {
            return new UsuarioDto
            {
                UsuarioId = rd.GetInt32(0),
                Nombre = rd.GetString(1),
                Correo = rd.GetString(2),
                Programa = rd.IsDBNull(3) ? string.Empty : rd.GetString(3),
                Rol = rd.GetString(4)
            };
        }
    }
}
