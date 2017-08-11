using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices;
using System.Threading;
using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTDirectMethodAsyncCollector : IAsyncCollector<IoTDirectMethodItem>
    {
        private readonly ServiceClient serviceClient;

        public IoTDirectMethodAsyncCollector(ServiceClient serviceClient, IoTDirectMethodAttribute attribute)
        {
            this.serviceClient = serviceClient;
        }

        public async Task AddAsync(IoTDirectMethodItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            await InvokeMethod(item.DeviceId, item.MethodName, item.Payload, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        private async Task InvokeMethod(string deviceID, string methodName, JObject payload, CancellationToken cancellationToken)
        {
            var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson(payload.ToString());
            var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation, cancellationToken);
        }
    }
}
