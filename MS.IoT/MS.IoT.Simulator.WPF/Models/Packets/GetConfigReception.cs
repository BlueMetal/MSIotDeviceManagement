using System.Text;

namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class GetConfigReception : FourByFourReception
    {
        public string ConnectionString { get; private set; }

        public GetConfigReception(string command, int dataSize, byte[] rawData)
            : base(command, dataSize, rawData)
        {
            if (dataSize > 0)
                ConnectionString = Encoding.ASCII.GetString(rawData);
            else
                ConnectionString = string.Empty;
            State = ReceptionState.Success;
        }
    }
}
