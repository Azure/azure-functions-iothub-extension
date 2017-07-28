using System;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTCloudToDeviceExtension : IExtensionConfigProvider
    {
        private static string connectionString;
        private static ServiceClient serviceClient;

        public void Initialize(ExtensionConfigContext context)
        {

            // This allows a user to bind to IAsyncCollector<string>, and the sdk
            // will convert that to IAsyncCollector<IoTCloudToDeviceItem>
            context.AddConverter<string, IoTCloudToDeviceItem>(ConvertToItem);

            // This is useful on input. 
            context.AddConverter<IoTCloudToDeviceItem, string>(ConvertToString);

            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<IoTCloudToDeviceAttribute>();

            rule.BindToCollector<IoTCloudToDeviceItem>(BuildCollector);
        }

        private string ConvertToString(IoTCloudToDeviceItem item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private IoTCloudToDeviceItem ConvertToItem(string str)
        {
            var item = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);

            return new IoTCloudToDeviceItem
            {
                DeviceId = item["DeviceId"],
                MessageId = item["MessageId"],
                Message = str
            };
        }

        private IAsyncCollector<IoTCloudToDeviceItem> BuildCollector(IoTCloudToDeviceAttribute attribute)
        {
            if (serviceClient == null)
            {
                connectionString = attribute.Connection;
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            }

            return new IoTCloudToDeviceAsyncCollector(serviceClient, attribute);
        }
    }
}