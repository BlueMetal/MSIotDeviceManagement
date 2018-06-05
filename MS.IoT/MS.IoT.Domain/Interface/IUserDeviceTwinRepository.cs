using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.Domain.Interface
{
    public interface IUserDeviceTwinRepository
    {
        Task<DeviceTwinModel> GetUserDeviceTwinAsync(string userId,string deviceId);

        Task<List<DeviceTwinModel>> GetUserDevicesTwinAsync(string userId);

        Task<DirectMethodResponse> UpdateDeviceFeatureDirectMethod(string deviceId, DirectMethodBase method);
    } 
}
