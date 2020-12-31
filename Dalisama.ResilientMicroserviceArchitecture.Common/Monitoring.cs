using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class Monitoring<T> where T : MonitoringDataAbstract
    {
        public ElasticClient ElasticClient { get; }
        protected string _defaultIndex { get; }
        public Monitoring(ElasticClientProvider elasticClientProvider)
        {
            ElasticClient = elasticClientProvider.ElasticClient;
            _defaultIndex = elasticClientProvider.DefaultIndex;
        }
        public void UpdateEvent(Guid id, object data)
        {

          ElasticClient.Update<T, object>(id, x => x.Doc(data).Index(_defaultIndex));

        }
        public void InsertEvent(T data)
        {
            ElasticClient.Index<T>(data, x => x.Index(_defaultIndex));
        }
        public T GetEvent(Guid id)
        {
         return   ElasticClient.SearchAsync<T>(descriptor => descriptor.Take(1).Index(_defaultIndex).Query(x => x.Ids(y => y.Values(id)))).Result.Documents.FirstOrDefault();
        }

    }
    public class TransactionMonitoring : Monitoring<TransactionMonitoringData>
    {
        public TransactionMonitoring(ElasticClientProvider elasticClientProvider) : base(elasticClientProvider)
        {
        }
    }
    public abstract class MonitoringDataAbstract
    {
        public Guid Id { get; set; }


    }
    public class TransactionMonitoringData : MonitoringDataAbstract
    {
        

        public TransactionMonitoringData(Guid transactionID, Guid sequenseID)
        {
            Id = transactionID;
            TransactionID = transactionID;
            SequenseID = sequenseID;
            LastModificationDateTime = DateTime.UtcNow;
            InsertionDateTime = DateTime.UtcNow;
            Status = "Received in the producer";
            CurrentPosition = Environment.GetEnvironmentVariable("application name");
        }

        public Guid TransactionID { get; set; }
        public Guid SequenseID { get; set; }
        public string Status { get; set; }
        public string CurrentPosition { get; set; }//which application has traited the message last
        public DateTime LastModificationDateTime { get; set; }
        public DateTime InsertionDateTime { get; set; }
        public DateTime EndtraitementDateTime { get; set; }
        public TimeSpan TotalTraitingTime { get; set; }
        public string TotalTraitingTimeFormatted { get; set; }

     
        
    }
}
