using RabbitMQSample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<DuckConsumerBackgroundService>();

builder.Services.AddRabbitmq(options =>
{
    options.RabbitmqConnections = builder.Configuration.GetSection("RabbitmqConnections").Get<RabbitmqConnection[]>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();