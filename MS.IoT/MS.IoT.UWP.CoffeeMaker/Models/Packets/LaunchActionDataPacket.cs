using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class LaunchActionDataPacket : DataPacket
    {
        public string LaunchAction { get; private set; }
        public string ParametersJson { get; private set; }

        public LaunchActionDataPacket(string launchAction, string parametersJson) :
            base(FourByFourHelper.GetNewMessageId(), 0, FourByFourConstants.CMD_LAUNCH_ACTION, null)
        {
            LaunchAction = launchAction;
            ParametersJson = parametersJson;
            RawData = GetBytes();
        }

        public LaunchActionDataPacket(DataPacket packet) :
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {
            byte[] rawData = packet.RawData;
            ParametersJson = string.Empty;

            List<byte> tempBytes = new List<byte>();
            for (int i = 0; i < rawData.Length; i++)
            {
                if (rawData[i] != 0x00)
                    tempBytes.Add(rawData[i]);
                else if (string.IsNullOrEmpty(LaunchAction))
                {
                    LaunchAction = Encoding.ASCII.GetString(tempBytes.ToArray());
                    tempBytes.Clear();
                }
                else
                {
                    if (tempBytes.Count != 0)
                        ParametersJson = Encoding.ASCII.GetString(tempBytes.ToArray());
                    tempBytes.Clear();
                    break;
                }
            }
        }

        private byte[] GetBytes()
        {
            byte[] propertyNameBytes = Encoding.ASCII.GetBytes(LaunchAction);
            byte[] propertyValueBytes = Encoding.ASCII.GetBytes(ParametersJson);
            int originalNameLength = propertyNameBytes.Length;

            Array.Resize<byte>(ref propertyNameBytes, propertyNameBytes.Length + propertyValueBytes.Length + 2);
            Array.Copy(propertyValueBytes, 0, propertyNameBytes, originalNameLength + 1, propertyValueBytes.Length);

            return propertyNameBytes;
        }
    }
}
