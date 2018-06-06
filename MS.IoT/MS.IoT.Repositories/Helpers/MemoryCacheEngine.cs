using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;

namespace MS.IoT.Repositories.Helpers
{
    public class MemoryCacheEngine : ICacheEngine
    {
        private const string CACHE_DEVICES_DB = "DevicePortal_DevicesDB";
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions()); // old behavior was static

        public DevicesDBCache DevicesDB
        {
            get
            {
                return _cache.GetOrCreate(CACHE_DEVICES_DB, ce => new DevicesDBCache());
            }
        }

        public void EnsureDeviceEntity(string deviceId, DeviceTwinTagsModel tags)
        {
            //Update cache
            DeviceTwinFlatModel deviceEntity = null;
            bool result = DevicesDB.Devices.TryGetValue(deviceId, out deviceEntity);
            if (!result)
            {
                deviceEntity = DevicesDB.Devices.GetOrAdd(deviceId, new DeviceTwinFlatModel());
            }

            deviceEntity.ProductFamily = tags.ProductFamily;
            deviceEntity.ProductName = tags.ProductName;
            deviceEntity.RetailerName = tags.RetailerName;
            deviceEntity.RetailerRegion = tags.RetailerRegion;
        }

        public void EnsureDeviceEntity(string deviceId, DeviceTwinImportModel tags)
        {
            //Update cache
            DeviceTwinFlatModel deviceEntity = null;
            bool result = DevicesDB.Devices.TryGetValue(deviceId, out deviceEntity);
            if (!result)
            {
                deviceEntity = DevicesDB.Devices.GetOrAdd(deviceId, new DeviceTwinFlatModel());
            }

            deviceEntity.ProductFamily = tags.ProductFamily;
            deviceEntity.ProductName = tags.ProductName;
            deviceEntity.RetailerName = tags.RetailerName;
            deviceEntity.RetailerRegion = tags.RetailerRegion;
        }

        public void RemoveDeviceEntity(string deviceId)
        {
            DeviceTwinFlatModel deviceEntity = null;
            bool result = DevicesDB.Devices.TryGetValue(deviceId, out deviceEntity);
            if (!result)
                return;

            result = DevicesDB.Devices.TryRemove(deviceId, out deviceEntity);
            if (!result)
                Log.Error(string.Format("Error removing device {0} from the cache", deviceId));
        }
    }
}
