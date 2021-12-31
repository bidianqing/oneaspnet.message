using OneAspNet.Message.Rabbitmq;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitmq(this IServiceCollection services, Action<RabbitmqOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<IProducingService, ProducingService>());

            return services;
        }
    }
}
