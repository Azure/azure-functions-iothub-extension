using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices;
using System.Threading;
using System;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTDirectMethodAsyncCollector : IAsyncCollector<IoTDirectMethodItem>
    {
        private static ServiceClient serviceClient;

        public IoTDirectMethodAsyncCollector(ServiceClient serviceClient, IoTDirectMethodAttribute attribute)
        {
            // create client;
            IoTDirectMethodAsyncCollector.serviceClient = serviceClient;
        }

        public Task AddAsync(IoTDirectMethodItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            InvokeMethod(item.DeviceId, item.MethodName).Wait();
            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        private static async Task InvokeMethod(string deviceID, string methodName)
        {
            var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };

#error include cancellation token 
            var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation);

#error no Console.WriteLine 
            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
        }
    }
}
