using MS.IoT.UWP.CoffeeMaker.Helpers;
using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace MS.IoT.UWP.CoffeeMaker.Services
{
    /// <summary>
    /// UARTService
    /// Class that manages the communication (in and out) with the 4x4 chip through UART
    /// </summary>
    public class UARTService : IDisposable
    {
        //Events
        public delegate void UARTPacketEventHandler(Object sender, UARTPacketEventArgs e);
        public event UARTPacketEventHandler UARTPacketReceived;

        //Properties
        public bool IsDeviceReady { get {
                if (_SerialPort == null)
                    return false;
                return _SerialPort.SerialPortReady;
            } }

        //SerialPort Variables
        private SerialPortListener _SerialPort;

        //Packet Analysis
        private static ProcessingSteps currentStep;
        private static ushort currentCursor = 0;
        private static byte[] headerBuffer = new byte[FourByFourConstants.HEADER_DATA_SIZE];
        private static ushort currentPacketMessageId = 0;
        private static ushort currentPacketSize = 0;
        private static byte[] currentPacketData;
        private static string currentPacketCommand = string.Empty;


        public SerialPortListener SerialPort { get { return _SerialPort; } }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public UARTService()
        {
        }



        /// <summary>
        /// Initialize a new SerialPort object with a specific deviceId and baud value
        /// </summary>
        public void InitSerialPort()
        {
            _SerialPort = new SerialPortListener();
            _SerialPort.UARTDataReceived += Event_UARTDataProcessing;
            _SerialPort.InitSerialPort(FourByFourConstants.SERIAL_PORT_BAUDS);
        }

        /// <summary>
        /// Free the SerialPort object memory, for example when swapping to another device 
        /// </summary>
        public void ReleaseResources()
        {
            if (_SerialPort != null)
            {
                _SerialPort.UARTDataReceived -= Event_UARTDataProcessing;
                _SerialPort.CloseDevice();
                _SerialPort = null;
            }
        }

        /// <summary>
        /// Send a packet to the 4x4 Chip
        /// </summary>
        /// <param name="packet">Packet</param>
        /// <returns></returns>
        public async Task<bool> SendPacket(DataPacket packet)
        {
            try
            {
                if (_SerialPort == null)
                    return false;

                byte[] packetBinaries = BuildPacket(packet);
                await _SerialPort.WriteAsync(packetBinaries);
                //Console.WriteLine(string.Format("INFO: Command '{0}' sent to UART. Data bytes: '{1}'", packet.Command, packetBinaries.Length));

                return true;
            }
            catch (Exception e)
            {
                LogHelper.LogError(string.Format("Error while sending UART Packet: {0}", e.Message));
                return false;
            }
        }

        private byte[] BuildPacket(DataPacket dataPacket)
        {
            byte[] data = dataPacket.RawData;
            if (data == null)
                data = new byte[0];
            if (data.Length + FourByFourConstants.HEADER_SIZE > ushort.MaxValue)
                throw new Exception(string.Format("Data packet size exceed maximum value: {0}", ushort.MaxValue));

            byte[] packet = new byte[FourByFourConstants.HEADER_SIZE + data.Length];
            byte[] headerTag = FourByFourConstants.HEADER_TAG;
            byte[] headerCommand = Encoding.ASCII.GetBytes(dataPacket.Command);
            Array.Copy(headerTag, 0x00, packet, 0x00, headerTag.Length);
            Array.Copy(BitConverter.GetBytes(dataPacket.MessageId), 0x00, packet, FourByFourConstants.HEADER_TAG_SIZE, FourByFourConstants.HEADER_MESSAGE_ID_SIZE);
            Array.Copy(BitConverter.GetBytes(data.Length), 0x00, packet, FourByFourConstants.HEADER_TAG_SIZE + FourByFourConstants.HEADER_MESSAGE_ID_SIZE, FourByFourConstants.HEADER_DATASIZE_SIZE);
            Array.Copy(headerCommand, 0x00, packet, FourByFourConstants.HEADER_TAG_SIZE + FourByFourConstants.HEADER_MESSAGE_ID_SIZE + FourByFourConstants.HEADER_DATASIZE_SIZE, FourByFourConstants.HEADER_COMMAND_SIZE);

            if (data.Length > 0)
                Array.Copy(data, 0, packet, FourByFourConstants.HEADER_SIZE, data.Length);

            return packet;
        }

        /// <summary>
        /// Regenerates a new packet based on the type of command after successfull processing.
        /// </summary>
        private void GenerateNewPacket()
        {
            //if (currentPacketSize > 0)
            //    Debug.WriteLine(string.Format("INFO: Data received: '{0}'", BitConverter.ToString(currentPacketData)));

            DataPacket newPacket = new DataPacket(currentPacketMessageId, currentPacketSize, currentPacketCommand, currentPacketData);

            switch (currentPacketCommand)
            {
                case FourByFourConstants.CMD_PING:
                    NewResponsePacketReceived(newPacket.ChangeChild<PingDataPacket>());
                    break;
                case FourByFourConstants.CMD_PONG:
                    NewResponsePacketReceived(newPacket.ChangeChild<PongDataPacket>());
                    break;
                case FourByFourConstants.CMD_GET_PROPERTY:
                    NewResponsePacketReceived(newPacket.ChangeChild<GetPropertyDataPacket>());
                    break;
                case FourByFourConstants.CMD_SEND_PROPERTY:
                    NewResponsePacketReceived(newPacket.ChangeChild<SendPropertyDataPacket>());
                    break;
                case FourByFourConstants.CMD_CONFIRM_PROPERTY:
                    NewResponsePacketReceived(newPacket.ChangeChild<ConfirmVariableDataPacket>());
                    break;
                case FourByFourConstants.CMD_LAUNCH_ACTION:
                    NewResponsePacketReceived(newPacket.ChangeChild<LaunchActionDataPacket>());
                    break;
                case FourByFourConstants.CMD_CONFIRM_ACTION:
                    NewResponsePacketReceived(newPacket.ChangeChild<ConfirmActionDataPacket>());
                    break;
            }

            //Reset processing
            currentPacketCommand = string.Empty;
            currentPacketSize = 0;
            currentPacketData = null;
            currentCursor = 0;
            currentStep = ProcessingSteps.TAG;
        }

        /// <summary>
        /// Callback indicating the reception of data from the UART device
        /// This method will analyze the data and process the tag, header and data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_UARTDataProcessing(object sender, SerialPortDataReceivedArgs e)
        {
            if (_SerialPort == null || e.Data == null || e.Data.Length == 0) return;

            foreach (byte b in e.Data)
            {
                switch (currentStep)
                {
                    case ProcessingSteps.DATA:
                        //Header processing, command correct, processing data
                        currentPacketData[currentCursor] = b;
                        currentCursor++;

                        if (currentCursor == currentPacketSize)
                            GenerateNewPacket();
                        break;
                    case ProcessingSteps.HEADER:
                        //Tag found, processing header
                        headerBuffer[currentCursor] = b;
                        currentCursor++;

                        if (currentCursor == FourByFourConstants.HEADER_DATA_SIZE)
                        {
                            currentCursor = 0;

                            //Get size and command
                            currentPacketMessageId = BitConverter.ToUInt16(headerBuffer, 0);
                            currentPacketSize = BitConverter.ToUInt16(headerBuffer, FourByFourConstants.HEADER_MESSAGE_ID_SIZE);
                            currentPacketCommand = Encoding.ASCII.GetString(headerBuffer, FourByFourConstants.HEADER_MESSAGE_ID_SIZE + FourByFourConstants.HEADER_DATASIZE_SIZE, FourByFourConstants.HEADER_COMMAND_SIZE);

                            if (Array.Exists(FourByFourConstants.HEADER_VALID_COMMANDS, p => p == currentPacketCommand))
                            {
                                //Command valid, proceed to the parsing data (if needed)
                                //Console.WriteLine(string.Format("INFO: Command received: '{0}', data size: '{1}' bytes", currentPacketCommand, currentPacketSize));
                                if (currentPacketSize > 0)
                                {
                                    currentPacketData = new byte[currentPacketSize];
                                    currentStep = ProcessingSteps.DATA;
                                }
                                else
                                {
                                    //No data, sending the packet immediately, skipping DATA phase
                                    GenerateNewPacket();
                                }
                            }
                            else
                            {
                                //Command is not recognized, go back to tag seeking
                                currentStep = ProcessingSteps.TAG;
                            }
                        }
                        break;
                    case ProcessingSteps.TAG:
                        if (b == FourByFourConstants.HEADER_TAG[currentCursor])
                        {
                            currentCursor++;
                            if (currentCursor == FourByFourConstants.HEADER_TAG.Length)
                            {
                                //Pattern found, saving the rest of the packet as it comes
                                currentStep = ProcessingSteps.HEADER;
                                currentCursor = 0;
                            }
                        }
                        else
                            currentCursor = 0;
                        break;
                }
            }
        }



        /// <summary>
        /// Fires a new UARTPacketReceived event once a packet was successfully processed and recognized
        /// </summary>
        /// <param name="responsePacked">FourByFourReception packet</param>
        private void NewResponsePacketReceived(DataPacket responsePacket)
        {
            UARTPacketReceived?.Invoke(this, new UARTPacketEventArgs(responsePacket));
        }

        /// <summary>
        /// Method to be used when the application exits
        /// </summary>
        public void Dispose()
        {
            this.ReleaseResources();
        }
    }
}
