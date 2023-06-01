using RabbitMQ.Client;

namespace OneAspNet.Message.Rabbitmq.Internal
{
    internal class RabbitmqBus
    {
        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }
    }
}
