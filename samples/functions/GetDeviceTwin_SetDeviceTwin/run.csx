#r "Newtonsoft.Json"
#r "Microsoft.Azure.Devices.Shared"
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices.Shared;

public class DataInput 
{
    public string DeviceId { get; set; }
    public string MessageId { get; set; }
    public string Message { get; set; }
}

public static void Run(DataInput myEventHubMessage, Twin getDeviceTwin, ICollector<string> setDeviceTwin, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage.DeviceId}");

    log.Info($"C# get device twin: {getDeviceTwin.DeviceId}");

    log.Info($"C# get device twin: {getDeviceTwin.Properties.Desired}");

    // set device twin
    var setDTItem = new
    {
        DeviceId = getDeviceTwin.DeviceId,
        UpdateId = myEventHubMessage.MessageId,
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

    try {
        setDeviceTwin.Add(JsonConvert.SerializeObject(setDTItem));
    }
    catch(Exception e) {
        log.Error("Exception caught while sending DT ", e);
    }
        
}
