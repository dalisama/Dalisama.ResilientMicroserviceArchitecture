using Dalisama.ResilientMicroserviceArchitecture.Common;
using Dalisama.ResilientMicroserviceArchitecture.Middleware.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Middleware.Domain
{
    public class MiddlewareDomain : Dalisama.ResilientMicroserviceArchitecture.Common.Domain
    {
        private ProducerRabbitMQBase _producerRabbitMQBase;
        private MiddlewareDomainData _middlewareDomainData;
        private ILogger _logger;
        private TransactionMonitoring _monitoring;
        public MiddlewareDomain(MiddlewareProducer producerRabbitMQBase, IOptions<MiddlewareDomainData> data, ILogger<MiddlewareDomain> logger, TransactionMonitoring monitoring)
        {
            _producerRabbitMQBase = producerRabbitMQBase;
            _middlewareDomainData = data.Value;
            _logger = logger;
            _monitoring = monitoring;
        }

        public override Task<bool> ExecuteAsync(CancellationToken stoppingToken, Message message)
        {
            var transactionID = Guid.Parse(message.MetaData["Transaction id"]);
            _monitoring.UpdateEvent(transactionID, new
            {

                LastModificationDateTime = DateTime.UtcNow,
                Status = "Received in Middleware",
                CurrentPosition = Environment.GetEnvironmentVariable("application name"),
            });
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                message.Body = $"{message.Body}{Environment.NewLine}Middleware: {DateTime.UtcNow}";
                var count = 0;
                while (count < _middlewareDomainData.Duration)
                {
                    Thread.Sleep(1000);
                    count++;
                }

                _logger.LogInformation(message.Body);
                count = 0;
                while (count < _middlewareDomainData.Duration)
                {
                    Thread.Sleep(1000);
                    count++;
                    stoppingToken.ThrowIfCancellationRequested();
                }
                message.SetEndTime();

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("message has been canceled");
                Task.FromResult<bool>(false);

            }
            catch (Exception)
            {
                _logger.LogInformation("message has been canceled");
                Task.FromResult<bool>(false);
            }
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("message has been canceled");
                return Task.FromResult<bool>(false);
            }
            _logger.LogInformation("Reaching the no return point!");


            _producerRabbitMQBase.PushMessageAsync(message, stoppingToken);
            _monitoring.UpdateEvent(transactionID, new
            {

                LastModificationDateTime = DateTime.UtcNow,
                Status = "out of the Middleware",
                CurrentPosition = Environment.GetEnvironmentVariable("application name"),
            });
            return Task.FromResult<bool>(true);
        }

    }
}
