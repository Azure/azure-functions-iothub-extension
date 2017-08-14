'use strict';

var Mqtt = require('azure-iot-device-mqtt').Mqtt;
var DeviceClient = require('azure-iot-device').Client;

var connectionString = 'HostName=<Host Name>;DeviceId=<Device Name>;SharedAccessKey=<Device Key>';
var client = DeviceClient.fromConnectionString(connectionString, Mqtt);

function onWriteLine(request, response) {
    response.send(200, 'Input was written to log.', function (err) {
        if (err) {
            console.error('An error occurred when sending a method response:\n' + err.toString());
        } else {
            console.log('Response to method \'' + request.methodName + "\' with Payload: \'" + request.payload + '\' sent successfully.');
        }
    });
}

var initConfigChange = function (twin) {
    var currentTelemetryConfig = twin.properties.reported.telemetryConfig;
    currentTelemetryConfig.pendingConfig = twin.properties.desired.telemetryConfig;
    currentTelemetryConfig.status = "\x1b[36m Pending \x1b[0m";

    var patch = {
        telemetryConfig: currentTelemetryConfig
    };
    twin.properties.reported.update(patch, function (err) {
        if (err) {
            console.log('\x1b[36m  Could not report properties \x1b[0m');
        } else {
            console.log('\x1b[36m Reported pending config change: ' + JSON.stringify(patch) + " \x1b[0m");
            setTimeout(function () { completeConfigChange(twin); }, 60000);
        }
    });
}
var completeConfigChange = function (twin) {
    var currentTelemetryConfig = twin.properties.reported.telemetryConfig;

    try {
        currentTelemetryConfig.configId = currentTelemetryConfig.pendingConfig.configId;
        currentTelemetryConfig.sendFrequency = currentTelemetryConfig.pendingConfig.sendFrequency;
        currentTelemetryConfig.status = "\x1b[36m Success \x1b[0m";
        delete currentTelemetryConfig.pendingConfig;

        var patch = {
            telemetryConfig: currentTelemetryConfig
        };
        patch.telemetryConfig.pendingConfig = null;

        twin.properties.reported.update(patch, function (err) {
            if (err) {
                console.error('\x1b[36m Error reporting properties: ' + err + " \x1b[0m");
            } else {
                console.log('\x1b[36m Reported completed config change: ' + JSON.stringify(patch) + " \x1b[0m");
            }
        });
    } catch (error) {
        console.log("Fails to complete setting device twin. Check the currect config to see if your change went through");
        console.log(currentTelemetryConfig)
    }
}

client.open(function (err) {
    if (err) {
        console.error('could not open IotHub client');
    } else {
        console.log('client opened');
        client.onDeviceMethod('writeLine', onWriteLine);

        client.getTwin(function (err, twin) {
            if (err) {
                console.error('\x1b[36m could not get twin \x1b[0m');
            } else {
                console.log('\x1b[36m retrieved device twin \x1b[0m');
                twin.properties.reported.telemetryConfig = {
                    configId: "0",
                    sendFrequency: "24h"
                }
                twin.on('properties.desired', function (desiredChange) {
                    console.log("\x1b[36m received device twin change: " + JSON.stringify(desiredChange) + " \x1b[0m");
                    var currentTelemetryConfig = twin.properties.reported.telemetryConfig;
                    if (desiredChange.telemetryConfig && desiredChange.telemetryConfig.configId !== currentTelemetryConfig.configId) {
                        initConfigChange(twin);
                    }
                });
            }
        });
    }
});