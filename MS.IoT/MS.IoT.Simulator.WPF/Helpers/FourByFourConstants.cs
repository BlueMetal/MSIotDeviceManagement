namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// Enum used for UART packet processing
    /// </summary>
    public enum ProcessingSteps { TAG, HEADER, DATA }

    /// <summary>
    /// Constants used for UART communications
    /// </summary>
    public class FourByFourConstants
    {
        public const int SERIAL_PORT_BAUDS = 115200;

        public const int HEADER_TAG_SIZE = 0x04;
        public const int HEADER_DATASIZE_SIZE = 0x02;
        public const int HEADER_COMMAND_SIZE = 0x04;
        public const int HEADER_SIZE = HEADER_TAG_SIZE + HEADER_DATASIZE_SIZE + HEADER_COMMAND_SIZE;
        public static byte[] HEADER_TAG = { 0x46, 0x42, 0x46, 0x00 };

        public const string CMD_POKE_ALIVE = "POKE";
        public const string CMD_GET_CONFIG = "CFGG";
        public const string CMD_SET_CONFIG = "CFGS";
        public const string CMD_POKE_IOT = "CIOT";
        public const string CMD_SEND_TEMPLATE = "TPLT";
        public static string[] HEADER_VALID_COMMANDS = { CMD_POKE_ALIVE, CMD_GET_CONFIG, CMD_SET_CONFIG, CMD_POKE_IOT, CMD_SEND_TEMPLATE };
    }
}
