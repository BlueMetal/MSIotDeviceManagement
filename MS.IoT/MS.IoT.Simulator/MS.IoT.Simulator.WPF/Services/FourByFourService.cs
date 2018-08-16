using MS.IoT.Common;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Models.Packets;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Timers;
using static MS.IoT.Simulator.WPF.Services.UARTService;

namespace MS.IoT.Simulator.WPF.Services
{
    /// <summary>
    /// FourByFourService
    /// Class that handles general communication with the 4x4 Chip.
    /// Mainly, this chip serves as an entry point to the UART service and can detect wether the chip is connected to the SL0 network.
    /// </summary>
    public class FourByFourService : IFourByFourService, IDisposable
    {
        //Constants
        private static TimeSpan TIMEOUT = TimeSpan.FromMilliseconds(1500);
        private bool _Initialized = false;

        //Members
        private UARTService _UARTService;
        private COMDeviceInfo _SelectedCOMDevice = null;


        /// <summary>
        /// Property returning the current select COM Device
        /// </summary>
        public COMDeviceInfo SelectCOMDevice
        {
            get
            {
                return _SelectedCOMDevice;
            }
        }


        /// <summary>
        /// Main Constructor
        /// </summary>
        public FourByFourService()
        {
        }


        /// <summary>
        /// Initialize the service
        /// - Initiliaze the UART Service
        /// - Initiliaze the SerialPort object (if device found)
        /// </summary>
        public void Init()
        {
            if (_Initialized)
                return;

            //Init UART
            _UARTService = new UARTService();
            COMDeviceInfo comDevice = GuessCOMDevice();
            if (comDevice != null)
            {
                _SelectedCOMDevice = comDevice;
                InitUARTSerialPort();
            }

            _Initialized = true;
        }

        /// <summary>
        /// During the initialization phase, guess which COM device should be considered the one communicating to the Chip.
        /// If there is no device with the string "UART" in its name it will select the first COM device.
        /// </summary>
        /// <returns>COMDeviceInfo object</returns>
        private COMDeviceInfo GuessCOMDevice()
        {
            List<COMDeviceInfo> comDevices = GetCOMDevices();
            COMDeviceInfo defaultCOMDevice = comDevices.Find(p => p.Name.ToLower().Contains("uart"));
            if (defaultCOMDevice == null && comDevices.Count > 0)
                defaultCOMDevice = comDevices[0];
            return defaultCOMDevice;
        }

        /// <summary>
        /// Checks the status of the network interface SL0 to see if the Chip is connected to it
        /// and fires an event if the status changes.
        /// </summary>
        public bool PokeNetworkAvailability()
        {
            NetworkInterface netInterface = GetNetworkInterface();
            if (netInterface == null)
                return false;

            return netInterface.OperationalStatus == OperationalStatus.Up;
        }

        /// <summary>
        /// Get the network interface SL0
        /// </summary>
        /// <returns>NetworkInterface object</returns>
        private NetworkInterface GetNetworkInterface()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name != "sl0")
                    continue;

