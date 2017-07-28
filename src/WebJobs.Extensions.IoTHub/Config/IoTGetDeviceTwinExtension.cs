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
        private static string connectionString;
        static RegistryManager registryManager;
        private Twin deviceTwin;

        public void Initialize(ExtensionConfigContext context)
        {
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

        //private Twin ConvertToTwin(Twin item)
        //{
        //    return item;
        //}

        private JObject ConvertToJObject(Twin results)
        {
            return JObject.FromObject(results);
        }

        private IoTGetDeviceTwinItem ConvertToItem(string str)
        {
            var item = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);

            return new IoTGetDeviceTwinItem
            {
                DeviceId = (string)item["DeviceId"]
            };
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
            if (registryManager == null)
            {
                connectionString = attribute.Connection;
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }

            deviceTwin = await registryManager.GetTwinAsync(attribute.DeviceId);
        }
    }
}