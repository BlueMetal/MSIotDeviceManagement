namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class BooleanReception : FourByFourReception
    {
        public BooleanReception(string command, int dataSize, byte[] rawData)
            : base(command, dataSize, rawData)
        {
        }
    }
}
