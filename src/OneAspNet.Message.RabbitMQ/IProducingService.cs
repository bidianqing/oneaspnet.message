namespace OneAspNet.Message.RabbitMQ
{
    public interface IProducingService
    {
        void Send(string clientProvidedName, string message);
    }
}
