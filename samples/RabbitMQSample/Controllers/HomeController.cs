using Microsoft.AspNetCore.Mvc;
using OneAspNet.Message.Rabbitmq;
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
            for (int i = 0; i < 10; i++)
            {
                _producingService.Send("test", "", "queue_red", $"{i.ToString()}", (basicProperties) =>
                {
                    basicProperties.Headers = new Dictionary<string, object>();
                    basicProperties.Headers.Add("name", "bidianqing");
                });
            }

            return Content("发送成功");
        }
    }
}
