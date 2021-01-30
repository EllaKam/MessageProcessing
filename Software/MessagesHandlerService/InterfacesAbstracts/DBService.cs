using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesHandlerService
{
    public abstract class DBService
    {
        protected string _connectionString;
        public DBService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public abstract int GetCurrentValue();
        public abstract bool SetDelta(int delta);
     
    }
}
