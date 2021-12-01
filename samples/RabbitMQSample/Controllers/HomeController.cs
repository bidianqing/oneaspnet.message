using Microsoft.AspNetCore.Mvc;
using OneAspNet.Message.RabbitMQ;

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
            for (int i = 0; i < 10000000; i++)
            {
                _producingService.Send("localhost", $"{i.ToString()}-不用摇号也可以拥有北京车牌！242辆京牌小客车参加司法处置，今起报名报名时间：12月1日10:00至12月10日15:00报名时间：12月1日10:00至12月10日15:00报名时间：12月1日10:00至12月10日15:00报名时间：12月1日10:00至12月10日15:00");
            }

            return Content("发送成功");
        }
    }
}
