using Dalisama.ResilientMicroserviceArchitecture.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Middleware.Infrastructure
{
    public class MiddlewareProducer : ProducerRabbitMQBase
    {
        public MiddlewareProducer(ILogger<MiddlewareProducer> logger, IOptions<RabbitMqExchangeProducer> rabbitMqExchangeConfiguration, IOptions<RabbitMqQueueProducer> rabbitMqQueueConfiguration) : base(logger, rabbitMqExchangeConfiguration, rabbitMqQueueConfiguration)
        {
        }
    }
}
