using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.ConsoleTest.Packets
{
    public class PokeAliveCommand : FourByFourCommand
    {
        public PokeAliveCommand()
        {

        }

        protected override string GetCommandName()
        {
            return FourByFourConstants.CMD_POKE_ALIVE;
        }
    }
}
