using OneAspNet.Message.Kafka;
using System;
using System.Runtime.Serialization;

namespace KafkaSample.Models
{
    [TopicName("uatapplog")]
    [DataContract]
    public class Log
    {
        [DataMember(Order = 0)]
        public Guid Id { get; set; }

        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
