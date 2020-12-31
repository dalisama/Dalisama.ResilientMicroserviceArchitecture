namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class RabbitMqQueueConfiguration
    {
        public string QueueName { get; set; }
        public bool   Durable { get; set; }
        public bool   Exclusive { get; set; }
        public bool   Autodelete { get; set; }
        public string Key { get; set; }

    }

}
