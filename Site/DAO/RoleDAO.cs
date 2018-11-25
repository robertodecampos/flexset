using MySql.Data.MySqlClient;
using Site.Exceptions;
using Site.Models;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Site.DAO
{
    public class RoleDAO : IDao<RoleModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public RoleDAO(Connection connection)
        {
            _connection = connection;
        }

        private void DistributeData(RoleModel role, DataRow dr)
        {
            role.Id = int.Parse(dr["id"].ToString());
            role.Nome = dr["nome"].ToString();
            role.NormalizedNome = dr["normalizedNome"].ToString();
        }

        public int Insert(RoleModel role, MySqlTransaction transaction)
        {
            if (role.Id != 0)
                throw new SiteException("Não é possível inserir um registro que já possui um identificador!");

            string sql = "INSERT INTO Role (";
            sql += " nome, normalizedNome";
            sql += ") VALUES (";
            sql += " @nome, @normalizedNome";
            sql += ")";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = role.Nome });
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = role.NormalizedNome });

            int linhasAfetadas = _connection.Execute(sql, parameters, transaction);

            if (linhasAfetadas != 1)
                return linhasAfetadas;

            role.Id = _connection.UltimoIdInserido();

            return linhasAfetadas;
        }

        public int Update(RoleModel role, MySqlTransaction transaction)
        {
            if (role.Id == 0)
                throw new SiteException("Não é possível alterar um registro que não possui um identificador!");

            string sql = "UPDATE Role SET";
            sql += " nome = @nome, normalizedNome = @normalizedNome";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = role.Id });
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = role.Nome });
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = role.NormalizedNome });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Remove(RoleModel role, MySqlTransaction transaction)
        {
            if (role.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE Role SET";
            sql += " removido = 1";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = role.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        public bool GetById(RoleModel role, int id, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Role";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = role.Id });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(role, dt.Rows[0]);

            return true;
        }

        public bool GetByNomeNormalized(RoleModel role, string normalizedNome, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Role";
            sql += " WHERE normalizedNome = @normalizedNome";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = normalizedNome });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(role, dt.Rows[0]);

            return true;
        }

        public bool GetByNome(RoleModel role, string nome, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Role";
            sql += " WHERE nome = @nome";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = nome });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(role, dt.Rows[0]);

            return true;
        }

        public IList<RoleModel> GetRoles(MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Role";
            sql += " WHERE removido = 0";

            DataTable dt = _connection.ExecuteReader(sql, null, transaction);

            var roles = new List<RoleModel>();

            foreach (DataRow row in dt.Rows)
            {
                var role = new RoleModel();

                DistributeData(role, row);

                roles.Add(role);
            }

            return roles;
        }

        public void Dispose()
        {
            // Destruir objetos
        }
    }
}
