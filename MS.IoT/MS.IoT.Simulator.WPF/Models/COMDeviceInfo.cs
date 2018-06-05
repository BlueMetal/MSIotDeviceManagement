namespace MS.IoT.Simulator.WPF.Models
{
    /// <summary>
    /// COMDeviceInfo
    /// Contains basic properties for COM Devices
    /// </summary>
    public class COMDeviceInfo
    {
        public COMDeviceInfo(string deviceID, string pnpDeviceID, string description, string name)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.Name = name;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
    }
}
