using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class EstadoModel
    {
        public string Uf { get; set; }
        public string Nome { get; set; }

        public bool GetByUf(string uf, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new EstadoDAO(conn))
            {
                return dao.GetByUf(this, uf, transaction);
            }
        }

        public static IList<EstadoModel> GetAll(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new EstadoDAO(conn))
            {
                return dao.GetEstados(transaction);
            }
        }
    }
}
