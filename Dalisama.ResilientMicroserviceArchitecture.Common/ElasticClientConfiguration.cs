using System.Collections.Generic;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class ElasticClientConfiguration
    {
        public string[] Urls { get; set; }
        public string DefaultIndex { get; set; }
    }
}
