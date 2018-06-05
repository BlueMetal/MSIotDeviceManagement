using MS.IoT.UWP.CoffeeMaker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    public class SerialPortListener
    {
        //Events
        public delegate void UARTDataReceivedHandler(Object sender, SerialPortDataReceivedArgs e);
        public event UARTDataReceivedHandler UARTDataReceived;
        public delegate void SerialDeviceReadyEventHandler(Object sender, EventArgs e);
        public event SerialDeviceReadyEventHandler SerialDeviceReady;

        //SerialPort Variables
        private bool _SerialPortReady = false;
        private SerialDevice _SerialPort;
        DataWriter _DataWriteObject = null;
        DataReader _DataReaderObject = null;
        private CancellationTokenSource _ReadCancellationTokenSource;


        public bool SerialPortReady { get { return _SerialPortReady; } }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public SerialPortListener()
        {
        }


        /// <summary>
        /// Initialize a new SerialPort object with a specific baud value
        /// </summary>
        /// <param name="baud">Baud value</param>
        public async void InitSerialPort(int baud)
        {
            _SerialPortReady = false;
            string aqs = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection dis = await DeviceInformation.FindAllAsync(aqs);
            if (dis.Count == 0)
                return;

            try
            {
                _SerialPort = await SerialDevice.FromIdAsync(GuessDeviceId(dis));

                if (_SerialPort == null) return;

                _SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(100);
                _SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(100);
                _SerialPort.BaudRate = (uint)baud;
                _SerialPort.Parity = SerialParity.None;
                _SerialPort.StopBits = SerialStopBitCount.One;
                _SerialPort.DataBits = 8;
                _SerialPort.Handshake = SerialHandshake.None;

                // Create cancellation token object to close I/O operations when closing the device
                _ReadCancellationTokenSource = new CancellationTokenSource();

                _SerialPortReady = true;
                SerialDeviceReady?.Invoke(this, new EventArgs());
                Listen();
            }
            catch (Exception e)
            {
                LogHelper.LogError(e.Message);
            }
        }

        public async Task WriteAsync(byte[] data)
        {
            if (_SerialPort == null)
                return;

            if(_DataWriteObject == null)
                _DataWriteObject = new DataWriter(_SerialPort.OutputStream);

            Task<UInt32> storeAsyncTask;
            _DataWriteObject.WriteBytes(data);

            // Launch an async task to complete the write operation
            storeAsyncTask =  _DataWriteObject.StoreAsync().AsTask();
            await storeAsyncTask;
        }

        private string GuessDeviceId(DeviceInformationCollection deviceCol)
        {
            foreach(DeviceInformation devInformation in deviceCol)
            {
                if (devInformation.Name.Contains("UART"))
                    return devInformation.Id;
            }

            if (deviceCol.Count > 0)
                return deviceCol[0].Id;

            return string.Empty;
        }

        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Listen()
        {
            try
            {
                if (_SerialPort != null)
                {
                    _DataReaderObject = new DataReader(_SerialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(_ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException tce)
            {
                LogHelper.LogError(string.Format("Reading task was cancelled, closing device and cleaning up: {0}", tce.Message));
                CloseDevice();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Format("UART Error: {0}", ex.Message));
            }
            finally
            {
                // Cleanup once complete
                if (_DataReaderObject != null)
                {
                    _DataReaderObject.DetachStream();
                    _DataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            _DataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = _DataReaderObject.LoadAsync(ReadBufferLength).AsTask(childCancellationTokenSource.Token);

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    byte[] bytesReceived = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                        bytesReceived[i] = _DataReaderObject.ReadByte();
                    UARTDataReceived?.Invoke(this, new SerialPortDataReceivedArgs(bytesReceived));
                }
            }
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (_ReadCancellationTokenSource != null)
            {
                if (!_ReadCancellationTokenSource.IsCancellationRequested)
                {
                    _ReadCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// </summary>
        public void CloseDevice()
        {
            if (_SerialPort != null)
            {
                _SerialPortReady = false;
                CancelReadTask();
                _SerialPort.Dispose();
                if (_DataReaderObject != null)
                    _DataReaderObject.Dispose();
                if (_DataWriteObject != null)
                    _DataWriteObject.Dispose();
            }
            _SerialPort = null;
        }
    }
}
