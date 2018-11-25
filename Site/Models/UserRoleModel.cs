using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class UserRoleModel : IDisposable
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdRole { get; set; }

        public void Salvar(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                if (Id == 0)
                    dao.Insert(this, transaction);
                else
                    dao.Update(this, transaction);
            }
        }

        public void Remover(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                dao.Remove(this, transaction);
            }
        }

        public bool GetByIdUsuarioAndIdRole(int usuarioId, int roleId, Connection conn, MySqlTransaction transaction = null)
        {

            using (var dao = new UserRoleDAO(conn))
            {
                return dao.GetByIdUsuarioAndIdRole(this, usuarioId, roleId);
            }
        }

        public static IList<string> GetNameRolesByIdUsuario(int usuarioId, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                return dao.GetNameRolesByIdUsuario(usuarioId, null);
            }
        }

        public static IList<string> GetNormalizedNameRolesByIdUsuario(int usuarioId, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                return dao.GetNormalizedNameRolesByIdUsuario(usuarioId, null);
            }
        }

        public static bool UserInRole(int usuarioId, int roleId, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                return dao.UserInRole(usuarioId, roleId, transaction);
            }
        }

        public static bool UserInRole(int usuarioId, string normalizedRoleName, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserRoleDAO(conn))
            {
                return dao.UserInRole(usuarioId, normalizedRoleName, transaction);
            }
        }

        public void Dispose()
        {

        }
    }
}
