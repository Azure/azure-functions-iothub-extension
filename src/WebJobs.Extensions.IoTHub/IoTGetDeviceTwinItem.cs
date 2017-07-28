using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub
{
    public class IoTGetDeviceTwinItem
    {
        // Destination IoT DeviceId
        public string DeviceId { set; get; }
    }
}
