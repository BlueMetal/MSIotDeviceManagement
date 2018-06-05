using MS.IoT.Simulator.WPF.Helpers;
using System.Text;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class SetConfigCommand : FourByFourCommand
    {
        private string _ConnectionString;

        public SetConfigCommand(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public override byte[] GetData()
        {
            return Encoding.ASCII.GetBytes(_ConnectionString);
        }

        public override string Command
        {
            get { return FourByFourConstants.CMD_SET_CONFIG; }
        }
    }
}
