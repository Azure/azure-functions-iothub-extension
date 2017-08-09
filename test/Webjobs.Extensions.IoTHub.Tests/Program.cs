// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs;
using SampleFunctions;
using System;
using IoTHubExtension = Microsoft.Azure.WebJobs.Extensions.IoTHub;

namespace Host
{
    // WebJobs is .NET 4.6 
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JobHostConfiguration();
            config.DashboardConnectionString = null;

            // apply config before creating the host.
            var cloudToDeviceExtension = new IoTHubExtension.Config.IoTCloudToDeviceExtension();
            config.AddExtension(cloudToDeviceExtension);

            var directMethodExtension = new IoTHubExtension.Config.IoTDirectMethodExtension();
            config.AddExtension(directMethodExtension);

            var setDeviceTwinExtension = new IoTHubExtension.Config.IoTSetDeviceTwinExtension();
            config.AddExtension(setDeviceTwinExtension);

            var getDeviceTwinExtension = new IoTHubExtension.Config.IoTGetDeviceTwinExtension();
            config.AddExtension(getDeviceTwinExtension);

            // Debug diagnostics!
            config.CreateMetadataProvider().DebugDumpGraph(Console.Out);

            var host = new JobHost(config);

            //var method = typeof(Functions).GetMethod("Dummy");

            //Test some invocations.
            //var method = typeof(Functions).GetMethod("WriteMessageFromC2D");
            //host.Call(method);

            var method = typeof(Functions).GetMethod("WriteMessageFromC2DArg");
            host.Call(method, new { deviceId = "receiverBob" });

            //Test some invocations.
            method = typeof(Functions).GetMethod("DirectInvokeMethod");
            host.Call(method, new { deviceId = "receiverAlice" });

            //// Test some invocations. 
            method = typeof(Functions).GetMethod("SetDeviceTwin");
            host.Call(method, new { deviceId = "receiverBob" });


            //// Test some invocations. 
            method = typeof(Functions).GetMethod("GetDeviceTwinTwinObject");
            host.Call(method, new { deviceId = "receiverBob", messageId = "123" });

            // host.RunAndBlock();
        }
    }
}
