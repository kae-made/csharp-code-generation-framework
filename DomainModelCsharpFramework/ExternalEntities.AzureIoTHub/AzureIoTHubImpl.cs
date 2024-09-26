// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.AzureIoTHub
{
    public class AzureIoTHubImpl : AIHWrapper
    {
        protected static AzureIoTHubImpl instance;

        public override async Task<(string resultPayload, int status)> InvokeOperation(string name, string payload, string deviceId, string moduleName)
        {
            var method = new CloudToDeviceMethod(name);
            if (!string.IsNullOrEmpty(payload))
            {
                method.SetPayloadJson(payload);
            }
            CloudToDeviceMethodResult invocationResult = null;
            if (string.IsNullOrEmpty(moduleName))
            {
                logger.LogInfo($"Invoking {method.MethodName} of {deviceId}...");
                invocationResult = await serviceClient.InvokeDeviceMethodAsync(deviceId, method);
                logger.LogInfo($"Invoked {method.MethodName} of {deviceId} Status - {invocationResult.Status}");

            }
            else
            {
                logger.LogInfo($"Invoking {method.MethodName} of {deviceId}.{moduleName}...");
                invocationResult = await serviceClient.InvokeDeviceMethodAsync(deviceId, moduleName, method);
                logger.LogInfo($"Invoked {method.MethodName} of {deviceId}.{moduleName} Status - {invocationResult.Status}");
            }
            return (invocationResult.GetPayloadAsJson(), invocationResult.Status);
        }

        public override async Task SendCommand(string command, string deviceId, string moduleName = null)
        {
            var sendingMessage = new Message(System.Text.Encoding.UTF8.GetBytes(command));
            if (string.IsNullOrEmpty(moduleName))
            {
                logger.LogInfo($"Sending to {deviceId}...");
                await serviceClient.SendAsync(deviceId, sendingMessage);
                logger.LogInfo($"Send to {deviceId}.");
            }
            else
            {
                logger.LogInfo($"Sending to {deviceId}.{moduleName}...");
                await serviceClient.SendAsync(deviceId, moduleName, sendingMessage);
                logger.LogInfo($"Send to {deviceId}.{moduleName}.");
            }
        }

        public override async Task UpdateProperty(string name, object value, string deviceId, string moduleName = "")
        {
            Twin currentTwin = null;
            if (string.IsNullOrEmpty(moduleName))
            {
                currentTwin = await registryManager.GetTwinAsync(deviceId);
            }
            else
            {
                currentTwin = await registryManager.GetTwinAsync(deviceId, moduleName);
            }
            var updateTwin = new
            {
                properties = new
                {
                    desired = new
                    {
                        name = value
                    }

                }
            };
            string dtPatch = Newtonsoft.Json.JsonConvert.SerializeObject(updateTwin);
            if (string.IsNullOrEmpty(moduleName))
            {
                logger.LogInfo($"Updating {name} property to {deviceId}...");
                await registryManager.UpdateTwinAsync(deviceId, dtPatch, currentTwin.ETag);
                logger.LogInfo($"Updated {name} property to {deviceId}.");
            }
            else
            {
                logger.LogInfo($"Updating {name} property to {deviceId}.{moduleName}...");
                await registryManager.UpdateTwinAsync(deviceId, moduleName, dtPatch, currentTwin.ETag);
                logger.LogInfo($"Updated {name} property to {deviceId}.{moduleName}.");
            }
        }
    }
}
