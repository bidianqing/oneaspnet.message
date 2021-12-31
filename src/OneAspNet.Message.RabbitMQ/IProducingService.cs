using RabbitMQ.Client;
using System;

namespace OneAspNet.Message.Rabbitmq
{
    public interface IProducingService
    {
        void Send(string connectionName, string exchange, string routingKey, string message, Action<IBasicProperties> action = null);
    }
}
