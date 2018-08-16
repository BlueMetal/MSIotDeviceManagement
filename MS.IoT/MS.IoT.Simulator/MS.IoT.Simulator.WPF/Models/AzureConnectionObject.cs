using MS.IoT.Domain.Model;

namespace MS.IoT.Simulator.WPF.Models
{
    /// <summary>
    /// Enum for different connection status
    /// </summary>
    public enum AzureConnectionStatus
    {
        UnknownError = 0,
        NotConnected = 1,
        Authenticated = 2,
        AccessDenied = 3,
        UserInteractionRequired = 4
    }

    /// <summary>
    /// AzureConnectionObject
    /// Class used to give a connection status of a specific user
    /// </summary>
    public class AzureConnectionObject
    {
        public AzureConnectionStatus ConnectionStatus { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }
}
