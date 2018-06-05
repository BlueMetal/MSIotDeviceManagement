using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class ConfirmActionDataPacket : DataPacket
    {
        public string LaunchAction { get; private set; }

        public ConfirmActionDataPacket(ushort sourceMessageId, string launchAction) :
             base(sourceMessageId, 0, FourByFourConstants.CMD_CONFIRM_ACTION, null)
        {
            LaunchAction = launchAction;
            RawData = GetBytes();
        }
 

        public ConfirmActionDataPacket(DataPacket packet) :
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {

        }

        private byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(LaunchAction);
        }
    }
}
