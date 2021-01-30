using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesHandlerService
{
    public class AppDisplay
    {
        public  void Init()
        {
            MessageHandler.MessageShow += delegate(int message, bool trend) { Show(message, trend); };
        }

        private void Show(int message, bool trend)
        {
            Console.Write($"message", trend);
        }
    }
}
