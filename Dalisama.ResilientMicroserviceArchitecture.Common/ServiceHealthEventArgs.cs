using System;
using System.Collections.Generic;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class ServiceHealthEventArgs : EventArgs
    {
        public List<string> ServiceDowns { get; set; } // list de service down
        public DateTime TimeReached { get; set; }
        public string Message { get; set; }
    }

}
