using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
  public  class GracefulShutdown : IHostedService
    {
        private IHostApplicationLifetime _appLifetime;
        private ServiceStatus _serviceStatus;
        private ILogger _logger;

        public GracefulShutdown(IHostApplicationLifetime appLifetime, ServiceStatus serviceStatus, ILogger<GracefulShutdown> logger)
        {
            _appLifetime = appLifetime;
            _serviceStatus = serviceStatus;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("hello, let's go!");
            });

            _appLifetime.ApplicationStopped.Register(() =>
            {
                _serviceStatus.RaiseServiceHealthDegadedEvent(new ServiceHealthEventArgs { });
                _logger.LogWarning("GracefulShutdown has been engaged");
                var count = 0;
                while (count<30)
                {
                    Task.Delay(1000).Wait();
                    _logger.LogWarning($"{count} -- GracefulShutdown has been engaged ");
                    count++;
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
