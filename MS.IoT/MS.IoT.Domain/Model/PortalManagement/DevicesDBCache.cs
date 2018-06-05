using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DevicesDBCache
    {
        public ConcurrentDictionary<string, DeviceTwinFlatModel> Devices { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Count { get { return Devices == null ? 0 : Devices.Count; } }
        public bool IsDevicesDBCacheInitialized { get; set; }
        public bool IsDevicesDBCacheLoading { get; set; }
        public bool IsDevicesDBCacheLocationUpdating { get; set; }

        public DevicesDBCache()
        {
            LastUpdate = DateTime.MinValue;
        }
    }
}
