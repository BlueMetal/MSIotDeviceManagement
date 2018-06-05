using MS.IoT.Simulator.WPF.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class SendTemplateCommand : FourByFourCommand
    {
        private TemplatePacket _TemplatePacket;

        public SendTemplateCommand(TemplatePacket templatePacket)
        {
            _TemplatePacket = templatePacket;
        }

        public override byte[] GetData()
        {
            string serialized = JsonConvert.SerializeObject(_TemplatePacket);
            return Encoding.ASCII.GetBytes(string.Format("[{0}]",serialized));
        }

        public override string Command
        {
            get { return FourByFourConstants.CMD_SEND_TEMPLATE; }
        }
    }
}
