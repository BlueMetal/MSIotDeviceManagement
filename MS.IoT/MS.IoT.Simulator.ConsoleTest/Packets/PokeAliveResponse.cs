using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.ConsoleTest.Packets
{
    public class PokeAliveResponse : FourByFourResponse
    {
        public bool IsAlive
        {
            get;
            private set;
        }

        public PokeAliveResponse(string command, int dataSize, byte[] rawData)
            : base(command, dataSize, rawData)
        {
            if (rawData.Length == 1)
                if (rawData[0] == 0x31)
                    IsAlive = true;
        }
    }
}
