using MS.IoT.Simulator.WPF.Models.Packets;
using System;

namespace MS.IoT.Simulator.WPF.Helpers
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
        public FourByFourReception ResponsePacket { get; private set; }


        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="packet"></param>
        public UARTPacketEventArgs(FourByFourReception packet)
        {
            ResponsePacket = packet;
        }
    }
}
