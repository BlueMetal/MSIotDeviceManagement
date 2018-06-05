using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.ConsoleTest.Packets
{
    public enum ProcessingSteps { TAG, HEADER, DATA }

    public class FourByFourConstants
    {
        public const int HEADER_TAG_SIZE = 0x04;
        public const int HEADER_DATASIZE_SIZE = 0x02;
        public const int HEADER_COMMAND_SIZE = 0x04;
        public const int HEADER_SIZE = HEADER_TAG_SIZE + HEADER_DATASIZE_SIZE + HEADER_COMMAND_SIZE;
        public static byte[] HEADER_TAG = { 0x46, 0x42, 0x46, 0x00 };

        public const string CMD_POKE_ALIVE = "POKE";
        public const string CMD_SEND_CONFIG = "CFGK";
        public const string CMD_POKE_IOT = "CIOT";
        public const string CMD_SEND_TEMPLATE = "TPLT";
        public static string[] HEADER_VALID_COMMANDS = { CMD_POKE_ALIVE, CMD_SEND_CONFIG, CMD_POKE_IOT, CMD_SEND_TEMPLATE };
    }
}
