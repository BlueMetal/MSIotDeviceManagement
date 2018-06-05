using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using System;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    /// <summary>
    /// UARTPacketEventArgs
    /// Class used while firing a UART event
    /// </summary>
    public class UARTPacketEventArgs : EventArgs
    {
        /// <summary>
        /// Packet received through UART and its data
        /// </summary>
        public DataPacket ResponsePacket { get; private set; }


        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="packet"></param>
        public UARTPacketEventArgs(DataPacket packet)
        {
            ResponsePacket = packet;
        }
    }
}
