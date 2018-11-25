using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Site.DAO;
using Site.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models
{
    public class UserModel : IDisposable
    {
        public int Id { get; set; }
        public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool PrimeiroAcesso { get; set; }

        public void Salvar(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                if (Id == 0)
                    dao.Insert(this, transaction);
                else
                    dao.Update(this, transaction);
            }
        }

        public void Remove(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                dao.Remove(this, transaction);
            }
        }

        public bool GetById(int id, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.GetById(this, id, transaction);
            }
        }

        public bool GetByNormalizedUserName(string normalizedNome, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.GetByNormalizedUserName(this, normalizedNome, transaction);
            }
        }

        public static IList<UserModel> GetUsersInRole(int roleId, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.GetUsersInRole(roleId, transaction);
            }
        }

        public static IList<UserModel> GetUsers(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.GetUsers(transaction);
            }
        }

        public static IList<UserModel> GetUsers(UserViewModels.FiltroViewModel filtro, int limitInitial, int limitCount, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.GetUsers(filtro, limitInitial, limitCount, transaction);
            }
        }

        public static int CountUsers(Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.CountUsers(transaction);
            }
        }

        public static int CountUsers(UserViewModels.FiltroViewModel filtro, Connection conn, MySqlTransaction transaction = null)
        {
            using (var dao = new UserDAO(conn))
            {
                return dao.CountUsers(filtro, transaction);
            }
        }

        public void Dispose()
        {

        }
    }
}
