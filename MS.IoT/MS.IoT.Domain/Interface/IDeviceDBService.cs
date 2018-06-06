using Microsoft.Azure.Devices;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface IDeviceDBService
    {
        Task<DeviceTwinModel> GetDeviceTwinAsync(string deviceId);
        Task<bool> RefreshDeviceDBCache();
        IEnumerable<string> GetDevicesTwinIds(DeviceQueryRuleGroup where);
        DeviceQueryResponse GetDevicesTwinInfoAsync(DeviceQueryConfiguration queryConfiguration);
        DeviceMapGroupsInformation GetMapGroupsInformations();
        DeviceMapQueryResponse GetDevicesTwinMapAsync(DeviceMapQueryConfiguration queryMapConfiguration);
        DeviceMapAreaQueryResponse GetDevicesTwinMapAreaAsync(DeviceMapQueryConfiguration queryMapConfiguration);
        DeviceTwinSummaryAggregationsModel GetDevicesSummaryAggregationsAsync(string topActivatedGroupBy);
        Task InitializeDeviceTwinDesiredFeaturesAsync(DeviceTwinDesiredFeaturesModel deviceTwinFeatureModel);
        Task UpdateDeviceTwinDesiredFeatureAsync(DeviceTwinDesiredSingleFeatureModel deviceTwinFeaturesModel);
        Task DeleteDeviceAsync(string deviceId);
        Task DeleteMultipleDevicesAsync(List<string> deviceIds);
        Task<DeviceUpdateResult> UpdateDevicesAsync(List<string> deviceIds, string jsonDesired, string jsonTags);
        Task<bool> UpdateDeviceSync(string deviceId, string jsonDesired, string jsonTags);
        Task<Device> ImportInitializeDeviceTwin(string deviceId, DeviceTwinImportModel tags);
        List<DeviceFieldModel> GetDeviceFields();
    }
}
