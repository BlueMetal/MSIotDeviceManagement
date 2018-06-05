using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;

namespace MS.IoT.UWP.CoffeeMaker.Models.Packets
{
    public class DataPacket
    {
        public ushort MessageId { get; private set; }
        public int DataSize { get; private set; }
        public string Command { get; private set; }
        public byte[] RawData { get; protected set; }

        public DataSource Source
        {
            get
            {
                if (MessageId <= short.MaxValue)
                    return DataSource.FourByFour;
                return DataSource.Other;
            }
        }

        public DataPacket(ushort messageId, int dataSize, string command, byte[] rawData)
        {
            MessageId = messageId;
            DataSize = dataSize;
            Command = command;
            RawData = rawData;
        }

        public DataPacket ChangeChild<T>() where T : DataPacket
        {
            if (typeof(T) == typeof(PingDataPacket))
                return new PingDataPacket(this);
            else if (typeof(T) == typeof(PongDataPacket))
                return new PongDataPacket(this);
            else if (typeof(T) == typeof(GetPropertyDataPacket))
                return new GetPropertyDataPacket(this);
            else if (typeof(T) == typeof(SendPropertyDataPacket))
                return new SendPropertyDataPacket(this);
            else if (typeof(T) == typeof(ConfirmVariableDataPacket))
                return new ConfirmVariableDataPacket(this);
            else if (typeof(T) == typeof(LaunchActionDataPacket))
                return new LaunchActionDataPacket(this);
            else if (typeof(T) == typeof(ConfirmActionDataPacket))
                return new ConfirmActionDataPacket(this);
            else
                throw new NotImplementedException("Unhandled type");
        }
    }
}
