using Dalisama.ResilientMicroserviceArchitecture.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Consumer.Domain
{
    public class FrontEndDomain : Common.Domain
    {


        private ILogger _logger;
        private TransactionMonitoring _monitoring;
        public FrontEndDomain(ILogger<FrontEndDomain> logger, TransactionMonitoring monitoring)
        {

            _monitoring = monitoring;
            _logger = logger;
        }

        public override Task<bool> ExecuteAsync(CancellationToken stoppingToken, Message message)
        {
            var transactionID = Guid.Parse(message.MetaData["Transaction id"]);
            _monitoring.UpdateEvent(transactionID, new
            {

                LastModificationDateTime = DateTime.UtcNow,
                Status = "Received in FrontEnd",
                CurrentPosition = Environment.GetEnvironmentVariable("application name"),
            });
            stoppingToken.ThrowIfCancellationRequested();
            message.Body = $"{message.Body}{Environment.NewLine}frontend: {DateTime.UtcNow}";
            Console.Clear();
            message.SetEndTime();
            Console.WriteLine("**********************");
            Console.WriteLine(message.Body);
            Console.WriteLine("---");

            foreach (var item in message.GetMetaData())
            {
                Console.WriteLine($"{item.Key} ==> {item.Value}");
            }

            Console.WriteLine("**********************");
            var monitorEvent = _monitoring.GetEvent(transactionID);
            var datetime = DateTime.UtcNow;
            var TotalTraitingTime = (datetime - monitorEvent.InsertionDateTime);
            var TotalTraitingTimeFormatted = $"{TotalTraitingTime.Hours}h, {TotalTraitingTime.Minutes}m, {TotalTraitingTime.Seconds}s, {TotalTraitingTime.Milliseconds}ms";
            _monitoring.UpdateEvent(transactionID, new
            {
                EndtraitementDateTime = datetime,
                TotalTraitingTime = TotalTraitingTime,
                TotalTraitingTimeFormatted = TotalTraitingTimeFormatted,
                LastModificationDateTime = datetime,
                Status = "Success"
            });

            _logger.LogInformation("End of messge traitement", message.GetMetaData());
            return Task.FromResult(true);



        }

    }
}
