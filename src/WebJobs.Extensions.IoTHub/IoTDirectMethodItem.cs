using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub
{
    public class IoTDirectMethodItem
    {
        // Destination IoT DeviceId
        public string DeviceId { set; get; }

        // MethodName to be invoked
        public string MethodName { set; get; }
        
        // Payload as arguments to the method
        public JObject Payload { set; get; }
    }
}
