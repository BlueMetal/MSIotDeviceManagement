using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using MS.IoT.Simulator.ConsoleTest.Packets;

namespace MS.IoT.Simulator.ConsoleTest
{
    class Program
    {
        private static string deviceId;
        private static int baudsValue = 115200;
        private static SerialPort serialPort;

        private static ProcessingSteps currentStep;
        private static ushort currentCursor = 0;
        private static byte[] headerBuffer = new byte[6];
        private static ushort currentPacketSize = 0;
        private static byte[] currentPacketData;
        private static string currentPacketCommand = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                var usbDevices = GetUSBDevices();

                foreach (var usbDevice in usbDevices)
                {
                    Console.WriteLine("Device ID: {0}, PNP Device ID: {1}, Description: {2}, Name: {3}",
                        usbDevice.DeviceID, usbDevice.PnpDeviceID, usbDevice.Description, usbDevice.Name);
                }

                if (usbDevices.Count == 0)
                {
                    Console.WriteLine("No serial USB device detected.");
                    return;
                }
                deviceId = usbDevices[0].DeviceID;

                serialPort = new SerialPort(deviceId, baudsValue);
                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;

                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Elapsed += ATimer_Elapsed;
                aTimer.Interval = 50;
                aTimer.Enabled = true;
                

                Console.Read();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                serialPort.Close();
            }
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!serialPort.IsOpen) return;

            int bytes = serialPort.BytesToRead;
            byte[] buffer = new byte[bytes];
            serialPort.Read(buffer, 0, bytes);

            Console.WriteLine(string.Format("Hex: {0}", BitConverter.ToString(buffer)));

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

        private static void GenerateNewPacket()
        {
            if (currentPacketSize > 0)
                Console.WriteLine(string.Format("INFO: Data received: '{0}'", BitConverter.ToString(currentPacketData)));

            //Configuration of the new packet
            FourByFourResponse newResponse;

            switch (currentPacketCommand)
            {
                case FourByFourConstants.CMD_POKE_ALIVE:
                    newResponse = new PokeAliveResponse(currentPacketCommand, currentPacketSize, currentPacketData);
                    break;
            }

            //Reset processing
            currentPacketCommand = string.Empty;
            currentPacketSize = 0;
            currentPacketData = null;
            currentCursor = 0;
            currentStep = ProcessingSteps.TAG;
        }

        private static void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendPacket(new PokeAliveCommand());
        }

        static void SendPacket(FourByFourCommand packet)
        {
            byte[] packetBinaries = packet.GetDataPacket(); 
            serialPort.Write(packetBinaries, 0, packetBinaries.Length);
        }

        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
                collection = searcher.Get();

            foreach (var device in collection)
            {

                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Name")
                ));
            }

            collection.Dispose();
            return devices;
        }
    }
}
