using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Site.DAO
{
    interface IDao<in Object, in Transaction> : IDisposable
    {
        int Insert(Object model, Transaction transaction);

        int Update(Object model, Transaction transaction);

        int Remove(Object model, Transaction transaction);
    }
}
