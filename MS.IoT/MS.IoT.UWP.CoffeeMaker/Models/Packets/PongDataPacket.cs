using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class PongDataPacket : DataPacket
    {
        public PongDataPacket(ushort sourceMessageId) :
            base(sourceMessageId, 0, FourByFourConstants.CMD_PONG, null)
        {

        }

        public PongDataPacket(DataPacket packet) : 
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {

        }
    }
}
