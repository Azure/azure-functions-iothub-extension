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
        private Dictionary<string, ServiceClient> _clients;
        private string connectionString;
        private ServiceClient serviceClient;

        public void Initialize(ExtensionConfigContext context)
        {
            _clients = new Dictionary<string, ServiceClient>();

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
            return JsonConvert.DeserializeObject<IoTCloudToDeviceItem>(str);
        }

        private IAsyncCollector<IoTCloudToDeviceItem> BuildCollector(IoTCloudToDeviceAttribute attribute)
        {
            connectionString = attribute.Connection;
            if (_clients.ContainsKey(connectionString))
            {
                serviceClient = _clients[connectionString];
            }
            else
            {
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                _clients.Add(connectionString, serviceClient);
            }

            return new IoTCloudToDeviceAsyncCollector(serviceClient, attribute);
        }
    }
}