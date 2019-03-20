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
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private Producer<Null, byte[]> _producer;
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

        /// <summary>
        /// send a single message to kafka topic
        /// https://github.com/confluentinc/confluent-kafka-dotnet/issues/770
        /// https://github.com/confluentinc/confluent-kafka-dotnet/issues/803
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="partition">partition</param>
        /// <returns></returns>
        public async Task ProduceAsync(T message, int partition)
        {
            var producer = GetProducer();

            try
            {
                var dr = await producer.ProduceAsync(new TopicPartition(_topic, new Partition(partition)), new Message<Null, byte[]>
                {
                    Value = MessagePack.MessagePackSerializer.Serialize(message)
                });
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError(e, $"Delivery failed: {e.Error.Reason}");
            }
        }

        /// <summary>
        /// send a single message to kafka topic
        /// https://github.com/confluentinc/confluent-kafka-dotnet/issues/770
        /// https://github.com/confluentinc/confluent-kafka-dotnet/issues/803
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        public async Task ProduceAsync(T message)=>
            await ProduceAsync(message, Partition.Any.Value);



        public async Task ProcessAsync(Func<T, CancellationToken, Task> action, CancellationToken stoppingToken, string groupId)
        {
            _kafkaOptions.ConsumerConfig.GroupId = groupId;

            using (var c = new ConsumerBuilder<Ignore, byte[]>(_kafkaOptions.ConsumerConfig).Build())
            {
                c.Subscribe(_topic);

                try
                {
                    const int commitPeriod = 50;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = c.Consume(stoppingToken);

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

        private Producer<Null, byte[]> GetProducer()
        {
            if (_producer == null)
            {
                _connectionLock.Wait();
                try
                {
                    _producer = new ProducerBuilder<Null, byte[]>(_kafkaOptions.ProducerConfig).Build();
                }
                catch
                {
                    _connectionLock.Release();
                }
            }

            return _producer;
        }
    }
}
