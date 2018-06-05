using System.Collections.Generic;
using System.Threading.Tasks;
using static MS.IoT.UWP.CoffeeMaker.Services.FourByFourService;

namespace MS.IoT.UWP.CoffeeMaker.Services.Interfaces
{
    /// <summary>
    /// Interface for FourByFourService
    /// </summary>
    /// <summary>
    public interface IFourByFourService
    {
        bool IsDeviceReady { get; }
        event VariableChangedEventHandler VariableChanged;
        event SerialDeviceReadyEventHandler SerialDeviceReady;
        Task<bool> CommandSendPing();
        Task<string> CommandGetVariable(string propertyName);
        Task<bool> CommandLaunchAction(string actionName);
        Task<bool> CommandLaunchAction(string actionName, string parametersJson);
        void Init();
        void Dispose();
    }
}
