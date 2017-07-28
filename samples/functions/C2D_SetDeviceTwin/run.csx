#r "Newtonsoft.Json"
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/* Sending D2C messages to C2D */ 
public static void Run(string myEventHubMessage, ICollector<string> cloudToDevice, ICollector<string> setDeviceTwin, TraceWriter log)
{
    // myEventHubMessage = D2C message 
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    
    var msgJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(myEventHubMessage);

    var c2dItem = new
        {
            DeviceId = msgJson["DeviceId"],
            MessageId = msgJson["MessageId"],
            Message = "CLOUD " + msgJson["Message"] 
        };  

    var setDTItem = new
        {
            DeviceId = "receiverCarol",
            UpdateId = msgJson["MessageId"],
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
        cloudToDevice.Add(JsonConvert.SerializeObject(c2dItem));
    }
    catch(Exception e) {
        log.Error("Exception caught for c2dItem method: ", e);
        log.Info("failed to add to Dave");
    }

    try {
        setDeviceTwin.Add(JsonConvert.SerializeObject(setDTItem));
    }
    catch(Exception e) {
        log.Error("Exception caught while sending DT ", e);
    }
}

