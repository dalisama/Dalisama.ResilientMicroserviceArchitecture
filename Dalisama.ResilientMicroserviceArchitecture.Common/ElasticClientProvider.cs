using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class ElasticClientProvider
    {
        private string[] _urls;
        public string DefaultIndex { get; }
        public ElasticClient ElasticClient { get; }

        public ElasticClientProvider(IOptions<ElasticClientConfiguration> elasticClientConf)
        {
            _urls = elasticClientConf.Value.Urls;
            DefaultIndex = elasticClientConf.Value.DefaultIndex;
            if (_urls.Length > 1)
            {
                var nodes = _urls.Select(x => new Uri(x));

                var connectionPool = new SniffingConnectionPool(nodes);
                var settings = new ConnectionSettings(connectionPool)
                    .DefaultIndex(elasticClientConf.Value.DefaultIndex);

                ElasticClient = new ElasticClient(settings);
            }
            else
            {
                var nodes = new Uri(_urls.FirstOrDefault());

                var connectionPool = new SingleNodeConnectionPool(nodes);
                var settings = new ConnectionSettings(connectionPool)
                    .DefaultIndex(elasticClientConf.Value.DefaultIndex);

                ElasticClient = new ElasticClient(settings);
            }
            
        }
    }
}
