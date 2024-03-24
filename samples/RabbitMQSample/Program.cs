using RabbitMQ.Client;
using RabbitMQSample;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<DuckConsumerBackgroundService>();

builder.Services.AddRabbitmq(options =>
{
    options.RabbitmqConnections = builder.Configuration.GetSection("RabbitmqConnections").Get<RabbitmqConnection[]>();
    var conn = Array.Find(options.RabbitmqConnections, u => u.ConnectionName == "test");
    conn.CreateModelAfter = (channel) =>
    {
        channel.BasicReturn += (object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine($"未路由到任何一个队列，消息内容：{message}");
        };
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();