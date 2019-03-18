using OneAspNet.Message.Kafka;
using System;
using System.Runtime.Serialization;

namespace KafkaSample.Models
{
    [TopicName("Order")]
    [DataContract]
    public class Order
    {
        [DataMember(Order = 0)]
        public Guid Number { get; set; }

        [DataMember(Order = 1)]
        public string Title { get; set; }
    }
}
