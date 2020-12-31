using Dalisama.ResilientMicroserviceArchitecture.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Middleware.Infrastructure
{
    public class GracefulShutdownMiddleware : GracefulShutdown
    {
        public GracefulShutdownMiddleware(IHostApplicationLifetime appLifetime, ServiceStatusMiddleware serviceStatus, ILogger<GracefulShutdown> logger)
            : base(appLifetime, serviceStatus, logger)
        {
        }
    }
}
