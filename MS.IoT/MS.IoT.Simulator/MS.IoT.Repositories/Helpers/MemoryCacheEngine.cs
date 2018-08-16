using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System.Runtime.Caching;

namespace MS.IoT.Repositories.Helpers
{
    public class MemoryCacheEngine : ICacheEngine
    {
        private const string CACHE_DEVICES_DB = "DevicePortal_DevicesDB";
        private MemoryCache _cache = MemoryCache.Default;
        private object _sync = new object();

        public DevicesDBCache DevicesDB
        {
            get
            {
                if (_cache[CACHE_DEVICES_DB] as DevicesDBCache == null)
                {
                    lock (_sync)
                    {
                        if (_cache[CACHE_DEVICES_DB] as DevicesDBCache == null)
                        {
                            _cache[CACHE_DEVICES_DB] = new DevicesDBCache();
                            return DevicesDB;
                        }
                    }
                }
                return _cache[CACHE_DEVICES_DB] as DevicesDBCache;
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
