namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class RabbitMqExchangeConfiguration
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

    }

}
