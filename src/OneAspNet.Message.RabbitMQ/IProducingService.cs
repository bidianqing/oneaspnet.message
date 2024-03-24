using RabbitMQ.Client;
using System;

namespace OneAspNet.Message.Rabbitmq
{
    public interface IProducingService
    {
        void Send(string connectionName, string exchange, string routingKey, string message, bool mandatory = false, Action<IBasicProperties> action = null);
    }
}
