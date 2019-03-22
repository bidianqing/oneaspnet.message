using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            KafkaOptions kafkaOptions = new KafkaOptions();
            Configuration.GetSection("KafkaOptions").Bind(kafkaOptions);

            services.AddKafka(options =>
            {
                options.ProducerConfig = kafkaOptions.ProducerConfig;
                options.ConsumerConfig = kafkaOptions.ConsumerConfig;
                options.AdminClientConfig = kafkaOptions.AdminClientConfig;
            });

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, LogConsumerBackgroundService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, OrderConsumerBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
