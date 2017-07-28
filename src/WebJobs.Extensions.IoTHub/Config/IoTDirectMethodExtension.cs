using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Extensions.IoTHub.Config
{
    public class IoTDirectMethodExtension : IExtensionConfigProvider
    {
        private static string connectionString;
        private static ServiceClient serviceClient;

        public void Initialize(ExtensionConfigContext context)
        {
            // This allows a user to bind to IAsyncCollector<string>, and the sdk
            // will convert that to IAsyncCollector<IoTCloudToDeviceItem>
            context.AddConverter<string, IoTDirectMethodItem>(ConvertToItem);

            // This is useful on input. 
            context.AddConverter<IoTDirectMethodItem, string>(ConvertToString);

            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<IoTDirectMethodAttribute>();

            //rule.BindToInput<SampleItem>(BuildItemFromAttr);
            rule.BindToCollector<IoTDirectMethodItem>(BuildCollector);
        }

        private string ConvertToString(IoTDirectMethodItem item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private IoTDirectMethodItem ConvertToItem(string str)
        {
            var item = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);

            return new IoTDirectMethodItem
            {
                DeviceId = item["DeviceId"],
                InvokeId = item["InvokeId"],
                MethodName = item["MethodName"]
            };
        }

        private IAsyncCollector<IoTDirectMethodItem> BuildCollector(IoTDirectMethodAttribute attribute)
        {
            if (serviceClient == null)
            {
                connectionString = attribute.Connection;
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            }

            return new IoTDirectMethodAsyncCollector(serviceClient, attribute);
        }

    }
}