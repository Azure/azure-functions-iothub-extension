### IoTHub Extension

Fully featured IoT Hub input and output bindings to Azure IoT Hub, allowing common interactions between cloud and devices to be done from Azure Functions. Common scenarios currently supported are: 
  * Cloud to Device: output binding that sends messages from Azure Functions to IoTHub, which then transfer the messages to specified device id in the message structure
  * Direct Method: output binding that invokes methods in the device from Azure Functions
  * Set Device Twin: output binding that updates desired properties of specified device from Azure Functions
  * Get Device Twin: input binding that gets device twin of the specified device once the Function's trigger is fired

#### Current Status

This extension is still in the prototype phase and has not been published to NuGet. If you need to process IOT Hub messages with Azure Functions, the currently supported method is documented [here](https://docs.microsoft.com/en-us/samples/azure-samples/functions-js-iot-hub-processing/processing-data-from-iot-hub-with-azure-functions/).

#### Example

##### Cloud to Device
###### function.json


```csharp
{
  "bindings": [
    {
      "type": "eventHubTrigger",
      "name": "myEventHubMessage",
      "direction": "in",
      "path": "messages/events",
      "connection": "IoTConnectionString",
      "consumerGroup": "secondconsumergroup"
    },
    {
      "name": "cloudToDevice",
      "type": "ioTCloudToDevice",
      "direction": "out",
      "connection": "IoTConnectionString" // connection string can differ from the trigger's
    }
  ],
  "disabled": false
}
```

###### run.csx


```csharp
using System;

public static void Run(string myEventHubMessage, ICollector<string> cloudToDevice, ICollector<string> setDeviceTwin, TraceWriter log)
{
    cloudToDevice.Add("{\"DeviceId\":\"myFirstDevice\",\"MessageId\":1,\"Message\":\"C2D message\"}");
}
```

See more [sample code](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/functions) for each scenario

#### Sample Code

[functions folders](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/functions) contains functions to use in Azure Functions portal. To run, do the following:
1. Zip extension .dll files and put them in Function's library via **Advanced tools (Kudu)**
2. Add the path where the extension lives to appsetting using **AzureWebJobs_ExtensionsPath** as key
3. Create a new function in portal and copy codes from selected scenario in the sample folder
4. Change the appsetting key for **connection string** in function.json to **IoTConnectionString** or change the key for **connection string** (and/or **consumerGroup** for EventHubTrigger) in function.json according to your custom app setting. 
5. Run your device code or [sender.js](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices/sender)
Your function should receive messages from **sender.js**
6. Run your device receiver code or select ones in [simulatedDevices folder](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices) according to your scenario. 
  * [receiverAlice](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices/receiverAlice): **Direct Method** or **Set/Get Device Twin**
  * [receiverBob](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices/receiverBob): **Cloud to Device** or **Set/Get Device Twin**
  * [receiverCarol](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices/receiverCarol):  **Set/Get Device Twin**
  * [receiverDave](https://github.com/ElleTojaroon/azure-functions-iothub-extension/tree/master/samples/simulatedDevices/receiverDave): **Cloud to Device** or **Set/Get Device Twin**

> Direct Method assumes that the device has a method matched with the specified method's name given in the argument. Otherwise, Function throws an exception. 

> Executing direct method that takes longer than the lifetime of a Function (5 minutes by default and can be set up to 10 minutes) can never be completed.

## License

This project is under the benevolent umbrella of the [.NET Foundation](http://www.dotnetfoundation.org/) and is licensed under [the MIT License](https://github.com/Azure/azure-webjobs-sdk/blob/master/LICENSE.txt)

## Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
