using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MS.IoT.Simulator.WPF.Services.FourByFourService;

namespace MS.IoT.Simulator.WPF.Services.Interfaces
{
    /// <summary>
    /// Interface for FourByFourService
    /// </summary>
    public interface IFourByFourService
    {
        COMDeviceInfo SelectCOMDevice { get; }

        bool PokeNetworkAvailability();
        Task<bool> TestUARTConnection();
        Task<string> GetConnectionStringFromChip();
        Task<bool> SetConfigOnChip(string connectionString);
        Task<bool> TestIoTHubConnection();
        bool SendTemplateCommand(TemplatePacket template);
        void Init();
        void SetCOMDevice(string deviceId);
        List<COMDeviceInfo> GetCOMDevices();
    }
}
