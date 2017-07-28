using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub
{
    /// <summary>
    /// Binding attribute to place on user code for WebJobs. 
    /// </summary>
    [Binding]
    public class IoTDirectMethodAttribute : Attribute
    {
        [AutoResolve]
        public string DeviceId { get; set; }

        [AutoResolve]
        public string InvokeId { get; set; }

        [AutoResolve]
        public string MethodName { get; set; }

        [AppSetting]
        public string Connection { get; set; }
    }
}
