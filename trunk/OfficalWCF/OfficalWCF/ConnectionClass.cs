using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace OfficalWCF
{
    public class ConnectionClass
    {
        private static ConnectionClass _instance;
        private DatabaseProviderFactory factory;
        private Database defaultDB;
        public static ConnectionClass GetInstance()
        {
            if (_instance == null)
                _instance = new ConnectionClass();
            return _instance;
        }

        private ConnectionClass()
        {
            factory = new DatabaseProviderFactory();
            defaultDB = factory.CreateDefault();
        }

        public IDataReader ExecuteReader(string sql)
        {
            DbCommand dbCommand = defaultDB.GetSqlStringCommand(sql);
            return defaultDB.ExecuteReader(dbCommand);
        }

        public int ExecuteNonQuery(string sql)
        {
            DbCommand dbCommand = defaultDB.GetSqlStringCommand(sql);
            int a;
            a = defaultDB.ExecuteNonQuery(dbCommand);
            return a;
        }

        public void Dispose()
        {
 
        }
    }
}