                return ni;
            }

            return null;
        }

        /// <summary>
        /// Calls the UART Service to initialize a new SerialPort object.
        /// </summary>
        private void InitUARTSerialPort()
        {
            if (_SelectedCOMDevice == null)
                return;
            _UARTService.InitSerialPort(_SelectedCOMDevice.DeviceID, FourByFourConstants.SERIAL_PORT_BAUDS); 
        }

        /// <summary>
        /// Set the Current COM Device
        /// </summary>
        /// <param name="deviceId">ID of the COM device to be used</param>
        public void SetCOMDevice(string deviceId)
        {
            COMDeviceInfo comDevice = GetCOMDevices().Find(p => p.DeviceID == deviceId);
            if(comDevice != null)
            {
                _SelectedCOMDevice = comDevice;
                InitUARTSerialPort();
            }
        }

        /// <summary>
        /// Send a UART command POKE that will ensures a connection exists between the simulator and the Chip.
        /// </summary>
        /// <returns>True if the connection was established.</returns>
        public async Task<bool> TestUARTConnection()
        {
            if (_UARTService == null)
                return false;

            TaskCompletionSource<FourByFourReception> signalEventPoke = new TaskCompletionSource<FourByFourReception>();

            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.Command == FourByFourConstants.CMD_POKE_ALIVE)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;


            if (_UARTService.SendPacket(new PokeAliveCommand()))
            {
                FourByFourReception reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT);
                _UARTService.UARTPacketReceived -= handler;

                return reception.State == ReceptionState.Success;
            }
            else
            {
                Log.Error("PokeAliveCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return false; 
        }

        /// <summary>
        /// Send UART command CIOT that will check whether the chip can access IoT Hub. It will use the connection string set by CFGS command
        /// </summary>
        /// <returns>True if the connection was established.</returns>
        public async Task<bool> TestIoTHubConnection()
        {
            if (_UARTService == null)
                return false;

            TaskCompletionSource<FourByFourReception> signalEventPoke = new TaskCompletionSource<FourByFourReception>();

            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.Command == FourByFourConstants.CMD_POKE_IOT)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;


            if (_UARTService.SendPacket(new PokeIoTCommand()))
            {
                FourByFourReception reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT);
                _UARTService.UARTPacketReceived -= handler;

                return reception.State == ReceptionState.Success;
            }
            else
            {
                Log.Error("PokeIoTCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return false;
        }

        /// <summary>
        /// Command CFGG to retrieve the current connection string saved in the chip.
        /// </summary>
        /// <returns>Connection String</returns>
        public async Task<string> GetConnectionStringFromChip()
        {
            if (_UARTService == null)
                return string.Empty;

            TaskCompletionSource<FourByFourReception> signalEventPoke = new TaskCompletionSource<FourByFourReception>();

            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.Command == FourByFourConstants.CMD_GET_CONFIG)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;


            if (_UARTService.SendPacket(new GetConfigCommand()))
            {
                FourByFourReception reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT);
                _UARTService.UARTPacketReceived -= handler;

                return ((GetConfigReception)reception).ConnectionString;
            }
            else
            {
                Log.Error("GetConfigCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return string.Empty;
        }

        /// <summary>
        /// Command CFGS to set the connection string in the chip appconfig store.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>True if the connection string was successfully set.</returns>
        public async Task<bool> SetConfigOnChip(string connectionString)
        {
            if (_UARTService == null)
                return false;

            TaskCompletionSource<FourByFourReception> signalEventPoke = new TaskCompletionSource<FourByFourReception>();

            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.Command == FourByFourConstants.CMD_SET_CONFIG)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;


            if (_UARTService.SendPacket(new SetConfigCommand(connectionString)))
            {
                FourByFourReception reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT);
                _UARTService.UARTPacketReceived -= handler;

                return reception.State == ReceptionState.Success;
            }
            else
            {
                Log.Error("SetConfigCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return false;
        }

        /// <summary>
        /// Command TLPT that sends a template with randomized data to the 4x4 chip, that will forward it to IoT Hub.
        /// The operation does not wait for an answer from the chip.
        /// </summary>
        /// <param name="templatePacket">TemplatePacket</param>
        /// <returns>true if the send operation succeeded</returns>
        public bool SendTemplateCommand(TemplatePacket templatePacket)
        {
            if (_UARTService == null)
                return false;

            return _UARTService.SendPacket(new SendTemplateCommand(templatePacket));
        }

        /// <summary>
        /// Get a list of the COM devices available on the computer.
        /// </summary>
        /// <returns>List of COM Devices availables</returns>
        public List<COMDeviceInfo> GetCOMDevices()
        {
            List<COMDeviceInfo> devices = new List<COMDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
                collection = searcher.Get();

            foreach (var device in collection)
            {

                devices.Add(new COMDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Name")
                ));
            }

            collection.Dispose();
            return devices;
        }

        /// <summary>
        /// Event that checks the status of SL0 network and send a UART POKE packet to the chip to test the connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_RefreshConnectionInformation(object sender, ElapsedEventArgs e)
        {
            PokeNetworkAvailability();
            TestUARTConnection().ConfigureAwait(false);
        }

        /// <summary>
        /// Method to be used when the application exits
        /// </summary>
        public void Dispose()
        {
            _UARTService.Dispose();
        }
    }
}
