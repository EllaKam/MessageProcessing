using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesHandlerService
{
    public class DBServiceMock :DBService
    {
        public DBServiceMock(string connectionString) : base(connectionString)
        {
        }

        public override int GetCurrentValue()
        {
            return 5;
        }

        public override bool SetDelta(int currentValue)
        {
            return true;
        }
    }
}
