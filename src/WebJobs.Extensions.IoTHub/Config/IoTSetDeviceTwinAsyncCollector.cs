using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices;
using System.Threading;
using System;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTSetDeviceTwinAsyncCollector : IAsyncCollector<IoTSetDeviceTwinItem>
    {
        private RegistryManager registryManager;

        public IoTSetDeviceTwinAsyncCollector(RegistryManager registryManager, IoTSetDeviceTwinAttribute attribute)
        {
            // create client;
            this.registryManager = registryManager;
        }

        public async Task AddAsync(IoTSetDeviceTwinItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SetDesiredConfigurationAndQuery(item, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        private async Task SetDesiredConfigurationAndQuery(IoTSetDeviceTwinItem item, CancellationToken cancellationToken)
        {
            var twin = await registryManager.GetTwinAsync(item.DeviceId, cancellationToken); // how to include cancellation token?
            await registryManager.UpdateTwinAsync(twin.DeviceId, item.Patch, twin.ETag);
        }
    }
}
