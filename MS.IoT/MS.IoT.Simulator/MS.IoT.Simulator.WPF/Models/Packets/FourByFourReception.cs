namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public enum ReceptionState
    {
        Unknown = 0,
        Success = 1,
        Error = 2,
        Timeout = 3
    }

    public abstract class FourByFourReception
    {
        public string Command { get; private set; }
        protected byte[] RawData { get; private set; }
        public int DataSize { get; private set; }
        public ReceptionState State { get; protected set; }

        public FourByFourReception(string command, int dataSize, byte[] rawData)
        {
            Command = command;
            DataSize = dataSize;
            RawData = rawData;

            if (dataSize == 1 && rawData[0] == 0x30)
                State = ReceptionState.Error;
            else if (dataSize == 1 && rawData[0] == 0x31)
                State = ReceptionState.Success;
        }
    }
}
