using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.ConsoleTest.Packets
{
    public abstract class FourByFourResponse
    {
        public string Command { get; private set; }
        public byte[] RawData { get; private set; }
        public int DataSize { get; private set; }

        public FourByFourResponse(string command, int dataSize, byte[] rawData)
        {
            Command = command;
            DataSize = dataSize;
            RawData = rawData;
        }
    }
}
