using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Site.Models;
using Site.Utils;

namespace Site.DAO
{
    public class EstadoDAO : IDao<EstadoModel, MySqlTransaction>
    {
        private readonly Connection _connection;

        public EstadoDAO(Connection connection)
        {
            _connection = connection;
        }

        public int Insert(EstadoModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public int Remove(EstadoModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public int Update(EstadoModel model, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        private void DistributeData(EstadoModel model, DataRow dr)
        {
            model.Uf = dr["ufe_sg"].ToString();
            model.Nome = dr["ufe_no"].ToString();
        }

        public bool GetByUf(EstadoModel model, string uf, MySqlTransaction transaction = null)
        {
            string sql = "SELECT a.`ufe_sg`, a.`ufe_no`" +
                         " FROM `enderecamento`.`log_faixa_uf` a" +
                         " WHERE a.`ufe_sg` = @uf";

            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@uf", MySqlDbType.String) { Value = uf });

            DataTable dt = _connection.ExecuteReader(sql, parameters, transaction);

            if ((dt.Rows.Count == 0) || (dt.Rows.Count > 1))
                return false;

            DistributeData(model, dt.Rows[0]);

            return true;
        }

        public IList<EstadoModel> GetEstados(MySqlTransaction transaction = null)
        {
            string sql = "SELECT  a.`ufe_sg`, a.`ufe_no`" +
                         " FROM `enderecamento`.`log_faixa_uf` a";

            DataTable dt = _connection.ExecuteReader(sql, null, transaction);

            var estados = new List<EstadoModel>();

            foreach (DataRow row in dt.Rows)
            {
                var estado = new EstadoModel();

                DistributeData(estado, row);

                estados.Add(estado);
            }

            return estados;
        }

        public void Dispose() { }
    }
}
