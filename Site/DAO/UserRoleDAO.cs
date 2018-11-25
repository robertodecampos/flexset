using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Site.Exceptions;
using Site.Models;
using Site.Utils;

namespace Site.DAO
{
    public class UserRoleDAO : IDao<UserRoleModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public UserRoleDAO(Connection connection)
        {
            _connection = connection;
        }

        public int Insert(UserRoleModel model, MySqlTransaction transaction)
        {
            if (model.Id != 0)
                throw new SiteException("Não é possível inserir um registro que já possui um identificador!");

            UserRoleModel userRole = GetByIdUsuarioAndIdRoleDeleted(model.IdUsuario, model.IdRole, transaction);
            if (userRole != null)
                return Active(userRole, transaction);

            string sql = "INSERT INTO usuario_role (";
            sql += " usuarioId, roleId";
            sql += ") VALUES (";
            sql += " @usuarioId, @roleId";
            sql += ")";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = model.IdUsuario });
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.Int32) { Value = model.IdRole });

            int linhasAfetadas = _connection.Execute(sql, parameters, transaction);

            if (linhasAfetadas != 1)
                return linhasAfetadas;

            model.Id = _connection.UltimoIdInserido();

            return linhasAfetadas;
        }

        public int Remove(UserRoleModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE usuario_role SET";
            sql += " removido = 1";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Active(UserRoleModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE usuario_role SET";
            sql += " removido = 0";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Update(UserRoleModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível alterar um registro que não possui um identificador!");

            string sql = "UPDATE usuario_role SET";
            sql += " usuarioId = @usuarioId, roleId = @roleId";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.String) { Value = model.IdUsuario });
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.String) { Value = model.IdRole });

            return _connection.Execute(sql, parameters, transaction);
        }

        public bool GetByIdUsuarioAndIdRole(UserRoleModel userRole, int usuarioId, int roleId, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM usuario_role";
            sql += " WHERE usuarioId = @usuarioId AND roleId = @roleId";
            sql += " AND removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = usuarioId });
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.Int32) { Value = roleId });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(userRole, dt.Rows[0]);

            return true;
        }

        public bool GetByIdUsuarioAndNormalizedRole(UserRoleModel userRole, string normalizedRole, MySqlTransaction transaction = null)
        {
            string sql = "SELECT b.*";
            sql += " FROM role a";
            sql += " INNER JOIN usuario_role b ON a.id = b.roleId";
            sql += " WHERE a.normalizedNome = @normalizedNome";
            sql += " AND removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = normalizedRole });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(userRole, dt.Rows[0]);

            return true;
        }

        public bool UserInRole(int usuarioId, int roleId, MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.id";
            sql += " FROM usuario_role a";
            sql += " WHERE a.usuarioId = @usuarioId";
            sql += " AND a.roleId = @roleId";
            sql += " AND a.removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.String) { Value = usuarioId });
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.String) { Value = roleId });

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, null))
            {
                return (dt.Rows.Count > 0);
            }
        }

        public bool UserInRole(int usuarioId, string normalizedRoleName, MySqlTransaction transaction = null)
        {
            string sql = "SELECT b.id";
            sql += " FROM role a";
            sql += " INNER JOIN usuario_role b ON a.id = b.roleId AND b.usuarioId = @usuarioId AND b.removido = 0";
            sql += " WHERE a.normalizedNome = @normalizedNome";
            sql += " AND a.removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = usuarioId });
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = normalizedRoleName });

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, null))
            {
                return (dt.Rows.Count > 0);
            }
        }

        public IList<string> GetNameRolesByIdUsuario(int idUsuario, MySqlTransaction transaction = null)
        {
            string sql = "SELECT b.nome";
            sql += " FROM usuario_role a";
            sql += " INNER JOIN role b ON a.roleId = b.id";
            sql += " WHERE a.usuarioId = @usuarioId";
            sql += " AND a.removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = idUsuario });

            using (DataTable rows = _connection.ExecuteReader(sql, parameters, null))
            {
                var nomes = new List<string>();
                foreach (DataRow row in rows.Rows)
                    nomes.Add(row["nome"].ToString());
                return nomes;
            }
        }

        public IList<string> GetNormalizedNameRolesByIdUsuario(int idUsuario, MySqlTransaction transaction = null)
        {
            string sql = "SELECT b.normalizedNome";
            sql += " FROM usuario_role a";
            sql += " INNER JOIN role b ON a.roleId = b.id";
            sql += " WHERE a.usuarioId = @usuarioId";
            sql += " AND a.removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = idUsuario });

            using (DataTable rows = _connection.ExecuteReader(sql, parameters, null))
            {
                var nomes = new List<string>();
                foreach (DataRow row in rows.Rows)
                    nomes.Add(row["normalizedNome"].ToString());
                return nomes;
            }
        }

        private void DistributeData(UserRoleModel userRole, DataRow dr)
        {
            userRole.Id = int.Parse(dr["id"].ToString());
            userRole.IdUsuario = int.Parse(dr["usuarioId"].ToString());
            userRole.IdRole = int.Parse(dr["roleId"].ToString());
        }

        private UserRoleModel GetByIdUsuarioAndIdRoleDeleted(int usuarioId, int roleId, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM usuario_role";
            sql += " WHERE usuarioId = @usuarioId AND usuarioId = @usuarioId";
            sql += " AND removido = 1";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@usuarioId", MySqlDbType.Int32) { Value = usuarioId });
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.Int32) { Value = roleId });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return null;

            using (var userRole = new UserRoleModel())
            {
                DistributeData(userRole, dt.Rows[0]);

                return userRole;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
