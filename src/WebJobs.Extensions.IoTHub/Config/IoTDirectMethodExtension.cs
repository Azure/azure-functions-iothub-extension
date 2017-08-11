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
        private Dictionary<string, ServiceClient> _clients; // key: connection string
        private string connectionString;
        private ServiceClient serviceClient;

        public void Initialize(ExtensionConfigContext context)
        {
            _clients = new Dictionary<string, ServiceClient>();

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
            //return JsonConvert.DeserializeObject<IoTDirectMethodItem>(str);
            var item = JsonConvert.DeserializeObject<Dictionary<string, Object>>(str);

            return (item.ContainsKey("Payload")) ?
                new IoTDirectMethodItem
                {
                    DeviceId = item["DeviceId"].ToString(),
                    MethodName = item["MethodName"].ToString(),
                    Payload = item["Payload"].ToString()
                } :
                new IoTDirectMethodItem
                {
                    DeviceId = item["DeviceId"].ToString(),
                    MethodName = item["MethodName"].ToString()
                };
        }

        private IAsyncCollector<IoTDirectMethodItem> BuildCollector(IoTDirectMethodAttribute attribute)
        {
            connectionString = attribute.Connection;
            if (_clients.TryGetValue(connectionString, out serviceClient)) { }
            else
            {
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                _clients.Add(connectionString, serviceClient);
            }

            return new IoTDirectMethodAsyncCollector(serviceClient, attribute);
        }

    }
}