using RabbitMQ.Client;

namespace OneAspNet.Message.Rabbitmq.Internal
{
    internal class RabbitmqEntry
    {
        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }
    }
}
