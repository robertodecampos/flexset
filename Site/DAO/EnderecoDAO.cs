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
    public class EnderecoDAO : IDao<EnderecoModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public EnderecoDAO(Connection connection)
        {
            _connection = connection;
        }

        public int Insert(EnderecoModel model, MySqlTransaction transaction)
        {
            if (model.Id != 0)
                throw new SiteException("Não é possível inserir um registro que já possui um identificador!");

            if (model.IsValid().Count > 0)
                throw new SiteValidateException("O endereco contém algumas inconsistências!");

            string sql = "INSERT INTO `Endereco` (" +
                         "`ufEstado`, `idCidade`, `cep`,`logradouro`, `numero`, `complemento`, `bairro`" +
                         ") VALUES(" +
                         "@ufEstado, @idCidade, @cep, @logradouro, @numero, @complemento, @bairro" +
                         ")";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@ufEstado", MySqlDbType.String) { Value = model.Uf });
            parameters.Add(new MySqlParameter("@idCidade", MySqlDbType.Int32) { Value = model.IdCidade });
            parameters.Add(new MySqlParameter("@cep", MySqlDbType.String) { Value = model.Cep });
            parameters.Add(new MySqlParameter("@logradouro", MySqlDbType.String) { Value = model.Logradouro });
            parameters.Add(new MySqlParameter("@numero", MySqlDbType.String) { Value = model.Numero });
            parameters.Add(new MySqlParameter("@complemento", MySqlDbType.String) { Value = model.Complemento });
            parameters.Add(new MySqlParameter("@bairro", MySqlDbType.String) { Value = model.Bairro });

            int linhasAfetadas = _connection.Execute(sql, parameters, transaction);

            if (linhasAfetadas != 1)
                return linhasAfetadas;

            model.Id = _connection.UltimoIdInserido();

            return linhasAfetadas;
        }

        public int Remove(EnderecoModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível remover um registro que não possui um identificador!");

            string sql = "UPDATE `Endereco` SET";
            sql += " removido = 1";
            sql += " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });

            return _connection.Execute(sql, parameters, transaction);
        }

        public int Update(EnderecoModel model, MySqlTransaction transaction)
        {
            if (model.Id == 0)
                throw new SiteException("Não é possível alterar um registro que não possui um identificador!");

            if (model.IsValid().Count > 0)
                throw new SiteValidateException("O endereco contém algumas inconsistências!");

            string sql = "UPDATE `Endereco` SET" +
                         " `ufEstado` = @ufEstado, `idCidade` = @idCidade, `cep` = @cep, `logradouro` = @logradouro, `numero` = @numero, `complemento` = @complemento" +
                         " WHERE id = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = model.Id });
            parameters.Add(new MySqlParameter("@ufEstado", MySqlDbType.String) { Value = model.Uf });
            parameters.Add(new MySqlParameter("@idCidade", MySqlDbType.Int32) { Value = model.IdCidade });
            parameters.Add(new MySqlParameter("@cep", MySqlDbType.String) { Value = model.Cep });
            parameters.Add(new MySqlParameter("@logradouro", MySqlDbType.String) { Value = model.Logradouro });
            parameters.Add(new MySqlParameter("@numero", MySqlDbType.String) { Value = model.Numero });
            parameters.Add(new MySqlParameter("@complemento", MySqlDbType.String) { Value = model.Complemento });
            parameters.Add(new MySqlParameter("@bairro", MySqlDbType.String) { Value = model.Bairro });

            return _connection.Execute(sql, parameters, transaction);
        }

        public bool GetById(EnderecoModel model, int id, MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.`idEndereco`, a.`ufEstado`, a.`idCidade`, a.`cep`, a.`logradouro`, a.`numero`, a.`complemento`, a.`bairro`" +
                         " FROM `Endereco` a" +
                         " WHERE a.`idEndereco` = @id AND a.`removido` = 0";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = id });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(model, dt.Rows[0]);

            return true;
        }

        public bool SearchByCep(EnderecoModel model, string cep, MySqlTransaction transaction = null)
        {
            DataTable dt;
            string sql;
            List<MySqlParameter> parameters;

            sql = "SELECT a.`log_nome` logradouro, b.`bai_no` bairro, c.`loc_nu_sequencial` idCidade, d.`ufe_sg` ufEstado" +
                  " FROM `enderecamento`.`log_logradouro` a" +
                  " INNER JOIN `enderecamento`.`log_bairro` b ON b.`bai_nu_sequencial` IN(a.`bai_nu_sequencial_ini`, a.`bai_nu_sequencial_fim`)" +
                  " INNER JOIN `enderecamento`.`log_localidade` c ON b.`loc_nu_sequencial` = c.`loc_nu_sequencial`" +
                  " INNER JOIN `enderecamento`.`log_faixa_uf` d ON c.`ufe_sg` = d.`ufe_sg`" +
                  " WHERE a.`cep` = @cep";

            parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@cep", MySqlDbType.String) { Value = cep });

            dt = _connection.ExecuteReader(sql, parameters, transaction);

            if (dt.Rows.Count == 1)
            {
                model.Cep = cep;
                model.Logradouro = dt.Rows[0]["logradouro"].ToString();
                model.Bairro = dt.Rows[0]["bairro"].ToString();
                model.IdCidade = int.Parse(dt.Rows[0]["idCidade"].ToString());
                model.Uf = dt.Rows[0]["ufEstado"].ToString();

                return true;
            }

            sql = "SELECT a.`loc_nu_sequencial` idCidade, b.`ufe_sg` ufEstado" +
                  " FROM `enderecamento`.`log_localidade` a" +
                  " INNER JOIN `enderecamento`.`log_faixa_uf` b ON a.`ufe_sg` = b.`ufe_sg`" +
                  " WHERE a.`cep` = @cep";

            parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@cep", MySqlDbType.String) { Value = cep });

            dt = _connection.ExecuteReader(sql, parameters, transaction);

            if (dt.Rows.Count == 1)
            {
                model.Cep = cep;
                model.IdCidade = int.Parse(dt.Rows[0]["idCidade"].ToString());
                model.Uf = dt.Rows[0]["ufEstado"].ToString();

                return true;
            }

            sql = "SELECT b.`loc_nu_sequencial` idCidade, c.`ufe_sg` ufEstado" +
                   " FROM `enderecamento`.`log_faixa_localidade` a" +
                   " INNER JOIN `enderecamento`.`log_localidade` b ON a.`loc_nu_sequencial` = b.`loc_nu_sequencial`" +
                   " INNER JOIN `enderecamento`.`log_faixa_uf` c ON b.`ufe_sg` = c.`ufe_sg`" +
                   " WHERE(@part1 BETWEEN a.`loc_rad1_ini` AND a.`loc_rad1_fim` AND@ part2 BETWEEN a.`loc_suf1_ini` AND a.`loc_suf1_fim`)" +
                   " OR (@part1 BETWEEN a.`loc_rad2_ini` AND a.`loc_rad2_fim` AND@ part2 BETWEEN a.`loc_suf2_ini` AND a.`loc_suf2_fim`)";

            parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@part1", MySqlDbType.String) { Value = cep.Substring(0, 5) });
            parameters.Add(new MySqlParameter("@part2", MySqlDbType.String) { Value = cep.Substring(5, 3) });

            dt = _connection.ExecuteReader(sql, parameters, transaction);

            if (dt.Rows.Count == 1)
            {
                model.Cep = cep;
                model.IdCidade = int.Parse(dt.Rows[0]["idCidade"].ToString());
                model.Uf = dt.Rows[0]["ufEstado"].ToString();

                return true;
            }

            sql = "SELECT a.`ufe_sg` ufEstado" +
                  " FROM `enderecamento`.`log_faixa_uf` a" +
                  " WHERE(@part1 BETWEEN a.`ufe_rad1_ini` AND a.`ufe_rad1_fim` AND @part2 BETWEEN a.`ufe_suf1_ini` AND a.`ufe_suf1_fim`)" +
                  " OR (@part1 BETWEEN a.`ufe_rad2_ini` AND a.`ufe_rad2_fim` AND @part2 BETWEEN a.`ufe_suf2_ini` AND a.`ufe_suf2_fim`)";

            parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@part1", MySqlDbType.String) { Value = cep.Substring(0, 5) });
            parameters.Add(new MySqlParameter("@part2", MySqlDbType.String) { Value = cep.Substring(5, 3) });

            dt = _connection.ExecuteReader(sql, parameters, transaction);

            if (dt.Rows.Count == 1)
            {
                model.Cep = cep;
                model.Uf = dt.Rows[0]["ufEstado"].ToString();

                return true;
            }

            return false;
        }

        private void DistributeData(EnderecoModel endereco, DataRow dr)
        {
            endereco.Id = int.Parse(dr["idEndereco"].ToString());
            endereco.Uf = dr["ufEstado"].ToString();
            endereco.IdCidade = int.Parse(dr["idCidade"].ToString());
            endereco.Cep = dr["cep"].ToString();
            endereco.Logradouro = dr["logradouro"].ToString();
            endereco.Numero = dr["numero"].ToString();
            endereco.Complemento = dr["complemento"].ToString();
            endereco.Bairro = dr["bairro"].ToString();
        }

        public void Dispose() { }
    }
}
