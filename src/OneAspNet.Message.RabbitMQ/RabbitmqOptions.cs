using RabbitMQ.Client;
using System.Threading.Tasks;
using System;
using RabbitMQ.Client.Events;
using System.Reflection;

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

        /// <summary>
        /// 创建Channel以后的委托函数
        /// </summary>
        public Action<IModel> CreateModelAfter { get; set; }
    }
}
