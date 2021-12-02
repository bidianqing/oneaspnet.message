using RabbitMQ.Client;

namespace OneAspNet.Message.RabbitMQ
{
    public class RabbitmqEntryOptions
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
