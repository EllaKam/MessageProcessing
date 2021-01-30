using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace MessagesHandlerService
{
    public class DBFactory
    {
        private static DBFactory _intance;
        private static readonly object _lockObject = new object();

        private DBFactory()
        {

        }

        public static DBFactory Istance
        {
            get
            {
                if (_intance == null)
                {
                    lock (_lockObject)
                    {
                        if (_intance == null)
                        {
                            _intance = new DBFactory();
                        }
                    }
                }

                return _intance;
            }
        }
        private string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        public DBService GetDBService()
        {
            bool isDB = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDB"]);

            DBService service =   (isDB) ? (DBService) new SQLDBServer(_connectionString) : new DBServiceMock(_connectionString);
            return service;
        }
    }
}
