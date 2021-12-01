using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace OneAspNet.Message.RabbitMQ
{
    public class ProducingService : IProducingService
    {
        private readonly ConcurrentDictionary<string, Lazy<(IConnection, IModel)>> _channels = new ConcurrentDictionary<string, Lazy<(IConnection, IModel)>>(StringComparer.OrdinalIgnoreCase);

        public ProducingService(IOptions<RabbitmqOptions> optionsAccessor)
        {

        }

        public void Send(string clientProvidedName, string message)
        {
            if (string.IsNullOrWhiteSpace(clientProvidedName))
            {
                throw new ArgumentException("is null or empty", nameof(clientProvidedName));
            }

            var item = _channels.GetOrAdd(clientProvidedName, key => new Lazy<(IConnection, IModel)>(() => CreateConnection(key)));

            var body = Encoding.UTF8.GetBytes(message);
            item.Value.Item2.BasicPublish("", "queue_red", null, body);

        }

        private (IConnection, IModel) CreateConnection(string key)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection(key);
            var channel = connection.CreateModel();

            return (connection, channel);
        }
    }
}
