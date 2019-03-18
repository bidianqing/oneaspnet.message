using Confluent.Kafka;

namespace OneAspNet.Message.Kafka
{
    public class KafkaOptions
    {
        public ProducerConfig ProducerConfig { get; set; }

        public ConsumerConfig ConsumerConfig { get; set; }
    }
}
