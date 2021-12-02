using RabbitMQ.Client;

namespace OneAspNet.Message.RabbitMQ.Internal
{
    internal class RabbitmqEntry
    {
        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }
    }
}
