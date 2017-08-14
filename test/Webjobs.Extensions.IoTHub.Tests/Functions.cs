using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.WebJobs.Extensions.IoTHub;

namespace SampleFunctions
{
    public class Functions
    {
        // Write some messages
        [NoAutomaticTrigger]
        public void WriteMessageFromC2D([IoTCloudToDevice] ICollector<string> output)
        {
            var item = new
            {
                DeviceId = "receiverBob",
                Message = "Hello"
            };
            output.Add(JsonConvert.SerializeObject(item));

            item = new
            {
                DeviceId = "receiverBob",
                Message = "From"
            };
            output.Add(JsonConvert.SerializeObject(item));

            item = new
            {
                DeviceId = "receiverBob",
                Message = "Cloud"
            };
            output.Add(JsonConvert.SerializeObject(item));
        }

        // Write some messages
        [NoAutomaticTrigger]
        public void WriteMessageFromC2DArg(string deviceId,  // from trigger
            [IoTCloudToDevice(DeviceId = "{deviceId}", Connection = "IoTConnectionString")] ICollector<string> output)
        {
            var item = new
            {
                DeviceId = deviceId,
                Message = "telemetry data point"
            };
            output.Add(JsonConvert.SerializeObject(item));
        }

        // Write some messages
        [NoAutomaticTrigger]
        public void DirectInvokeMethod(string deviceId,  // from trigger
            [IoTDirectMethod(DeviceId = "{deviceId}", Connection = "IoTConnectionString")] ICollector<string> output)
        {
            var item1 = new
            {
                DeviceId = deviceId,
                MethodName = "writeLine",
                Payload = new
                {
                    arg1 = "HiHo1"
                }
            };
            output.Add(JsonConvert.SerializeObject(item1));

            var item2 = new
            {
                DeviceId = deviceId,
                MethodName = "writeLine",
                Payload = new
                {
                    arg1 = "HiHo2"
                }
            };
            output.Add(JsonConvert.SerializeObject(item2));

            var item3 = new
            {
                DeviceId = deviceId,
                MethodName = "writeLine",
                Payload = new { }
            };
            output.Add(JsonConvert.SerializeObject(item3));
        }

        // Write some messages
        [NoAutomaticTrigger]
        public void SetDeviceTwin(string deviceId,  // from trigger
            [IoTSetDeviceTwin(DeviceId = "{deviceId}", Connection = "IoTConnectionString")] ICollector<string> output)
        {

            var item2 = new
            {
                DeviceId = deviceId,
                Patch = new
                {
                    properties = new
                    {
                        desired = new
                        {
                            telemetryConfig = new
                            {
                                configId = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };
            output.Add(JsonConvert.SerializeObject(item2));

            var item = new
            {
                DeviceId = deviceId,
                Patch = new
                {
                    tags = new
                    {
                        location = new
                        {
                            region = "US",
                            plant = "Redmond43"
                        }
                    }
                }
            };
            output.Add(JsonConvert.SerializeObject(item));

            var item3 = new
            {
                DeviceId = deviceId,
                Patch = new
                {
                    properties = new
                    {
                        desired = new
                        {
                            telemetryConfig = new
                            {
                                configId = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };
            output.Add(JsonConvert.SerializeObject(item3));
        }

        // Write some messages
        [NoAutomaticTrigger]
        public void GetDeviceTwin(string deviceId,  // from trigger
            [IoTGetDeviceTwin(DeviceId = "{deviceId}", Connection = "IoTConnectionString")]JObject result,
            TraceWriter log)
        {
            log.Info(JsonConvert.SerializeObject(result));
        }

        [NoAutomaticTrigger]
        public void GetDeviceTwinTwinObject(string deviceId,  // from trigger
            [IoTGetDeviceTwin(DeviceId = "{deviceId}", Connection = "IoTConnectionString")]Twin result,
            TraceWriter log)
        {
            log.Info(result.ToJson());
        }

#if false
        #region Using 2nd extensions

        // Bind to input as rich type:
        // BindToInput<SampleItem> --> item
        [NoAutomaticTrigger]
        public void Reader3(
            string name,  // from trigger
            [Sample(Name = "{name}")] CustomType<int> item,
            TextWriter log)
        {
            log.WriteLine($"Via custom type {item.Name}:{item.Value}");
        }
        #endregion
#endif
    }
}
