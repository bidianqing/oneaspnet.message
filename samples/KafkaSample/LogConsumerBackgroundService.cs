using Elasticsearch.Net;
using KafkaSample.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneAspNet.Message.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaSample
{
    public class LogConsumerBackgroundService : BackgroundService
    {
        private readonly KafkaService<Log> _kafkaService;
        private readonly ILogger _logger;
        public LogConsumerBackgroundService(KafkaService<Log> kafkaService, ILogger<LogConsumerBackgroundService> logger)
        {
            _kafkaService = kafkaService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                var lowlevelClient = new ElasticLowLevelClient();

                await _kafkaService.ProcessAsync(async (message, token) =>
                {
                    string index = $"log-{DateTime.Now.ToString("yyyy-MM-dd")}";
                    await lowlevelClient.IndexAsync<StringResponse>(index, PostData.String(message));

                }, stoppingToken, "LogConsumer");
            });
        }
    }
}
