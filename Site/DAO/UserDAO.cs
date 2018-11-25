using System;
using System.Collections.Generic;
using Site.Utils;
using Site.Models;
using Site.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using Site.Models.UserViewModels;

namespace Site.DAO
{
    public class UserDAO : IDao<UserModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public UserDAO(Connection connection)
        {
            _connection = connection;
        }

        public int Insert(UserModel user, MySqlTransaction transaction = null)
        {
            if (user.Id != 0)
                throw new SiteException("Não é possível inserir um registro que já possui um identificador!");

            string sql = "INSERT INTO Usuario (";
            sql += " email, cpf, nome, normalizedNome, passwordHash, dataNascimento, primeiroAcesso";
            sql += ") VALUES (";
            sql += " @email, @cpf, @nome, @normalizedNome, @passwordHash, @dataNascimento, @primeiroAcesso";
            sql += ")";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@email", MySqlDbType.String) { Value = user.Email });
            parameters.Add(new MySqlParameter("@cpf", MySqlDbType.String) { Value = user.Cpf });
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = user.Nome });
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = user.NormalizedUserName });
            parameters.Add(new MySqlParameter("@passwordHash", MySqlDbType.String) { Value = user.PasswordHash });
            parameters.Add(new MySqlParameter("@dataNascimento", MySqlDbType.Date) { Value = user.DataNascimento });
            parameters.Add(new MySqlParameter("@primeiroAcesso", MySqlDbType.Int16) { Value = user.PrimeiroAcesso });

            int linhasAfetadas = _connection.Execute(sql, parameters, transaction);

            if (linhasAfetadas != 1)
                return linhasAfetadas;

            user.Id = _connection.UltimoIdInserido();

            return linhasAfetadas;
        }

        public int Update(UserModel user, MySqlTransaction transaction = null)
        {
            if (user.Id == 0)
                throw new SiteException("Não é possível alterar um registro que não possui um identificador!");

            string sql = "UPDATE Usuario SET";
            sql += " email = @email, cpf = @cpf, nome = @nome,";
            sql += " normalizedNome = @normalizedNome, passwordHash = @passwordHash,";
            sql += " dataNascimento = @dataNascimento, primeiroAcesso = @primeiroAcesso";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = user.Id });
            parameters.Add(new MySqlParameter("@email", MySqlDbType.String) { Value = user.Email });
            parameters.Add(new MySqlParameter("@cpf", MySqlDbType.String) { Value = user.Cpf });
            parameters.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = user.Nome });
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = user.NormalizedUserName });
            parameters.Add(new MySqlParameter("@passwordHash", MySqlDbType.String) { Value = user.PasswordHash });
            parameters.Add(new MySqlParameter("@dataNascimento", MySqlDbType.Date) { Value = user.DataNascimento });
            parameters.Add(new MySqlParameter("@primeiroAcesso", MySqlDbType.Int16) { Value = user.PrimeiroAcesso });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Remove(UserModel user, MySqlTransaction transaction = null)
        {
            if (user.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE Usuario SET";
            sql += " removido = 1";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = user.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        private void DistributeData(UserModel user, DataRow dr)
        {
            user.Id = int.Parse(dr["id"].ToString());
            user.Email = dr["email"].ToString();
            user.Cpf = dr["cpf"].ToString();
            user.UserName = dr["cpf"].ToString();
            user.Nome = dr["nome"].ToString();
            user.NormalizedUserName = dr["normalizedNome"].ToString();
            user.PasswordHash = dr["passwordHash"].ToString();
            DateTime dataNascimento;
            if (DateTime.TryParse(dr["dataNascimento"].ToString(), out dataNascimento))
                user.DataNascimento = dataNascimento;
            user.PrimeiroAcesso = (dr["primeiroAcesso"].ToString() == "True");
        }

        public bool GetById(UserModel user, int id, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Usuario";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = id });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(user, dt.Rows[0]);

            return true;
        }

        public bool GetByNormalizedUserName(UserModel user, string normalizedUserName, MySqlTransaction transaction = null)
        {
            string sql = "SELECT *";
            sql += " FROM Usuario";
            sql += " WHERE normalizedNome = @normalizedNome";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@normalizedNome", MySqlDbType.String) { Value = normalizedUserName });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(user, dt.Rows[0]);

            return true;
        }

        public IList<UserModel> GetUsersInRole(int roleId, MySqlTransaction transaction = null)
        {
            string sql = "SELECT b.*";
            sql += " FROM usuario_role a";
            sql += " INNER JOIN usuario b ON a.usuarioId = b.id";
            sql += " WHERE a.roleId = @roleId";
            sql += " AND a.removido = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@roleId", MySqlDbType.String) { Value = roleId });

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, transaction))
            {
                var users = new List<UserModel>();

                foreach (DataRow row in dt.Rows)
                {
                    var user = new UserModel();
                    DistributeData(user, row);
                    users.Add(user);
                }

                return users;
            }
        }

        public IList<UserModel> GetUsers(MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.*";
            sql += " FROM usuario a";
            sql += " WHERE a.removido = 0";

            var parameters = new List<MySqlParameter>();

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, transaction))
            {
                var users = new List<UserModel>();

                foreach (DataRow row in dt.Rows)
                {
                    var user = new UserModel();
                    DistributeData(user, row);
                    users.Add(user);
                }

                return users;
            }
        }

        public IList<UserModel> GetUsers(FiltroViewModel filtro, int limitInitial, int limitCount, MySqlTransaction transaction = null)
        {
            var parameters = new List<MySqlParameter>();

            string sql = "SELECT a.*";
            sql += " FROM usuario a";
            sql += " INNER JOIN usuario_role b ON a.id = b.usuarioId AND b.removido = 0";
            sql += " WHERE " + Filtrar(filtro, "a", "b", parameters);
            sql += " GROUP BY a.id";
            sql += $" LIMIT {limitInitial}, {limitCount}";

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, transaction))
            {
                var users = new List<UserModel>();

                foreach (DataRow row in dt.Rows)
                {
                    var user = new UserModel();
                    DistributeData(user, row);
                    users.Add(user);
                }

                return users;
            }
        }

        public int CountUsers(MySqlTransaction transaction = null)
        {
            string sql = "SELECT COUNT(a.id) qtde";
            sql += " FROM usuario a";
            sql += " WHERE a.removido = 0";

            var parameters = new List<MySqlParameter>();

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, transaction))
            {
                return int.Parse(dt.Rows[0]["qtde"].ToString());
            }
        }

        public int CountUsers(FiltroViewModel filtro, MySqlTransaction transaction = null)
        {
            var parameters = new List<MySqlParameter>();

            string sql = "SELECT COUNT(DISTINCT a.id) qtde";
            sql += " FROM usuario a";
            sql += " INNER JOIN usuario_role b ON a.id = b.usuarioId AND b.removido = 0";
            sql += " WHERE " + Filtrar(filtro, "a", "b", parameters);

            using (DataTable dt = _connection.ExecuteReader(sql, parameters, transaction))
            {
                return int.Parse(dt.Rows[0]["qtde"].ToString());
            }
        }

        public void Dispose()
        {
            // Destruir objetos necessários
        }

        private string Filtrar(FiltroViewModel filtro, string aliasUsuario, string aliasusuarioRole, IList<MySqlParameter> parametros)
        {
            if (parametros == null)
                throw new Exceptions.SiteException("O parâmetro \"parametros\" não pode ser nulo!");

            if (aliasUsuario != "")
                aliasUsuario += ".";
            if (aliasusuarioRole != "")
                aliasusuarioRole += ".";

            string sql = "a.removido = 0";

            if ((filtro.Nome != null) && (filtro.Nome.Trim() != ""))
            {
                sql += $" AND {aliasUsuario}nome LIKE @nome";
                parametros.Add(new MySqlParameter("@nome", MySqlDbType.String) { Value = $"%{filtro.Nome.Trim()}%" });
            }
            if ((filtro.Cpf != null) && (filtro.Cpf.Trim() != ""))
            {
                sql += $" AND {aliasUsuario}cpf LIKE @cpf";
                parametros.Add(new MySqlParameter("@cpf", MySqlDbType.String) { Value = $"%{filtro.Cpf.Trim()}%" });
            }
            if ((filtro.Email != null) && (filtro.Email?.Trim() != ""))
            {
                sql += $" AND {aliasUsuario}email LIKE @email";
                parametros.Add(new MySqlParameter("@email", MySqlDbType.String) { Value = $"%{filtro.Email.Trim()}%" });
            }
            if (filtro.Dia > 0)
            {
                sql += $" AND DAY({aliasUsuario}dataNascimento) = @dia";
                parametros.Add(new MySqlParameter("@dia", MySqlDbType.Int32) { Value = filtro.Dia });
            }
            if (filtro.Mes > 0)
            {
                sql += $" AND MONTH({aliasUsuario}dataNascimento) = @mes";
                parametros.Add(new MySqlParameter("@mes", MySqlDbType.Int32) { Value = filtro.Mes });
            }
            if (filtro.Ano > 0)
            {
                sql += $" AND YEAR({aliasUsuario}dataNascimento) = @ano";
                parametros.Add(new MySqlParameter("@ano", MySqlDbType.Int32) { Value = filtro.Ano });
            }
            if ((filtro.DataNascimentoInicial != null) && (filtro.DataNascimentoFinal != null))
            {
                sql += $" AND {aliasUsuario}dataNascimento BETWEEN @dataNascimentoInicial AND @dataNascimentoFinal";
                parametros.Add(new MySqlParameter("@dataNascimentoInicial", MySqlDbType.String) { Value = ((DateTime)filtro.DataNascimentoInicial).ToString("yyyy-MM-dd") });
                parametros.Add(new MySqlParameter("@dataNascimentoFinal", MySqlDbType.String) { Value = ((DateTime)filtro.DataNascimentoFinal).ToString("yyyy-MM-dd") });
            }
            else if (filtro.DataNascimentoInicial != null)
            {
                sql += $" AND {aliasUsuario}dataNascimento >= @dataNascimentoInicial";
                parametros.Add(new MySqlParameter("@dataNascimentoInicial", MySqlDbType.String) { Value = ((DateTime)filtro.DataNascimentoInicial).ToString("yyyy-MM-dd") });
            }
            else if (filtro.DataNascimentoFinal != null)
            {
                sql += $" AND {aliasUsuario}dataNascimento <= @dataNascimentoFinal";
                parametros.Add(new MySqlParameter("@dataNascimentoFinal", MySqlDbType.String) { Value = ((DateTime)filtro.DataNascimentoFinal).ToString("yyyy-MM-dd") });
            }
            if ((filtro.Funcoes != null) && (filtro.Funcoes.Count > 0))
            {
                sql += $" AND {aliasusuarioRole}roleId IN (";
                for (int i = 0; i < filtro.Funcoes.Count; i++)
                {
                    sql += $"{filtro.Funcoes[i]}";
                    if (i < (filtro.Funcoes.Count - 1))
                        sql += ", ";
                }
                sql += ")";
            }

            return sql;
        }
    }
}
