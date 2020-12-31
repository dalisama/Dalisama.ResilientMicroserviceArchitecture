using Dalisama.ResilientMicroserviceArchitecture.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;



namespace Dalisama.ResilientMicroserviceArchitecture.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayController : ControllerBase
    {

        [HttpPost]
        public void Post([FromBody] Message value, [FromServices] ILogger<GatewayController> logger, [FromServices] ProducerRabbitMQBase producerRabbitMQBase, [FromServices] TransactionMonitoring transactionMonitoring)
        {
            if (!value.MetaData.ContainsKey("Sequence id")) value.MetaData["Sequence id"] = Guid.NewGuid().ToString();
            if (!value.MetaData.ContainsKey("Transaction id")) value.MetaData["Transaction id"] = Guid.NewGuid().ToString();

            value.SetStartTime();
            transactionMonitoring.InsertEvent(new TransactionMonitoringData(Guid.Parse(value.MetaData["Transaction id"]), Guid.Parse(value.MetaData["Sequence id"])));
          
            using (logger.BeginScope<Dictionary<string, object>>(value.GetMetaData()))
            {
                logger.LogInformation(value.Body);
                value.SetEndTime();
                producerRabbitMQBase.PushMessageAsync(value, new System.Threading.CancellationToken());
            }

             transactionMonitoring.UpdateEvent(Guid.Parse(value.MetaData["Transaction id"]), new  {

                LastModificationDateTime = DateTime.UtcNow,
                Status="Out of producer"

            });
        }


    }
}
