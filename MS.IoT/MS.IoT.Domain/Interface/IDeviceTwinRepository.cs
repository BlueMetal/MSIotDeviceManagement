using MS.IoT.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IDeviceTwinRepository
    {
        Task<IEnumerable<Microsoft.Azure.Devices.Device>> GetDevices();
        Task<DeviceTwinModel> GetDeviceTwinAsync(string deviceId);
        Task<List<DeviceTwinFlatModel>> GetDevicesTwinAsync();
        Task<Microsoft.Azure.Devices.Device> GetDevice(string deviceId);
        Task<Microsoft.Azure.Devices.Device> CreateDevice(string deviceId);
        Task<Microsoft.Azure.Devices.Device> CreateAndInitializeDeviceTwin(string deviceId, DeviceTwinTagsModel tags);
        Task<Microsoft.Azure.Devices.Device> ImportInitializeDeviceTwin(string deviceId, DeviceTwinImportModel tags);
        Task InitializeDeviceTwinDesiredFeaturesAsync(DeviceTwinDesiredFeaturesModel deviceTwinFeaturesModel);
        Task UpdateDeviceTwinDesiredFeatureAsync(DeviceTwinDesiredSingleFeatureModel deviceTwinFeatureModel);
        Task<DeviceUpdateResult> UpdateDevicesAsync(List<string> deviceIds, string jsonDesired, string jsonTags);
        Task<bool> UpdateDeviceSync(string deviceId, string jsonDesired, string jsonTags);
        Task DeleteDeviceAsync(string deviceId);
        Task DeleteMultipleDevicesAsync(List<string> deviceIds);
    }
}
