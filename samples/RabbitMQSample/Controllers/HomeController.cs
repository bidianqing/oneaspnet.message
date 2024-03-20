using Microsoft.AspNetCore.Mvc;
using OneAspNet.Message.Rabbitmq;

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

        [HttpGet]
        public IActionResult Get()
        {
            for (int i = 0; i < 10; i++)
            {
                _producingService.Send("test", "duck-direct", "red", $"我是第{i.ToString()}只小鸭子", (basicProperties) =>
                {
                    basicProperties.Headers = new Dictionary<string, object>();
                    basicProperties.Headers.Add("name", "bidianqing");
                });
            }

            return Content("发送成功");
        }
    }
}
