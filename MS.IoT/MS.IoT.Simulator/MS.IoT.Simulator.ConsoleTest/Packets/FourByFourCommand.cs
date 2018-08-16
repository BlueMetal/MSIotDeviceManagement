using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.ConsoleTest.Packets
{
    public abstract class FourByFourCommand
    {
        protected abstract string GetCommandName();
        protected virtual byte[] GetData() { return new byte[0]; }

        private byte[] BuildPacket()
        {
            byte[] data = GetData();
            if (data.Length + FourByFourConstants.HEADER_SIZE > ushort.MaxValue)
                throw new Exception(string.Format("Data packet size exceed maximum value: {0}", ushort.MaxValue));

            byte[] packet = new byte[FourByFourConstants.HEADER_SIZE + data.Length];
            byte[] headerTag = FourByFourConstants.HEADER_TAG;
            byte[] headerCommand = Encoding.ASCII.GetBytes(GetCommandName());
            Array.Copy(headerTag, 0x00, packet, 0x00, headerTag.Length);
            Array.Copy(BitConverter.GetBytes(data.Length), 0x00, packet, FourByFourConstants.HEADER_TAG_SIZE, FourByFourConstants.HEADER_DATASIZE_SIZE);
            Array.Copy(headerCommand, 0x00, packet, FourByFourConstants.HEADER_TAG_SIZE + FourByFourConstants.HEADER_DATASIZE_SIZE, FourByFourConstants.HEADER_COMMAND_SIZE);
            
            if(data.Length > 0)
                Array.Copy(data, 0, packet, FourByFourConstants.HEADER_SIZE, data.Length);

            return packet;
        }

        public byte[] GetDataPacket()
        {
            return BuildPacket();
        }
    }
}
