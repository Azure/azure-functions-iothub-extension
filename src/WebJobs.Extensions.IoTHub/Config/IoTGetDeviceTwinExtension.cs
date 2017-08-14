using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTGetDeviceTwinExtension : IExtensionConfigProvider
    {
        private Dictionary<string, RegistryManager> _manager; // key: connection string
        private Twin deviceTwin;

        public void Initialize(ExtensionConfigContext context)
        {
            _manager = new Dictionary<string, RegistryManager>();

            // This is useful on input. 
            context.AddConverter<Twin, string>(ConvertToString);
            context.AddConverter<Twin, Newtonsoft.Json.Linq.JObject>(ConvertToJObject);
            //context.AddConverter<Twin, Twin>(ConvertToTwin);

            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<IoTGetDeviceTwinAttribute>();

            rule.BindToInput<Twin>(BuildItemFromAttr);
        }

        private string ConvertToString(Twin item)
        {
            return JsonConvert.SerializeObject(item);
        }        

        private JObject ConvertToJObject(Twin results)
        {
            return JObject.FromObject(results);
        }

        private IoTGetDeviceTwinItem ConvertToItem(string str)
        {
            return JsonConvert.DeserializeObject<IoTGetDeviceTwinItem>(str);
        }


        // All {} and %% in the Attribute have been resolved by now.
        private Twin BuildItemFromAttr(IoTGetDeviceTwinAttribute attribute)
        {
            string deviceId = attribute.DeviceId;
            GetDeviceTwinAsync(attribute).Wait();
            return deviceTwin;
        }

        private async Task GetDeviceTwinAsync(IoTGetDeviceTwinAttribute attribute)
        {
            var connectionString = attribute.Connection;
            RegistryManager registryManager = null;
            if (!_manager.TryGetValue(connectionString, out registryManager)) {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                _manager.Add(connectionString, registryManager);
            }

            deviceTwin = await registryManager.GetTwinAsync(attribute.DeviceId);
        }
    }
}