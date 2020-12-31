using System.Threading;
using System.Threading.Tasks;


namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public abstract class Domain
    {
        public abstract Task<bool> ExecuteAsync(CancellationToken stoppingToken,Message message);
    }

}
