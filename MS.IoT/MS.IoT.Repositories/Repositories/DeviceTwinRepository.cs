using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;

namespace MS.IoT.Repositories
{
    public class DeviceTwinRepository : IDeviceTwinRepository
    {
        private string _iotHubConnectionString;
        private RegistryManager _registryManager;
        private JobClient _jobClient;
        private bool _jobRunning = false;

        public DeviceTwinRepository(string iotHubConnectionString)
        {
            _iotHubConnectionString = iotHubConnectionString;
            _registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            _jobClient = JobClient.CreateFromConnectionString(iotHubConnectionString);
        }

        public async Task<IEnumerable<Device>> GetDevices()
        {
            try
            {
                var devices = await _registryManager.GetDevicesAsync(20);
                return devices;
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices {@error}", e.Message);
                throw e;
            }
        }

        public async Task<Device> GetDevice(string deviceId)
        {
            try
            {
                Microsoft.Azure.Devices.Device device = await _registryManager.GetDeviceAsync(deviceId);

                if (device == null)
                {
                    return null;
                }
                else
                {
                    return device;
                }
            }
            catch (Exception e)
            {
                Log.Error("Get iotHub Device {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeviceTwinModel> GetDeviceTwinAsync(string deviceId)
        {
            try
            {
                IQuery query = _registryManager.CreateQuery("SELECT * FROM devices where deviceId='" + deviceId + "'");
                while (query.HasMoreResults)
                {
                    var page = await query.GetNextAsTwinAsync();
                    foreach (var twin in page)
                    {
                        var deviceTwinmodel = new DeviceTwinModel()
                        {
                            DeviceId = twin.DeviceId,
                            Tags = JsonConvert.DeserializeObject<DeviceTwinTagsModel>(twin.Tags.ToString()),
                            Desired = JsonConvert.DeserializeObject<DeviceTwinDesiredModel>(twin.Properties.Desired.ToString()),
                            Reported = JsonConvert.DeserializeObject<DeviceTwinReportedModel>(twin.Properties.Reported.ToString()),
                        };
                        if (deviceTwinmodel.Reported.Features != null)
                        {
                            foreach (string featureName in deviceTwinmodel.Reported.Features.Keys)
                                deviceTwinmodel.Reported.Features[featureName].Name = featureName;
                        }

                        return deviceTwinmodel;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<List<DeviceTwinFlatModel>> GetDevicesTwinAsync()
        {
            try
            {
                string queryString =
                    @"SELECT
                    deviceId, 
                    connectionState,
                    tags.productFamily,
                    tags.productType,
                    tags.productName,
                    tags.userId,
                    tags.manufacturedDate,
                    tags.retailerName,
                    tags.retailerRegion,
                    tags.shippedDate,
                    tags.location,
                    tags.customTags,
                    properties.reported.statusCode,
                    properties.reported.ipAddress,
                    properties.reported.firmwareVersion,
                    properties.reported.activationDate,
                    properties.reported.deviceState
                    FROM devices"; //properties.reported.heartbeat,

                List<DeviceTwinFlatModel> deviceTwinsLight = new List<DeviceTwinFlatModel>();
                IQuery query = _registryManager.CreateQuery(queryString);
                while (query.HasMoreResults)
                {
                    var page = await query.GetNextAsJsonAsync();
                    foreach (var twin in page)
                        deviceTwinsLight.Add(JsonConvert.DeserializeObject<DeviceTwinFlatModel>(twin));
                }
                return deviceTwinsLight;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }


        public async Task<Device> CreateDevice(string deviceId)
        {
            try
            {
                Device device = await GetDevice(deviceId);

                if (device == null)
                {
                    try
                    {
                        device = await _registryManager.AddDeviceAsync(new Device(deviceId));
                    }
                    catch (DeviceAlreadyExistsException)
                    {
                        device = await _registryManager.GetDeviceAsync(deviceId);
                    }
                }
                return device;
            }
            catch (Exception e)
            {
                Log.Error("Create IoTHub Device {@error}", e.Message);
                throw e;
            }
        }

        public async Task<Device> CreateAndInitializeDeviceTwin(string deviceId, DeviceTwinTagsModel tags)
        {
            try
            {
                Device device = await CreateDevice(deviceId);
                await UpdateDeviceSync(device.Id, null, JsonConvert.SerializeObject(tags));
                return device;
            }
            catch (Exception e)
            {
                Log.Error("Create IoTHub Device {@error}", e.Message);
                throw e;
            }
        }

        public async Task<Device> ImportInitializeDeviceTwin(string deviceId, DeviceTwinImportModel tags)
        {
            try
            {
                Device device = await CreateDevice(deviceId);

                if (device != null)
                {
                    await UpdateDeviceSync(device.Id, null, JsonConvert.SerializeObject(tags));
                    return device;
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error("Create IoTHub Device {@error}", e.Message);
                throw e;
            }
        }

        public async Task InitializeDeviceTwinDesiredFeaturesAsync(DeviceTwinDesiredFeaturesModel deviceTwinFeatureModel)
        {
            try
            {
                dynamic features = new ExpandoObject();
                features.features = new Dictionary<string, bool>();
                foreach (var feature in deviceTwinFeatureModel.Features)
                    features.features[feature.Name] = feature.IsActivated;

                await UpdateDeviceSync(deviceTwinFeatureModel.DeviceId, JsonConvert.SerializeObject(features), null);
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Device Twin Features error {@error}", e.Message);
                throw e;
            }
        }

        public async Task UpdateDeviceTwinDesiredFeatureAsync(DeviceTwinDesiredSingleFeatureModel deviceTwinFeaturesModel)
        {
            try
            {
                dynamic features = new ExpandoObject();
                features.features = new Dictionary<string, bool>();
                features.features[deviceTwinFeaturesModel.Feature.Name] = deviceTwinFeaturesModel.Feature.IsActivated;

                await UpdateDeviceSync(deviceTwinFeaturesModel.DeviceId, JsonConvert.SerializeObject(features), null);
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Device Twin Features error {@error}", e.Message);
                throw e;
            }
        }


        public async Task<DeviceUpdateResult> UpdateDevicesAsync(List<string> deviceIds, string jsonDesired, string jsonTags)
        {
            if (deviceIds == null || deviceIds.Count == 0 ||
                (string.IsNullOrEmpty(jsonTags) && string.IsNullOrEmpty(jsonDesired)))
                return new DeviceUpdateResult(false, string.Empty, "Nothing to update");

            if (_jobRunning)
                return new DeviceUpdateResult(false, string.Empty, "A job is already running. Please wait till the previous job has finished running.");

            _jobRunning = true;

            string jobId = Guid.NewGuid().ToString();
            try
            {
                var twin = new Twin();
                if (!string.IsNullOrEmpty(jsonDesired))
                    twin.Properties.Desired = JsonConvert.DeserializeObject<TwinCollection>(jsonDesired);
                if (!string.IsNullOrEmpty(jsonTags))
                    twin.Tags = JsonConvert.DeserializeObject<TwinCollection>(jsonTags);
                twin.ETag = "*";

                string query = "deviceId IN ['" + String.Join("','", deviceIds) + "']";
                
                int timeout = 10;
                if (deviceIds.Count > timeout)
                    timeout = deviceIds.Count * 2;

                JobResponse result = await _jobClient.ScheduleTwinUpdateAsync(jobId, query, twin, DateTime.Now, timeout);
                //TODO: Implement proper job handling
                //while ((result.Status != JobStatus.Completed) && (result.Status != JobStatus.Failed))
                //{
                //    result = await _jobClient.GetJobAsync(jobId);
                //    Thread.Sleep(2000);
                //}

                _jobRunning = false;

                //if (result.Status == JobStatus.Completed)
                    return new DeviceUpdateResult(true, jobId, string.Empty);
                //return new DeviceUpdateResult(false, jobId, result.FailureReason);
            }
            catch (Exception e)
            {
                _jobRunning = false;
                Log.Error(e.Message);
                return new DeviceUpdateResult(false, jobId, e.Message);
            }
        }

        public async Task<bool> UpdateDeviceSync(string deviceId, string jsonDesired, string jsonTags)
        {
            if (string.IsNullOrEmpty(deviceId) ||
                (string.IsNullOrEmpty(jsonTags) && string.IsNullOrEmpty(jsonDesired)))
                return false;

            try
            {
                Twin twin = new Twin();
                if (!string.IsNullOrEmpty(jsonDesired))
                    twin.Properties.Desired = JsonConvert.DeserializeObject<TwinCollection>(jsonDesired);
                if (!string.IsNullOrEmpty(jsonTags))
                    twin.Tags = JsonConvert.DeserializeObject<TwinCollection>(jsonTags);
                twin.ETag = "*";

                await _registryManager.UpdateTwinAsync(deviceId, twin, twin.ETag);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }

            return true;
        }


        public async Task DeleteDeviceAsync(string deviceId)
        {
            try
            {
                Microsoft.Azure.Devices.Device device = await _registryManager.GetDeviceAsync(deviceId);
                if (device != null)
                {
                    try
                    {
                        await _registryManager.RemoveDeviceAsync(device);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Device Delete exception" + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Delete device error {@error}", e.Message);
                throw e;
            }
        }

        public async Task DeleteMultipleDevicesAsync(List<string> deviceIds)
        {
            try
            {
                List<Microsoft.Azure.Devices.Device> devices = new List<Microsoft.Azure.Devices.Device>();
                foreach (var deviceId in deviceIds)
                {
                    Microsoft.Azure.Devices.Device device = new Device(deviceId);
                    if (device != null)
                    {
                        devices.Add(device);
                    }
                }

                if (devices.Count > 0)
                    await _registryManager.RemoveDevices2Async(devices, true, new System.Threading.CancellationToken());
            }
            catch (Exception e)
            {
                Log.Error("Delete multiple devices error {@error}", e.Message);
                throw e;
            }
        }  
    }
}
