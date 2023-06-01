using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public class RabbitmqOptions
    {
        public RabbitmqConnection[] RabbitmqConnections { get; set; }
    }

    public class RabbitmqConnection
    {
        /// <summary>
        /// ConnectionName
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Connection Options
        /// </summary>
        public ConnectionFactory ConnectionFactory { get; set; }
    }
}
