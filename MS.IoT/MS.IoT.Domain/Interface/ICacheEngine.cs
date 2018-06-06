using MS.IoT.Domain.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface ICacheEngine
    {
        DevicesDBCache DevicesDB { get; }
        void EnsureDeviceEntity(string deviceId, DeviceTwinTagsModel tags);
        void EnsureDeviceEntity(string deviceId, DeviceTwinImportModel tags);
        void RemoveDeviceEntity(string deviceId);
    }
}
