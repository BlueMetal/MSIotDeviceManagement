using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class ConfirmVariableDataPacket : DataPacket
    {
        public string PropertyName { get; private set; }

        public ConfirmVariableDataPacket(ushort sourceMessageId, string propertyName) :
             base(sourceMessageId, 0, FourByFourConstants.CMD_CONFIRM_PROPERTY, null)
        {
            PropertyName = propertyName;
            RawData = GetBytes();
        }
 

        public ConfirmVariableDataPacket(DataPacket packet) :
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {

        }

        private byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(PropertyName);
        }
    }
}
