#r "Newtonsoft.Json"
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string eventHubMessage, ICollector<string> dm, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {eventHubMessage}");

    var msgJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(eventHubMessage);

    var item = new
    {
        DeviceId = msgJson["DeviceId"],
        InvokeId = msgJson["MessageId"],
        MethodName = msgJson["Message"]
    };  

    dm.Add(JsonConvert.SerializeObject(item));
    
}