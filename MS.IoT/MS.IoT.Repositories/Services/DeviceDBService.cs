using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MS.IoT.Common;
using System.Collections.Concurrent;
using Microsoft.Azure.Devices;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;

namespace MS.IoT.Repositories.Services
{
    public class DeviceDBService : IDeviceDBService
    {
        private ICacheEngine _Cache;
        private IDeviceTwinRepository _DeviceTwinRepo;
        private object _Sync = new object(); //For lock

        private const string CHECK_IP_API = "http://freegeoip.net/json";

        public DeviceDBService(string iotHubConnectionString)
        {
            _Cache = new MemoryCacheEngine();
            _DeviceTwinRepo = new DeviceTwinRepository(iotHubConnectionString);
        }

        #region Data Management
        public async Task<bool> RefreshDeviceDBCache()
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            try
            {
                //Initialization
                if ((!deviceDB.IsDevicesDBCacheInitialized || 
                    (deviceDB.IsDevicesDBCacheInitialized && DateTime.UtcNow.AddMinutes(-5) >= deviceDB.LastUpdate)) && 
                    !deviceDB.IsDevicesDBCacheLoading)
                {
                    deviceDB.IsDevicesDBCacheLoading = true;
                    deviceDB.LastUpdate = DateTime.UtcNow;
                    ConcurrentDictionary<string, DeviceTwinFlatModel> twins = new ConcurrentDictionary<string, DeviceTwinFlatModel>();
                    List<DeviceTwinFlatModel> devicesLocation = new List<DeviceTwinFlatModel>();

                    //Get devices
                    List<DeviceTwinFlatModel> devices = await _DeviceTwinRepo.GetDevicesTwinAsync();
                    foreach (DeviceTwinFlatModel device in devices)
                    {
                        if (device.Location == null)
                            device.Location = new LocationAddress();
                        if (device.DeviceState == null)
                            device.DeviceState = new Dictionary<string, string>();
                        if (device.CustomTags == null)
                            device.CustomTags = new Dictionary<string, string>();
                        device.ConnectionState = device.ActivationDate == DateTime.MinValue ? DeviceConnectionStatus.NotActivated : device.ConnectionState;
                        if (!string.IsNullOrEmpty(device.IpAddress) && device.IpAddress != device.Location.IpAddress)
                            devicesLocation.Add(device);
                        twins[device.DeviceId] = device;  
                    }
                    deviceDB.Devices = twins;
                    deviceDB.IsDevicesDBCacheLoading = false;
                    deviceDB.IsDevicesDBCacheInitialized = true;

                    await RefreshDevicesTwinLocation(devicesLocation);
                }
                return true;   
            }
            catch (Exception e)
            {
                deviceDB.IsDevicesDBCacheLoading = false;
                //deviceDB.IsDevicesDBCacheInitialized = false;
                Log.Error(e.Message);
                return false;
            }
        }

        private async Task<LocationAddress> GetLocationByIPAddress(string ipAddress)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var data = await client.GetStringAsync(CHECK_IP_API);
                    var resp = JsonConvert.DeserializeObject<LocationAddress>(data);
                    resp.IpAddress = ipAddress;
                    return resp;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task RefreshDevicesTwinLocation(List<DeviceTwinFlatModel> devices)
        {
            try
            {
                DevicesDBCache deviceDB = _Cache.DevicesDB;
                if (!deviceDB.IsDevicesDBCacheInitialized || deviceDB.IsDevicesDBCacheLocationUpdating)
                    return;

                //Set updating to true
                deviceDB.IsDevicesDBCacheLocationUpdating = true;

                //Get devices
                ConcurrentDictionary<string, DeviceTwinFlatModel> deviceEntities = deviceDB.Devices;

                foreach (DeviceTwinFlatModel device in devices)
                {
                    LocationAddress location = await GetLocationByIPAddress(device.IpAddress);

                    if (deviceEntities.ContainsKey(device.DeviceId))
                    {
                        DeviceTwinFlatModel twin = deviceEntities[device.DeviceId];
                        twin.Location = location;
                        dynamic locationPatch = new ExpandoObject();
                        locationPatch.location = location;
                        await _DeviceTwinRepo.UpdateDeviceSync(device.DeviceId, null, JsonConvert.SerializeObject(locationPatch));
                    }
                }

                //Set updating to false
                deviceDB.IsDevicesDBCacheLocationUpdating = false;
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                throw e;
            }
        }
        #endregion

