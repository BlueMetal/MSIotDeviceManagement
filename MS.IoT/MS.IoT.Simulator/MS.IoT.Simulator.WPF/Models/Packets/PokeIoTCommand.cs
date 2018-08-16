using MS.IoT.Simulator.WPF.Helpers;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class PokeIoTCommand : FourByFourCommand
    {
        public PokeIoTCommand()
        {

        }

        public override string Command { get { return FourByFourConstants.CMD_POKE_IOT; } }
    }
}
