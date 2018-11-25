using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Site.Utils
{
    public class Connection : IConnection, IDisposable
    {
        private readonly MySqlConnection _conn;

        public Connection(IConfiguration conf)
        {
            _conn = new MySqlConnection(conf.GetConnectionString("DefaultConnection"));
            _conn.Open();
        }

        private void SetParametersToCommand(MySqlCommand command, IList<MySqlParameter> parameters)
        {
            foreach (MySqlParameter parameter in parameters)
                command.Parameters.Add(parameter);
        }

        public int Execute(string sql, IList<MySqlParameter> parameters = null, MySqlTransaction transaction = null)
        {
            using (var command = new MySqlCommand(sql, _conn))
            {
                if (parameters != null)
                    SetParametersToCommand(command, parameters);

                if (transaction != null)
                    command.Transaction = transaction;

                return command.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteReader(string sql, IList<MySqlParameter> parameters = null, MySqlTransaction transaction = null)
        {
            using (var command = new MySqlCommand(sql, _conn))
            {
                if (parameters != null)
                    SetParametersToCommand(command, parameters);

                if (transaction != null)
                    command.Transaction = transaction;

                using (MySqlDataReader dr = command.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(dr);

                    dr.Close();

                    return dt;
                }
            }
        }

        public int UltimoIdInserido()
        {
            string sql = "SELECT @@IDENTITY id";

            using (var rows = ExecuteReader(sql, null, null)) { return int.Parse(rows.Rows[0]["id"].ToString()); }
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
