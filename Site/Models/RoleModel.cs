using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class RoleModel : IDisposable
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string NormalizedNome { get; set; }

        public void Salvar(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new RoleDAO(conn))
            {
                if (Id == 0)
                    dao.Insert(this, transaction);
                else
                    dao.Update(this, transaction);
            }
        }

        public bool GetById(int id, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new RoleDAO(conn))
            {
                return dao.GetById(this, id, transaction);
            }
        }

        public bool GetByNomeNormalized(string normalizedNome, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new RoleDAO(conn))
            {
                return dao.GetByNomeNormalized(this, normalizedNome, transaction);
            }
        }

        public bool GetByNome(string nome, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new RoleDAO(conn))
            {
                return dao.GetByNome(this, nome, transaction);
            }
        }

        public static IList<RoleModel> GetRoles(Connection conn, MySqlTransaction transaction)
        {
            using (var dao = new RoleDAO(conn))
            {
                return dao.GetRoles(transaction);
            }
        }

        public void Dispose()
        {

        }
    }
}
