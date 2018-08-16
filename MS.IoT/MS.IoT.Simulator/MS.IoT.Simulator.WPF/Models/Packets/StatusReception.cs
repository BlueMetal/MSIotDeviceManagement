namespace MS.IoT.Simulator.WPF.Models.Packets
{
    public class StatusReception : FourByFourReception
    {
        public StatusReception(string command, ReceptionState state)
            : base(command, 0, null)
        {
            State = state;
        }
    }
}
