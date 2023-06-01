using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSample
{
    public class DuckConsumerBackgroundService : BackgroundService
    {
        private readonly ILogger<DuckConsumerBackgroundService> _logger;
        public DuckConsumerBackgroundService(ILogger<DuckConsumerBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = new ConnectionFactory().CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(message);

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(consumer, "red", false);

            await Task.CompletedTask;
        }
    }
}
