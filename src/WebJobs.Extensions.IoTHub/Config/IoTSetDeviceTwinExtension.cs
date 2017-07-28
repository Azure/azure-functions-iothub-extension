using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTSetDeviceTwinExtension : IExtensionConfigProvider
    {
        private static string connectionString;
        static RegistryManager registryManager;

        public void Initialize(ExtensionConfigContext context)
        {
            // This allows a user to bind to IAsyncCollector<string>, and the sdk
            // will convert that to IAsyncCollector<IoTCloudToDeviceItem>
            context.AddConverter<string, IoTSetDeviceTwinItem>(ConvertToItem);

            // This is useful on input. 
            context.AddConverter<IoTSetDeviceTwinItem, string>(ConvertToString);

            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<IoTSetDeviceTwinAttribute>();

            //rule.BindToInput<IoTSetDeviceTwinItem>(BuildItemFromAttr);
            rule.BindToCollector<IoTSetDeviceTwinItem>(BuildCollector);
        }

        private string ConvertToString(IoTSetDeviceTwinItem item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private IoTSetDeviceTwinItem ConvertToItem(string str)
        {
            var item = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);

            return new IoTSetDeviceTwinItem
            {
                DeviceId = (string)item["DeviceId"],
                UpdateId = (string)item["UpdateId"],
                Patch = JsonConvert.SerializeObject(item["Patch"])
            };
        }

        private IAsyncCollector<IoTSetDeviceTwinItem> BuildCollector(IoTSetDeviceTwinAttribute attribute)
        {
            if (registryManager == null)
            {
                connectionString = attribute.Connection;
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }

            return new IoTSetDeviceTwinAsyncCollector(registryManager, attribute);
        }

    }
}