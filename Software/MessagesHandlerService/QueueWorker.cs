using System;
using System.Collections.Generic;
using System.Text;
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
    public class QueueWorker : IQueue
    {
        private ConsumerSettings<Null, string> _consumerSettings;
        private ActorSystem _system;
        private string _topic;
        private MessageHandler _messageHendler;
        public void Init(string host, string groupName, string queueName)
        {
            Config fallbackConfig = ConfigurationFactory.ParseString(@"
                    akka.suppress-json-serializer-warning=true
                    akka.loglevel = DEBUG
                ").WithFallback(ConfigurationFactory.FromResource<ConsumerSettings<object, object>>("Akka.Streams.Kafka.reference.conf"));

            _system = ActorSystem.Create("TestKafka", fallbackConfig);
            
            _consumerSettings = ConsumerSettings<Null, string>.Create(_system, null, null)
                .WithBootstrapServers(host)
                .WithGroupId(groupName);
            _topic = queueName;
            _messageHendler = new MessageHandler();
        }

        public void Work()
        {
            var committerDefaults = CommitterSettings.Create(_system);
            var materializer = _system.Materializer();
            var subscription = Subscriptions.Topics(_topic);
            DrainingControl<NotUsed> control = KafkaConsumer.CommittableSource(_consumerSettings, subscription)
                .SelectAsync(1, msg =>
                    Business(msg.Record).ContinueWith(done => (ICommittable)msg.CommitableOffset))
                .ToMaterialized(
                    Committer.Sink(committerDefaults.WithMaxBatch(1)),
                    DrainingControl<NotUsed>.Create)
                .Run(materializer);
        }

        private Task Business(ConsumeResult<Null, string> record)
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
            catch{}
            return result;
        }
    }
}
