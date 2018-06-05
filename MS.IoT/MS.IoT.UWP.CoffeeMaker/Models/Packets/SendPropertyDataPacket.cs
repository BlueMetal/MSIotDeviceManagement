using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class SendPropertyDataPacket : DataPacket
    {
        public string PropertyName { get; private set; }
        public string PropertyValue { get; private set; }

        public SendPropertyDataPacket(string propertyName, string propertyValue) :
            base(FourByFourHelper.GetNewMessageId(), 0, FourByFourConstants.CMD_SEND_PROPERTY, null)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            RawData = GetBytes();
        }

        public SendPropertyDataPacket(ushort sourceMessageId, string propertyName, string propertyValue) :
             base(sourceMessageId, 0, FourByFourConstants.CMD_SEND_PROPERTY, null)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            RawData = GetBytes();
        }
 

        public SendPropertyDataPacket(DataPacket packet) :
            base(packet.MessageId, packet.DataSize, packet.Command, packet.RawData)
        {
            byte[] rawData = packet.RawData;

            List<byte> tempBytes = new List<byte>();
            for (int i = 0; i < rawData.Length; i++)
            {
                if (rawData[i] != 0x00)
                    tempBytes.Add(rawData[i]);
                else if (string.IsNullOrEmpty(PropertyName))
                {
                    PropertyName = Encoding.ASCII.GetString(tempBytes.ToArray());
                    tempBytes.Clear();
                }
                else if (string.IsNullOrEmpty(PropertyValue))
                {
                    if (tempBytes.Count == 0)
                        PropertyValue = string.Empty;
                    else
                        PropertyValue = Encoding.ASCII.GetString(tempBytes.ToArray());
                    tempBytes.Clear();
                    break;
                }
            }
        }

        private byte[] GetBytes()
        {
            byte[] propertyNameBytes = Encoding.ASCII.GetBytes(PropertyName);
            byte[] propertyValueBytes = Encoding.ASCII.GetBytes(PropertyValue);
            int originalNameLength = propertyNameBytes.Length;

            Array.Resize<byte>(ref propertyNameBytes, propertyNameBytes.Length + propertyValueBytes.Length + 2);
            Array.Copy(propertyValueBytes, 0, propertyNameBytes, originalNameLength + 1, propertyValueBytes.Length);

            return propertyNameBytes;
        }
    }
}
