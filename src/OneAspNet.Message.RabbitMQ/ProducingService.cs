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
        private readonly ConcurrentDictionary<string, Lazy<RabbitmqBus>> entries = new ConcurrentDictionary<string, Lazy<RabbitmqBus>>(StringComparer.OrdinalIgnoreCase);
        private readonly RabbitmqOptions _rabbitmqOptions;

        public ProducingService(IOptions<RabbitmqOptions> rabbitmqOptionsAccessor)
        {
            _rabbitmqOptions = rabbitmqOptionsAccessor.Value;
        }


        public void Send(string connectionName, string exchange, string routingKey, string message, bool mandatory = false, Action<IBasicProperties> action = null)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException("is null or empty", nameof(connectionName));
            }

            var bus = entries.GetOrAdd(connectionName, key => new Lazy<RabbitmqBus>(() => CreateBus(key)));

            IBasicProperties basicProperties = default;
            if (action != null)
            {
                basicProperties = bus.Value.Channel.CreateBasicProperties();
                action(basicProperties);
            }

            var body = Encoding.UTF8.GetBytes(message);
            bus.Value.Channel.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
        }

        private RabbitmqBus CreateBus(string key)
        {
            var entry = _rabbitmqOptions.RabbitmqConnections.FirstOrDefault(e => e.ConnectionName.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                throw new ArgumentException($"无效的参数：{key}", nameof(key));
            }

            var connection = entry.ConnectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            if (entry.CreateModelAfter != null)
            {
                entry.CreateModelAfter(channel);
            }

            return new RabbitmqBus
            {
                Connection = connection,
                Channel = channel
            };
        }
    }
}
