using Microsoft.AspNetCore.Mvc;
using OneAspNet.Message.Rabbitmq;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IProducingService _producingService;
        public HomeController(IProducingService producingService)
        {
            _producingService = producingService;
        }

        public IActionResult Index()
        {
            //var connection = new ConnectionFactory().CreateConnection();
            //var model = connection.CreateModel();
            //model.QueueDeclare();
            for (int i = 0; i < 10; i++)
            {
                _producingService.Send("test", "duck-direct", "red", $"{i.ToString()}", (basicProperties) =>
                {
                    basicProperties.Headers = new Dictionary<string, object>();
                    basicProperties.Headers.Add("name", "bidianqing");
                });
            }

            return Content("发送成功");
        }
    }
}
