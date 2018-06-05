using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using System;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    /// <summary>
    /// SerialPortDataReceivedArgs
    /// Class used while firing a UART event
    /// </summary>
    public class SerialPortDataReceivedArgs : EventArgs
    {
        /// <summary>
        /// Packet received through UART and its data
        /// </summary>
        public byte[] Data { get; private set; }


        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="packet"></param>
        public SerialPortDataReceivedArgs(byte[] data)
        {
            Data = data;
        }
    }
}
