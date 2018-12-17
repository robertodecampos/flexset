using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class CidadeModel : IDisposable
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public bool GetById(int id, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new CidadeDAO(conn))
            {
                return dao.GetById(this, id, transaction);
            }
        }

        public static IList<CidadeModel> GetAll(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new CidadeDAO(conn))
            {
                return dao.GetCidades(transaction);
            }
        }

        public static IList<CidadeModel> GetByUf(string uf, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new CidadeDAO(conn))
            {
                return dao.GetByUf(uf, transaction);
            }
        }

        public void Dispose() { }
    }
}
