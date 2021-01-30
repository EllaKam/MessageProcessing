using System;

namespace MessagesHandlerService
{
    public class MessageHandler 
    {
        private DBService _dbService;
        
        public MessageHandler()
        {
            _dbService = DBFactory.Istance.GetDBService();
        }
        public bool Process(int message)
        {
            bool result = _dbService.SetDelta(message);
            int newValue = _dbService.GetCurrentValue();
            if (newValue < 0)
            {
                return false;
            }
            bool trend = (message > 0);
            MessageShow(newValue, trend);
            return result;
        }

        
        public static event Action<int, bool> MessageShow;
    }
}
