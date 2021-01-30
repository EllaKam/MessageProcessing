using System;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Kafka.Dsl;
using Akka.Streams.Kafka.Messages;
using Akka.Streams.Kafka.Settings;
using Confluent.Kafka;
using Config = Akka.Configuration.Config;

namespace SimulatorDataProviderService
{
    class Program
    {
        static void Main(string[] args)
        {
            Config fallbackConfig = ConfigurationFactory.ParseString(@"
                    akka.suppress-json-serializer-warning=true
                    akka.loglevel = DEBUG
                ").WithFallback(ConfigurationFactory.FromResource<ConsumerSettings<object, object>>("Akka.Streams.Kafka.reference.conf"));

            var system = ActorSystem.Create("TestKafka", fallbackConfig);
            var materializer = system.Materializer();

            var producerSettings = ProducerSettings<Null, string>.Create(system, null, null)
                .WithBootstrapServers("kafka:9092");

            Source
                .Cycle(() => Enumerable.Range(1, 100).GetEnumerator())
                .Select(c => c.ToString())
                .Select(elem => ProducerMessage.Single(new ProducerRecord<Null, string>("CovidMessages", elem)))
                .Via(KafkaProducer.FlexiFlow<Null, string, NotUsed>(producerSettings))
                .Select(result =>
                {
                    var response = result as Result<Null, string, NotUsed>;
                    Console.WriteLine($"Producer: {response.Metadata.Topic}/{response.Metadata.Partition} {response.Metadata.Offset}: {response.Metadata.Value}");
                    return result;
                })
                .RunWith(Sink.Ignore<IResults<Null, string, NotUsed>>(), materializer);

            Console.ReadLine();
        }
    }
}
