using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneAspNet.Message.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly KafkaService<Log> _logKafkaService;
        private readonly KafkaService<Order> _orderKafkaService;
        private readonly ILogger _logger;
        private readonly KafkaOptions _kafkaOptions;
        public HomeController(KafkaService<Log> logKafkaService, KafkaService<Order> orderKafkaService, ILogger<HomeController> logger, IOptionsMonitor<KafkaOptions> kafkaOptionsAccesstor)
        {
            _logKafkaService = logKafkaService;
            _orderKafkaService = orderKafkaService;
            _logger = logger;
            _kafkaOptions = kafkaOptionsAccesstor.CurrentValue;
        }

        [HttpGet("publishlog")]
        public async Task<IActionResult> PublishLog()
        {
            await _logKafkaService.ProduceAsync(new Log
            {
                Id = Guid.NewGuid(),
                Message = "Log Message"
            });
            return Content("ok");
        }

        [HttpGet("publishorder")]
        public async Task<IActionResult> PublishOrder()
        {
            await _orderKafkaService.ProduceAsync(new Order
            {
                Number = Guid.NewGuid(),
                Title = "Order Message"
            });
            return Ok();
        }

        public async Task<IActionResult> CreatePartitions()
        {
            using (IAdminClient adminClient = new Confluent.Kafka.AdminClientBuilder(_kafkaOptions.AdminClientConfig).Build())
            {
                await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                    new TopicSpecification{ Name = "Log", NumPartitions = 24 },
                    new TopicSpecification{ Name = "Order", NumPartitions = 24 },
                });
            }

            return Ok();
        }
    }
}
