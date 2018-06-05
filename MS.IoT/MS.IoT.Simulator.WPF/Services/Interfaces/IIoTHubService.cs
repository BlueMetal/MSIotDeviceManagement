using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MS.IoT.Simulator.WPF.Services.ActiveDirectoryUserService;

namespace MS.IoT.Simulator.WPF.Services.Interfaces
{
    /// <summary>
    /// Interface for IoTHubService
    /// </summary>
    public interface IIoTHubService
    {
        Task<string> RegisterDeviceInIoTHubAsync(string deviceId);
        Task<IEnumerable> GetDevices();
    }
}
