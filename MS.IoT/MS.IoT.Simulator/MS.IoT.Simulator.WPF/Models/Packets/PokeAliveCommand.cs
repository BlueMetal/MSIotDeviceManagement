using MS.IoT.Simulator.WPF.Helpers;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class PokeAliveCommand : FourByFourCommand
    {
        public PokeAliveCommand()
        {

        }

        public override string Command { get { return FourByFourConstants.CMD_POKE_ALIVE; } }
    }
}
