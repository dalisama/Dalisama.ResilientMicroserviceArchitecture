using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Threading;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class ProducerRabbitMQBase
    {

        private IModel _channel;
        protected readonly ILogger _logger;
        private readonly RabbitMqExchangeConfiguration _rabbitMqExchangeConfiguration;
        private readonly RabbitMqQueueConfiguration _rabbitMqQueueConfiguration;
        private IConnection _connection;

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqExchangeConfiguration.Hostname,
                Password = _rabbitMqExchangeConfiguration.Password,
                VirtualHost = _rabbitMqExchangeConfiguration.VirtualHost,
                UserName = _rabbitMqExchangeConfiguration.UserName,
                Port = _rabbitMqExchangeConfiguration.Port

            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_rabbitMqExchangeConfiguration.ExchangeName, _rabbitMqExchangeConfiguration.ExchangeType);
            _channel.QueueDeclare(_rabbitMqQueueConfiguration.QueueName, _rabbitMqQueueConfiguration.Durable, _rabbitMqQueueConfiguration.Exclusive, _rabbitMqQueueConfiguration.Autodelete, null);
            _channel.QueueBind(_rabbitMqQueueConfiguration.QueueName, _rabbitMqExchangeConfiguration.ExchangeName, _rabbitMqQueueConfiguration.Key, null);
            _channel.BasicQos(0, 1, true);


        }

        public ProducerRabbitMQBase(ILogger<ProducerRabbitMQBase> logger, IOptions<RabbitMqExchangeConfiguration> rabbitMqExchangeConfiguration, IOptions<RabbitMqQueueConfiguration> rabbitMqQueueConfiguration)
        {
            _rabbitMqExchangeConfiguration = rabbitMqExchangeConfiguration.Value;
            _rabbitMqQueueConfiguration = rabbitMqQueueConfiguration.Value;
            InitRabbitMQ();
            _logger = logger;
        }

        public virtual Task<bool> PushMessageAsync(Message message, CancellationToken cancellationToken)
        {


            try
            {

                _logger.LogInformation($"PushMessage,routingKey:{_rabbitMqQueueConfiguration.Key}");

                _channel.BasicPublish(exchange: _rabbitMqExchangeConfiguration.ExchangeName,
                                        routingKey: _rabbitMqQueueConfiguration.Key,
                                       basicProperties: null,
                                        body: (byte[])message);
                return Task.FromResult<bool>(true);
            }
            catch (Exception)
            {

                return Task.FromResult<bool>(false);
            }
        }
    }
}