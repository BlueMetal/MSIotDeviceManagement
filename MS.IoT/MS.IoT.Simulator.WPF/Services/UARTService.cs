using MS.IoT.Common;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models.Packets;
using System;
using System.IO.Ports;
using System.Text;

namespace MS.IoT.Simulator.WPF.Services
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

        //SerialPort Variables
        private SerialPort _SerialPort;

        //Packet Analysis
        private static ProcessingSteps currentStep;
        private static ushort currentCursor = 0;
        private static byte[] headerBuffer = new byte[6];
        private static ushort currentPacketSize = 0;
        private static byte[] currentPacketData;
        private static string currentPacketCommand = string.Empty;



        /// <summary>
        /// Main Constructor
        /// </summary>
        public UARTService()
        {
        }



        /// <summary>
        /// Initialize a new SerialPort object with a specific deviceId and baud value
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="baud">Baud value</param>
        public void InitSerialPort(string deviceId, int baud)
        {
            ReleaseResources();
            _SerialPort = new SerialPort(deviceId, baud);
            _SerialPort.Open();
            _SerialPort.DataReceived += Event_UARTDataProcessing;
        }

        /// <summary>
        /// Free the SerialPort object memory, for example when swapping to another device 
        /// </summary>
        public void ReleaseResources()
        {
            if (_SerialPort != null)
            {
                _SerialPort.DataReceived -= Event_UARTDataProcessing;
                _SerialPort.Close();
                _SerialPort.Dispose();
                _SerialPort = null;
            }
        }

        /// <summary>
        /// Send a packet to the 4x4 Chip
        /// </summary>
        /// <param name="packet">Packet</param>
        /// <returns></returns>
        public bool SendPacket(FourByFourCommand packet)
        {
            try
            {
                if (_SerialPort == null)
                    return false;

                byte[] packetBinaries = packet.GetDataPacket();
                _SerialPort.Write(packetBinaries, 0, packetBinaries.Length);
                Console.WriteLine(string.Format("INFO: Command '{0}' sent to UART. Data bytes: '{1}'", packet.Command, packetBinaries.Length));

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error when sending FourByFourCommand packet");
                return false;
            }
        }

        /// <summary>
        /// Regenerates a new packet based on the type of command after successfull processing.
        /// </summary>
        private void GenerateNewPacket()
        {
            if (currentPacketSize > 0)
                Console.WriteLine(string.Format("INFO: Data received: '{0}'", BitConverter.ToString(currentPacketData)));

            switch (currentPacketCommand)
            {
                case FourByFourConstants.CMD_POKE_ALIVE:
                    NewResponsePacketReceived(new StatusReception(currentPacketCommand, ReceptionState.Success));
                    break;
                case FourByFourConstants.CMD_GET_CONFIG:
                    NewResponsePacketReceived(new GetConfigReception(currentPacketCommand, currentPacketSize, currentPacketData));
                    break;
                case FourByFourConstants.CMD_SET_CONFIG:
                case FourByFourConstants.CMD_POKE_IOT:
                    NewResponsePacketReceived(new BooleanReception(currentPacketCommand, currentPacketSize, currentPacketData));
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
        private void Event_UARTDataProcessing(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = null;

            if (!_SerialPort.IsOpen) return;

            try
            {
                int bytes = _SerialPort.BytesToRead;
                buffer = new byte[bytes];
                _SerialPort.Read(buffer, 0, bytes);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error while reading SerialPort data.");
                if(buffer != null)
                    Log.Debug(string.Format("Hex: {0}", BitConverter.ToString(buffer)));
                return;
            }

            foreach (byte b in buffer)
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

                        if (currentCursor == FourByFourConstants.HEADER_DATASIZE_SIZE + FourByFourConstants.HEADER_COMMAND_SIZE)
                        {
                            currentCursor = 0;

                            //Get size and command
                            currentPacketSize = BitConverter.ToUInt16(headerBuffer, 0);
                            currentPacketCommand = Encoding.ASCII.GetString(headerBuffer, 2, FourByFourConstants.HEADER_COMMAND_SIZE);

                            if (Array.Exists(FourByFourConstants.HEADER_VALID_COMMANDS, p => p == currentPacketCommand))
                            {
                                //Command valid, proceed to the parsing data (if needed)
                                Console.WriteLine(string.Format("INFO: Command received: '{0}', data size: '{1}' bytes", currentPacketCommand, currentPacketSize));
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
        private void NewResponsePacketReceived(FourByFourReception responsePacked)
        {
            UARTPacketReceived?.Invoke(this, new UARTPacketEventArgs(responsePacked));
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
