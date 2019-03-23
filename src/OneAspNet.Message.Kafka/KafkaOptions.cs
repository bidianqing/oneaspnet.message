using Confluent.Kafka;

namespace OneAspNet.Message.Kafka
{
    public class KafkaOptions
    {
        public ProducerConfig ProducerConfig { get; set; }

        public ConsumerConfig ConsumerConfig { get; set; }

        public AdminClientConfig AdminClientConfig { get; set; }

        public CustomConfig CustomConfig { get; set; }
    }

    public class CustomConfig
    {
        /// <summary>
        /// 是否启用每消费多少条数据自动提交一次，默认true
        /// </summary>
        public bool EnableNumOfAutoCommit { get; set; } = true;

        /// <summary>
        /// 每消费多少条数据自动提交一次，默认100
        /// </summary>
        public int NumOfAutoCommit { get; set; } = 100;
    }
}
