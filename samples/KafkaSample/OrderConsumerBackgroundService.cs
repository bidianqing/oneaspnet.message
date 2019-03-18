using KafkaSample.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneAspNet.Message.Kafka;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaSample
{
    public class OrderConsumerBackgroundService : BackgroundService
    {
        private readonly KafkaService<Order> _kafkaService;
        private readonly ILogger _logger;
        public OrderConsumerBackgroundService(KafkaService<Order> kafkaService, ILogger<LogConsumerBackgroundService> logger)
        {
            _kafkaService = kafkaService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                await _kafkaService.ProcessAsync(async (message, token) =>
                {
                    _logger.LogWarning(JsonConvert.SerializeObject(message));

                }, stoppingToken, "OrderConsumer");
            });
        }
    }
}
