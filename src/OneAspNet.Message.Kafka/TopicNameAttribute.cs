using System;

namespace OneAspNet.Message.Kafka
{
    public class TopicNameAttribute : Attribute
    {
        public TopicNameAttribute(string topic)
        {
            this.Name = topic;
        }
        public string Name { get; set; }
    }
}
