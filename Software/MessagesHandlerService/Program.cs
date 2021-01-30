using System;
using System.Configuration;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Kafka.Dsl;
using Akka.Streams.Kafka.Helpers;
using Akka.Streams.Kafka.Messages;
using Akka.Streams.Kafka.Settings;
using Confluent.Kafka;
using Config = Akka.Configuration.Config;

namespace MessagesHandlerService
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDisplay appDisplay = new AppDisplay();
            appDisplay.Init();
            QueueWorker worker =  new QueueWorker();
            string host = ConfigurationManager.AppSettings["Host"];
            string group = ConfigurationManager.AppSettings["GroupName"];
            string queue = ConfigurationManager.AppSettings["QueueName"];
            worker.Init(host, group, queue);
            worker.Work();
            Console.ReadLine();
        }
        
    }
}
