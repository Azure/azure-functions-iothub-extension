using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub
{
    /// <summary>
    /// Binding attribute to place on user code for WebJobs. 
    /// </summary>
    [Binding]
    public class IoTCloudToDeviceAttribute : Attribute
    {
        [AutoResolve]
        public string DeviceId { get; set; }

        [AutoResolve]
        public string MessageId { get; set; }

        [AutoResolve]
        public string Message { get; set; }

        [AppSetting]
        public string Connection { get; set; }
    }

}
