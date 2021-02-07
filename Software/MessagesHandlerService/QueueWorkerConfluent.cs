using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MessagesHandlerService
{
    public class QueueWorkerConfluent : IQueue
    {
        private ConsumerConfig _config;
        private string _topic;
        private MessageHandler _messageHendler;
        public void Init(string host, string groupName, string queueName)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = host,
                GroupId = "csharp-consumer",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            _topic = queueName;
            _messageHendler = new MessageHandler();

        }

        public void Work()
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_config)
                 // Note: All handlers are called on the main .Consume thread.
                 .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                 .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                 .SetPartitionsAssignedHandler((c, partitions) =>
                 {
                     Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                 })
                 .SetPartitionsRevokedHandler((c, partitions) =>
                 {
                     Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                 })
                 .Build())
            {
                consumer.Subscribe(_topic);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume();
                            Business(consumeResult);
                            consumer.Commit(consumeResult);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }

        private Task Business(ConsumeResult<Ignore, string> record)
        {
            Task result = Task.FromException(new Exception("Error process"));
            try
            {
                int message = Convert.ToInt32(record.Message.Value);
                if (_messageHendler.Process(message))
                {
                    result = Task.CompletedTask;
                }
            }
            catch { }
            return result;
        }
    }
}
