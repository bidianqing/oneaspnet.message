using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneAspNet.Message.Kafka;

namespace KafkaSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            KafkaOptions kafkaOptions = new KafkaOptions();
            Configuration.GetSection("KafkaOptions").Bind(kafkaOptions);

            services.AddKafka(options =>
            {
                options.ProducerConfig = kafkaOptions.ProducerConfig;
                options.ConsumerConfig = kafkaOptions.ConsumerConfig;
                options.AdminClientConfig = kafkaOptions.AdminClientConfig;
                options.CustomConfig = kafkaOptions.CustomConfig;
            });
            
            services.AddHostedService<LogConsumerBackgroundService>();
            //services.AddHostedService<OrderConsumerBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