        #region Get Data
        public async Task<DeviceTwinModel> GetDeviceTwinAsync(string deviceId)
        {
            return await _DeviceTwinRepo.GetDeviceTwinAsync(deviceId);
        }

        public IEnumerable<string> GetDevicesTwinIds(DeviceQueryRuleGroup where)
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            if (!deviceDB.IsDevicesDBCacheInitialized)
                return new List<string>();

            try
            {
                //Get cached devices
                IEnumerable<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;
                return InMemoryLinqDeviceQueryHelper.GetFilteredDevicesIds(devices, where);
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin Ids error {@error}", e.Message);
                return new List<string>();
            }
        }

        public DeviceQueryResponse GetDevicesTwinInfoAsync(DeviceQueryConfiguration queryConfiguration)
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            if (!deviceDB.IsDevicesDBCacheInitialized)
                return new DeviceQueryResponse() { Success = true };

            try
            {
                //Get cached devices
                IEnumerable<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;

                //GroupBy Filter
                IEnumerable<DeviceTwinFlatModel> filteredQuery = InMemoryLinqDeviceQueryHelper.GetFilteredDevicesGroup(devices, queryConfiguration.Where);

                int nbrPages = filteredQuery.Count();
                if (string.IsNullOrEmpty(queryConfiguration.GroupBy))
                {
                    if (filteredQuery.Count() > 0)
                    {
                        filteredQuery = InMemoryLinqDeviceQueryHelper.GetOrderedDevices(filteredQuery, queryConfiguration.OrderBy, queryConfiguration.OrderBySorting)
                        .Skip(queryConfiguration.PageIndex * queryConfiguration.ItemsPerPage)
                        .Take(queryConfiguration.ItemsPerPage);
                    }
                    return new DeviceQueryResponse()
                    {
                        Items = filteredQuery.Count() > 0 ? filteredQuery.Select(p => new DeviceInfoEntity(p)) : new List<DeviceInfoEntity>(),
                        ItemsCount = nbrPages,
                        IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                        IsDatabaseLoaded = deviceDB.IsDevicesDBCacheInitialized,
                        ErrorMessage = string.Empty,
                        Success = true,
                        LastUpdate = deviceDB.LastUpdate
                    };
                }
                else
                {
                    return GetGroupedDevicesResponse(deviceDB, filteredQuery, queryConfiguration);
                }
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                return new DeviceQueryResponse()
                {
                    Items = new List<DeviceInfoEntity>(),
                    ItemsCount = 0,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    IsDatabaseLoaded = true,
                    ErrorMessage = e.Message,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
        }

        public DeviceMapQueryResponse GetDevicesTwinMapAsync(DeviceMapQueryConfiguration queryMapConfiguration)
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            if (!deviceDB.IsDevicesDBCacheInitialized)
                return new DeviceMapQueryResponse() { Success = true };

            try
            {
                //Filtering search
                IEnumerable<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;

                IEnumerable<DeviceTwinFlatModel> filteredQuery = InMemoryLinqDeviceQueryHelper.GetFilteredDevicesGroup(devices, new DeviceQueryRuleGroup(LogicalOperators.And) { Rules = queryMapConfiguration.Filters });

                if (queryMapConfiguration.ViewId == "alerts")
                    filteredQuery = filteredQuery.Where(p => p.StatusCode != 0);

                List<DeviceMapEntity> mapDevicesAddresses = filteredQuery.Where(p => !string.IsNullOrEmpty(p.Location.CountryCode)).Select(p => new DeviceMapEntity()
                //{ Count = 1, Name = p.ProductName, GeoLatitude = p.GeoLatitude.ToString(), GeoLongitude = p.GeoLongitude.ToString() })
                { GeoLatitude = p.Location.Latitude.ToString(), GeoLongitude = p.Location.Longitude.ToString() })
                .ToList();
                return new DeviceMapQueryResponse()
                {
                    Pushpins = mapDevicesAddresses,
                    IsDatabaseLoaded = deviceDB.IsDevicesDBCacheInitialized,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = string.Empty,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                return new DeviceMapQueryResponse()
                {
                    Pushpins = new List<DeviceMapEntity>(),
                    IsDatabaseLoaded = true,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = e.Message,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
        }

        public DeviceMapAreaQueryResponse GetDevicesTwinMapAreaAsync(DeviceMapQueryConfiguration queryMapConfiguration)
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            if (!deviceDB.IsDevicesDBCacheInitialized)
                return new DeviceMapAreaQueryResponse() { Success = true };

            try
            {
                //DeviceTwinSummaryAggregationsModel deviceSummaryAgg = new DeviceTwinSummaryAggregationsModel();
                Dictionary<string, int> devicesRetailer = new Dictionary<string, int>();
                IEnumerable<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;

                IEnumerable<DeviceTwinFlatModel> filteredQuery = InMemoryLinqDeviceQueryHelper.GetFilteredDevicesGroup(devices, new DeviceQueryRuleGroup(LogicalOperators.And) { Rules = queryMapConfiguration.Filters });

                IEnumerable<IGrouping<string, DeviceTwinFlatModel>> groups = null;

                if (queryMapConfiguration.ViewId == "count")
                {
                    groups = filteredQuery.Where(p => !string.IsNullOrEmpty(p.RetailerRegion)).GroupBy(p => p.RetailerRegion);
                    foreach (IGrouping<string, DeviceTwinFlatModel> group in groups)
                        devicesRetailer.Add(group.Key, group.Count());
                }
                else if (queryMapConfiguration.ViewId == "activated")
                {
                    groups = filteredQuery.Where(p => !string.IsNullOrEmpty(p.RetailerRegion)).GroupBy(p => p.RetailerRegion);
                    foreach (IGrouping<string, DeviceTwinFlatModel> group in groups)
                        devicesRetailer.Add(group.Key, 100 - (group.Where(x => x.ConnectionState == DeviceConnectionStatus.NotActivated).Count() * 100 / group.Count()));
                }
                else if (queryMapConfiguration.ViewId == "retailerName")
                {
                    string retailer = queryMapConfiguration.Filters.Find(p => p.Field == queryMapConfiguration.ViewId)?.Value;
                    if (!string.IsNullOrEmpty(retailer))
                    {
                        groups = devices.Where(p => !string.IsNullOrEmpty(p.RetailerRegion)).GroupBy(p => p.RetailerRegion);
                        foreach (IGrouping<string, DeviceTwinFlatModel> group in groups)
                            devicesRetailer.Add(group.Key, 100 - (group.Where(x => x.RetailerName != retailer).Count() * 100 / group.Count()));
                    }
                }
                else if (queryMapConfiguration.ViewId == "productFamily")
                {
                    string productFamily = queryMapConfiguration.Filters.Find(p => p.Field == queryMapConfiguration.ViewId)?.Value;
                    if (!string.IsNullOrEmpty(productFamily))
                    {
                        groups = devices.Where(p => !string.IsNullOrEmpty(p.RetailerRegion)).GroupBy(p => p.RetailerRegion);
                        foreach (IGrouping<string, DeviceTwinFlatModel> group in groups)
                            devicesRetailer.Add(group.Key, 100 - (group.Where(x => x.ProductFamily != productFamily).Count() * 100 / group.Count()));
                    }
                }

                return new DeviceMapAreaQueryResponse()
                {
                    AreaItems = devicesRetailer,
                    IsDatabaseLoaded = deviceDB.IsDevicesDBCacheInitialized,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = string.Empty,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                return new DeviceMapAreaQueryResponse()
                {
                    AreaItems = new Dictionary<string, int>(),
                    IsDatabaseLoaded = true,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = e.Message,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
        }

        public DeviceMapGroupsInformation GetMapGroupsInformations()
        {
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            if (!deviceDB.IsDevicesDBCacheInitialized)
                return new DeviceMapGroupsInformation();

            ICollection<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;

            return new DeviceMapGroupsInformation()
            {
                Retailers = devices.GroupBy(p => p.RetailerName).Select(p => new MapFilterItem() { Name = p.Key, DisplayName = p.Key, Count = p.Count() }).OrderBy(p => p.DisplayName).ToList(),
                ProductFamilies = devices.GroupBy(p => p.ProductFamily).Select(p => new MapFilterItem() { Name = p.Key, DisplayName = p.Key, Count = p.Count() }).OrderBy(p => p.DisplayName).ToList(),
                ConnectionStates = devices.GroupBy(p => p.ConnectionState).Select(p => new MapFilterItem() { Name = ((int)p.Key).ToString(), DisplayName = ((int)p.Key).ToString(), Count = p.Count() }).OrderBy(p => p.DisplayName).ToList(),
                ItemsCount = devices.Count
            };
        }

        private DeviceQueryResponse GetGroupedDevicesResponse(DevicesDBCache deviceDB, IEnumerable<DeviceTwinFlatModel> filteredQuery, DeviceQueryConfiguration queryConfiguration)
        {
            try
            {
                IEnumerable<DeviceGroup> groups = InMemoryLinqDeviceQueryHelper.GetDevicesGroups(filteredQuery, queryConfiguration.GroupBy, queryConfiguration.OrderBySorting);

                int nbrGroups = groups.Count();
                groups = groups
                    .Skip(queryConfiguration.PageIndex * queryConfiguration.ItemsPerPage)
                    .Take(queryConfiguration.ItemsPerPage);

                return new DeviceQueryResponse()
                {
                    Groups = groups,
                    GroupsCount = nbrGroups,
                    ItemsCount = filteredQuery.Count(),
                    IsDatabaseLoaded = deviceDB.IsDevicesDBCacheInitialized,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = string.Empty,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                return new DeviceQueryResponse()
                {
                    Groups = new List<DeviceGroup>(),
                    GroupsCount = 0,
                    ItemsCount = 0,
                    IsDatabaseLoaded = deviceDB.IsDevicesDBCacheInitialized,
                    IsDatabaseLoading = deviceDB.IsDevicesDBCacheLoading,
                    ErrorMessage = string.Empty,
                    Success = true,
                    LastUpdate = deviceDB.LastUpdate
                };
            }
        }

        public DeviceTwinSummaryAggregationsModel GetDevicesSummaryAggregationsAsync(string topActivatedGroupBy)
        {
            try
            {
                DevicesDBCache deviceDB = _Cache.DevicesDB;
                if (!deviceDB.IsDevicesDBCacheInitialized)
                    return new DeviceTwinSummaryAggregationsModel();

                IEnumerable<DeviceTwinFlatModel> devices = deviceDB.Devices.Values;
                DeviceTwinSummaryAggregationsModel deviceSummaryAgg = new DeviceTwinSummaryAggregationsModel();

                //Device Summary
                deviceSummaryAgg.TotalDevicesCount = deviceDB.Count;
                deviceSummaryAgg.ConnectedDevicesCount = devices.Count(p => p.ConnectionState == DeviceConnectionStatus.Connected);
                deviceSummaryAgg.DisconnectedDevicesCount = devices.Count(p => p.ConnectionState == DeviceConnectionStatus.Disconnected);
                deviceSummaryAgg.ActivatedDevicesCount = deviceSummaryAgg.ConnectedDevicesCount + deviceSummaryAgg.DisconnectedDevicesCount;
                deviceSummaryAgg.NotActivatedDevicesCount = deviceSummaryAgg.TotalDevicesCount - deviceSummaryAgg.ActivatedDevicesCount;

                //Alert
                deviceSummaryAgg.AlertCounts = devices.Where(p => p.StatusCode != 0).GroupBy(p => p.StatusCode).OrderByDescending(p => p.Count()).Select(p => new AlertCount() { AlertCode = p.Key, Count = p.Count() }).ToList();

                IEnumerable<IGrouping<string, DeviceTwinFlatModel>> topActivatedDevices = InMemoryLinqDeviceQueryHelper.GetGroupByString(devices, topActivatedGroupBy);
                deviceSummaryAgg.DevicePerGroupActivated = topActivatedDevices
                    .Select(p => new DevicePerGroupActivated() { GroupName = p.Key, PercentageActivated = 100 - (p.Where(x => x.ConnectionState == DeviceConnectionStatus.NotActivated).Count() * 100 / p.Count()) })
                    .OrderByDescending(p => p.PercentageActivated)
                    .Take(10)
                    .ToList();

                return deviceSummaryAgg;
            }
            catch (Exception e)
            {
                Log.Error("Get IoTHub Devices Twin error {@error}", e.Message);
                return new DeviceTwinSummaryAggregationsModel();
            }
        }

        public List<DeviceFieldModel> GetDeviceFields()
        {
            return InMemoryLinqDeviceQueryHelper.Fields;
        }
        #endregion

        #region Data Operations
        public async Task InitializeDeviceTwinDesiredFeaturesAsync(DeviceTwinDesiredFeaturesModel deviceTwinFeatureModel)
        {
            await _DeviceTwinRepo.InitializeDeviceTwinDesiredFeaturesAsync(deviceTwinFeatureModel);
        }

        public async Task UpdateDeviceTwinDesiredFeatureAsync(DeviceTwinDesiredSingleFeatureModel deviceTwinFeaturesModel)
        {
            await _DeviceTwinRepo.UpdateDeviceTwinDesiredFeatureAsync(deviceTwinFeaturesModel);
        }

        public async Task DeleteDeviceAsync(string deviceId)
        {
            await _DeviceTwinRepo.DeleteDeviceAsync(deviceId);

            //Empty cache
            _Cache.RemoveDeviceEntity(deviceId);
        }

        public async Task DeleteMultipleDevicesAsync(List<string> deviceIds)
        {
            await _DeviceTwinRepo.DeleteMultipleDevicesAsync(deviceIds);

            //Empty cache
            DevicesDBCache deviceDB = _Cache.DevicesDB;
            foreach (var deviceId in deviceIds)
                _Cache.RemoveDeviceEntity(deviceId);
        }

        public async Task<DeviceUpdateResult> UpdateDevicesAsync(List<string> deviceIds, string jsonDesired, string jsonTags)
        {
            return await _DeviceTwinRepo.UpdateDevicesAsync(deviceIds, jsonDesired, jsonTags);
        }

        public async Task<bool> UpdateDeviceSync(string deviceId, string jsonDesired, string jsonTags)
        {
            return await _DeviceTwinRepo.UpdateDeviceSync(deviceId, jsonDesired, jsonTags);
        }

        public async Task<Device> ImportInitializeDeviceTwin(string deviceId, DeviceTwinImportModel tags)
        {
            Device device = await _DeviceTwinRepo.ImportInitializeDeviceTwin(deviceId, tags);

            if (device != null)
            {
                //Update cache
                _Cache.EnsureDeviceEntity(deviceId, tags);
            }
            return device;
        }
        #endregion

        /*private bool AreCoordinatesInBoundaryBox(DeviceTwinFlatModel entity, double[] boundaryBox)
        {
            if (entity.Location.Latitude >= boundaryBox[1] && entity.Location.Latitude <= boundaryBox[0] &&
                entity.Location.Longitude >= boundaryBox[3] && entity.Location.Longitude <= boundaryBox[2])
                return true;
            return false;
        }*/
    }
}
