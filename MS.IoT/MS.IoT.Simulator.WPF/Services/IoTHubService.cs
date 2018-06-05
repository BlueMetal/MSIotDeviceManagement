using MS.IoT.Simulator.WPF.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
using MS.IoT.Simulator.WPF.Services.Interfaces;

namespace MS.IoT.Simulator.WPF.Services
{
    /// <summary>
    /// IoTHubService
    /// </summary>
    public class IoTHubService : IIoTHubService, IDisposable
    {
        static RegistryManager registryManager;

        /// <summary>
        /// Main Construction
        /// </summary>
        public IoTHubService()
        {
            registryManager = RegistryManager.CreateFromConnectionString(AppConfig.IoTHub.ConnectionString);
        }


        /// <summary>
        /// Register a new device in IoT Hub and returns its connection string.
        /// If it already exists, it will return the connection string of the existing device.
        /// </summary>
        /// <param name="deviceId">Name of the device</param>
        /// <returns></returns>
        public async Task<string> RegisterDeviceInIoTHubAsync(string deviceId)
        {
            Device device = await registryManager.GetDeviceAsync(deviceId);         

            if (device == null)
            {
                try
                {
                    device = await registryManager.AddDeviceAsync(new Device(deviceId));
                }
                catch (DeviceAlreadyExistsException)
                {
                    device = await registryManager.GetDeviceAsync(deviceId);
                }

            }
            var deviceConnectionString = String.Format("HostName={0};DeviceId={1};SharedAccessKey={2}",
                AppConfig.IoTHub.HostName,
                deviceId, device.Authentication.SymmetricKey.PrimaryKey);

            return deviceConnectionString;
        }

        /// <summary>
        /// Get registered devices from IoT Hub
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable> GetDevices()
        {
            List<Device> list = new List<Device>(await registryManager.GetDevicesAsync(1));
            return list;
        }

        /// <summary>
        /// Method to be used when the application exits
        /// </summary>
        public void Dispose()
        {
            if (registryManager == null)
                registryManager.Dispose();
        }
    }
}
