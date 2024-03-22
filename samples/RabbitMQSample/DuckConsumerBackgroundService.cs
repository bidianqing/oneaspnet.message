using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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
            var connection = new ConnectionFactory() { DispatchConsumersAsync = true }.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new AsyncEventingBasicConsumer(channel);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await Console.Out.WriteLineAsync($"接收消息：{message}");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                if (message == "cancel")
                {
                    channel.BasicCancel(consumer.ConsumerTags[0]);
                }
            };

            channel.BasicConsume(consumer, "red", false);


            await Task.CompletedTask;
        }
    }
}
