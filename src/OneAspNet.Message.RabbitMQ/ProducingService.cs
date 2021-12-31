using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneAspNet.Message.Rabbitmq.Internal;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace OneAspNet.Message.Rabbitmq
{
    public class ProducingService : IProducingService
    {
        private readonly ConcurrentDictionary<string, Lazy<RabbitmqEntry>> entries = new ConcurrentDictionary<string, Lazy<RabbitmqEntry>>(StringComparer.OrdinalIgnoreCase);
        private readonly RabbitmqOptions _rabbitmqOptions;

        public ProducingService(IOptions<RabbitmqOptions> rabbitmqOptionsAccessor)
        {
            _rabbitmqOptions = rabbitmqOptionsAccessor.Value;
        }

        public void Send(string connectionName, string exchange, string routingKey, string message, Action<IBasicProperties> action = null)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException("is null or empty", nameof(connectionName));
            }

            var entry = entries.GetOrAdd(connectionName, key => new Lazy<RabbitmqEntry>(() => CreateConnection(key)));

            IBasicProperties basicProperties = default;
            if (action != null)
            {
                basicProperties = entry.Value.Channel.CreateBasicProperties();
                action(basicProperties);
            }

            var body = Encoding.UTF8.GetBytes(message);
            entry.Value.Channel.BasicPublish(exchange, routingKey, false, basicProperties, body);
        }

        private RabbitmqEntry CreateConnection(string key)
        {
            var entry = _rabbitmqOptions.RabbitmqConnections.FirstOrDefault(e => e.ConnectionName == key);
            if (entry == null)
            {
                throw new ArgumentException($"无效的参数：{key}", nameof(key));
            }

            var connection = entry.ConnectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            return new RabbitmqEntry
            {
                Connection = connection,
                Channel = channel
            };
        }
    }
}
