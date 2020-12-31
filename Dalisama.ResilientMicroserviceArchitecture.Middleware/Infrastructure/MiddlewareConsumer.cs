using Dalisama.ResilientMicroserviceArchitecture.Common;
using Dalisama.ResilientMicroserviceArchitecture.Middleware.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dalisama.ResilientMicroserviceArchitecture.Middleware.Infrastructure
{
    public class MiddlewareConsumer : ConsumeRabbitMQHostedServiceBase<ServiceStatusMiddleware, MiddlewareDomain>
    {
        public MiddlewareConsumer(ILogger<ConsumeRabbitMQHostedServiceBase<ServiceStatusMiddleware, MiddlewareDomain>> logger, ServiceStatusMiddleware serviceStatus, MiddlewareDomain domain, IOptions<RabbitMqExchangeConsumer> rabbitMqExchangeConfiguration, IOptions<RabbitMqQueueConsumer> rabbitMqQueueConfiguration)
            : base(logger, serviceStatus, domain, rabbitMqExchangeConfiguration, rabbitMqQueueConfiguration)
        {
        }

        public override void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            base._logger.LogWarning($"Consumer Cancelled");
        }

        public override void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            base._logger.LogInformation($"Consumer Information");
        }

        public override void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            base._logger.LogWarning($"Consumer shutdown");
        }

        public override void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            base._logger.LogWarning($"Consumer unregistered");
        }

        public override void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            base._logger.LogWarning($"connection shutdown");
        }
    }
}
