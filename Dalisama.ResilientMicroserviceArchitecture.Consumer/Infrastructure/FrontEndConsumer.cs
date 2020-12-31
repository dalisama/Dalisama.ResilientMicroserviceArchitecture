using Dalisama.ResilientMicroserviceArchitecture.Common;
using Dalisama.ResilientMicroserviceArchitecture.Consumer.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dalisama.ResilientMicroserviceArchitecture.Consumer.Infrastructure
{
    public class FrontEndConsumer : ConsumeRabbitMQHostedServiceBase<ServiceStatusMiddleware, FrontEndDomain>
    {
        public FrontEndConsumer(ILogger<ConsumeRabbitMQHostedServiceBase<ServiceStatusMiddleware, FrontEndDomain>> logger, ServiceStatusMiddleware serviceStatus, FrontEndDomain domain, IOptions<RabbitMqExchangeConsumer> rabbitMqExchangeConfiguration, IOptions<RabbitMqQueueConsumer> rabbitMqQueueConfiguration)
            : base(logger, serviceStatus, domain, rabbitMqExchangeConfiguration, rabbitMqQueueConfiguration)
        {
        }

        public override void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogWarning($"Consumer Cancelled");
        }

        public override void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"Consumer Information");
        }

        public override void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning($"Consumer shutdown");
        }

        public override void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogWarning($"Consumer unregistered");
        }

        public override void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning($"connection shutdown");
        }
    }
}
