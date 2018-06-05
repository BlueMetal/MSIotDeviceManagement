using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class PingDataPacket : DataPacket
    {
        public PingDataPacket() :
            base(FourByFourHelper.GetNewMessageId(), 0, FourByFourConstants.CMD_PING, null)
        {

        }

        public PingDataPacket(DataPacket packet) : 
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {

        }
    }
}
