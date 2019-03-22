using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneAspNet.Message.Kafka;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KafkaSample.Controllers
{
    public class HomeController : Controller
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("publishlog")]
        public async Task<IActionResult> PublishLog()
        {
            await _logKafkaService.ProduceAsync(new Log
            {
                Id = Guid.NewGuid(),
                Message = "Log Message"
            });
            return Ok();
        }

        [HttpPost("publishorder")]
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
                await adminClient.CreatePartitionsAsync(new PartitionsSpecification[]
                {
                    new PartitionsSpecification
                    {
                        Topic="Log",
                        IncreaseTo=24,
                    },
                    new PartitionsSpecification
                    {
                        Topic="Order",
                        IncreaseTo=24,
                    },
                });
            }

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
