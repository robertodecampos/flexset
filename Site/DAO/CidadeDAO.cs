using MySql.Data.MySqlClient;
using Site.Models;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Site.DAO
{
    public class CidadeDAO : IDao<CidadeModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public CidadeDAO(Connection connection)
        {
            _connection = connection;
        }

        public int Insert(CidadeModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public int Remove(CidadeModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public int Update(CidadeModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool GetById(CidadeModel model, int id, MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.`loc_nu_sequencial` idCidade, a.`loc_no` cidade" +
                         " FROM `enderecamento`.`log_localidade` a" +
                         " WHERE a.`loc_nu_sequencial` = @id";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = id });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(model, dt.Rows[0]);

            return true;
        }

        public IList<CidadeModel> GetCidades(MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.`loc_nu_sequencial` idCidade, a.`loc_no` cidade" +
                         " FROM `enderecamento`.`log_localidade` a";

            DataTable dt = _connection.ExecuteReader(sql, null, transaction);

            var cidades = new List<CidadeModel>();

            foreach (DataRow row in dt.Rows)
            {
                var cidade = new CidadeModel();

                DistributeData(cidade, row);

                cidades.Add(cidade);
            }

            return cidades;
        }

        public IList<CidadeModel> GetByUf(string uf, MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.`loc_nu_sequencial` idCidade, a.`loc_no` cidade" +
                         " FROM `enderecamento`.`log_localidade` a" +
                         " WHERE a.`ufe_sg` = @uf";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@uf", MySqlDbType.String) { Value = uf });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            var cidades = new List<CidadeModel>();

            foreach (DataRow row in dt.Rows)
            {
                var cidade = new CidadeModel();

                DistributeData(cidade, row);

                cidades.Add(cidade);
            }

            return cidades;
        }

        private void DistributeData(CidadeModel cidade, DataRow dr)
        {
            cidade.Id = int.Parse(dr["idCidade"].ToString());
            cidade.Nome = dr["cidade"].ToString();
        }

        public void Dispose() { }
    }
}
