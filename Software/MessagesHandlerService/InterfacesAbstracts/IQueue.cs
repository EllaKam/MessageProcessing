using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesHandlerService
{
    interface IQueue
    {
        void Init(string host, string groupName, string queueName);
        void Work();
    }
}
