using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneAspNet.Message.Kafka
{
    public class KafkaService<T>
    {
        private readonly ILogger _logger;
        private readonly KafkaOptions _kafkaOptions;
        private readonly string _topic;

        public KafkaService(IOptionsMonitor<KafkaOptions> kafkaOptionsAccessor, ILogger<KafkaService<T>> logger)
        {
            if (kafkaOptionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(kafkaOptionsAccessor));
            }
            _logger = logger;

            _kafkaOptions = kafkaOptionsAccessor.CurrentValue;
            _topic = GetTopicName(typeof(T));
        }

        public async Task ProduceAsync(T message, int partition = 0)
        {
            using (var p = new ProducerBuilder<Null, byte[]>(_kafkaOptions.ProducerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(_topic, new Message<Null, byte[]>
                    {
                        Value = MessagePack.MessagePackSerializer.Serialize(message)
                    });
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError(e, $"Delivery failed: {e.Error.Reason}");
                }
            }
        }

        public async Task ProcessAsync(Func<T, CancellationToken, Task> action, CancellationToken stoppingToken, string groupId)
        {
            _kafkaOptions.ConsumerConfig.GroupId = groupId;

            using (var c = new ConsumerBuilder<Ignore, byte[]>(_kafkaOptions.ConsumerConfig).Build())
            {
                c.Subscribe(_topic);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    const int commitPeriod = 50;
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);

                            var message = MessagePack.MessagePackSerializer.Deserialize<T>(cr.Value);
                            await action(message, stoppingToken);

                            if (cr.Offset.Value % commitPeriod == 0)
                            {
                                c.Commit(cr);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError(e, $"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }


        private string GetTopicName(Type type)
        {
            string topic = string.Empty;
            var attr = type
#if NETSTANDARD1_3
                    .GetTypeInfo()
#endif
                    .GetCustomAttributes(false).SingleOrDefault(a => a.GetType().Name == "TopicNameAttribute") as TopicNameAttribute;

            if (attr != null)
            {
                topic = attr.Name;
            }
            else
            {
                topic = $"{type.Name}";
            }

            return topic;
        }
    }
}
