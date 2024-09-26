// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using Microsoft.Azure.Devices;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.AzureIoTHub
{
    public abstract class AIHWrapper : ExternalEntityDef
    {
        protected string eeKey = "AIH";
        public string EEKey { get { return eeKey; } }

        public Logger Logger { get { return logger; } set { logger = value; } }

        protected ServiceClient serviceClient = null;
        protected RegistryManager registryManager = null;
        protected string connectionString = "";
        protected Logger logger;

        public static readonly string configIoTHubConnectionStringKey = "iothub-connection-string";

        public IList<string> ConfigurationKeys { get { return new List<string> { configIoTHubConnectionStringKey }; } }

        public async void Initialize(IDictionary<string, object> configuration)
        {
            if (logger != null)
            {
                if (!configuration.ContainsKey(configIoTHubConnectionStringKey))
                {
                    logger.LogError($"Azure IoT Hub Wrapper needs {configIoTHubConnectionStringKey} for initialization!");
                    throw new ArgumentOutOfRangeException($"Azure IoT Hub Wrapper needs {configIoTHubConnectionStringKey} for initialization!");
                }
            }
            this.connectionString = (string)configuration[configIoTHubConnectionStringKey];
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            try
            {
                await serviceClient.OpenAsync();
                logger.LogInfo("Connected to IoT Hub as ServiceClient");
                await registryManager.OpenAsync();
                logger.LogInfo("Connected to IoT Hub as RegistryManager");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to connect IoT Hub - {ex.Message}");
            }
        }

        public async void Terminate()
        {
            if (serviceClient != null)
                await serviceClient.CloseAsync();
            if (registryManager != null)
                await registryManager.CloseAsync();
        }

        public abstract Task SendCommand(string command, string deviceId, string moduleName = null);
        public abstract Task UpdateProperty(string name, object value, string deviceId, string moduleName = "");
        public abstract Task<(string resultPayload, int status)> InvokeOperation(string name, string payload, string deviceId, string moduleName = "");

    }
}