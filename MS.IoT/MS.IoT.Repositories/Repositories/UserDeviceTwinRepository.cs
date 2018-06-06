using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using MS.IoT.Common;

namespace MS.IoT.Repositories
{
    public class UserDeviceTwinRepository : IUserDeviceTwinRepository
    {
        private static string _iotHubConnectionString;
        private static RegistryManager _registryManager;
        private static ServiceClient _serviceClient;

        public UserDeviceTwinRepository(string iotHubConnectionString)
        {
            _iotHubConnectionString = iotHubConnectionString;
            _registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            _serviceClient=  ServiceClient.CreateFromConnectionString(iotHubConnectionString);

        }

        public async Task<DeviceTwinModel> GetUserDeviceTwinAsync(string userId,string deviceId)
        {
            try
            {
                IQuery query = _registryManager.CreateQuery("SELECT * FROM devices where tags.userId='"+userId+"' and deviceId='"+deviceId+"'");
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

        public async Task<List<DeviceTwinModel>> GetUserDevicesTwinAsync(string userId)
        {
            try
            {
                List<DeviceTwinModel> devicesList = new List<DeviceTwinModel>();
                IQuery query = _registryManager.CreateQuery("SELECT * FROM devices where tags.userId='"+userId+"'");
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
                        devicesList.Add(deviceTwinmodel);
                        
                    }
                }
                return devicesList;
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DirectMethodResponse> UpdateDeviceFeatureDirectMethod(string deviceId, DirectMethodBase method)
        {
            try
            {
                var methodInvocation = new CloudToDeviceMethod(method.MethodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
                methodInvocation.SetPayloadJson(method.GetJson());

                var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

                DirectMethodResponse directResponse = JsonConvert.DeserializeObject<DirectMethodResponse>(response.GetPayloadAsJson());
                return directResponse;
            }
            catch (Exception e)
            {
                Log.Error("feature update {@error}", e.Message);
                throw e;
            }
        }
    }
}
