using MS.IoT.Simulator.WPF.Helpers;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class GetConfigCommand : FourByFourCommand
    {
        public GetConfigCommand()
        {
        }
        public override string Command
        {
            get { return FourByFourConstants.CMD_GET_CONFIG; }
        }
    }
}
