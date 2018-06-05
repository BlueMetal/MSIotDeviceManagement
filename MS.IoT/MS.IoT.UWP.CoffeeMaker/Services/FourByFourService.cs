using MS.IoT.UWP.CoffeeMaker.Helpers;
using MS.IoT.UWP.CoffeeMaker.Models;
using MS.IoT.UWP.CoffeeMaker.Models.Packets;
using MS.IoT.UWP.CoffeeMaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static MS.IoT.UWP.CoffeeMaker.Services.UARTService;

namespace MS.IoT.UWP.CoffeeMaker.Services
{
    /// <summary>
    /// FourByFourService
    /// Class that handles general communication with the 4x4 Chip.
    /// Mainly, this chip serves as an entry point to the UART service.
    /// </summary>
    public class FourByFourService : IFourByFourService, IDisposable
    {
        //Events
        public delegate void VariableChangedEventHandler(Object sender, VariableModel e);
        public event VariableChangedEventHandler VariableChanged;
        public delegate void SerialDeviceReadyEventHandler(Object sender, EventArgs e);
        public event SerialDeviceReadyEventHandler SerialDeviceReady;

        //Constants
        private static TimeSpan TIMEOUT_UART_ANSWER = TimeSpan.FromMilliseconds(FourByFourConstants.UART_PACKET_TIMEOUT_MS);
        private bool _Initialized = false;

        //Members
        private UARTService _UARTService;

        //Properties
        public bool IsDeviceReady
        {
            get
            {
                if (_UARTService == null)
                    return false;
                return _UARTService.IsDeviceReady;
            }
        }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public FourByFourService()
        {
        }


        /// <summary>
        /// Initialize the service
        /// - Initiliaze the UART Service
        /// - Initiliaze the SerialPort object (if device found)
        /// </summary>
        public void Init()
        {
            if (_Initialized)
                return;

            //Init UART
            _UARTService = new UARTService();
            _UARTService.UARTPacketReceived += Event_UARTPacketReceived;
            _UARTService.InitSerialPort();
            _UARTService.SerialPort.SerialDeviceReady += Event_SerialDeviceReady;

            _Initialized = true;
        }

        private void Event_SerialDeviceReady(object sender, EventArgs e)
        {
            SerialDeviceReady?.Invoke(this, new EventArgs());
        }

        private async void Event_UARTPacketReceived(object sender, UARTPacketEventArgs e)
        {
            if (e.ResponsePacket.Source != DataSource.FourByFour)
                return;

            switch (e.ResponsePacket.Command)
            {
                //Automatically send a pong answer
                case FourByFourConstants.CMD_PING:
                    await CommandSendPong(e.ResponsePacket.MessageId);
                    break;
                //Automatically send a confirmation
                case FourByFourConstants.CMD_SEND_PROPERTY:
                    SendPropertyDataPacket sendValueDataPacket = e.ResponsePacket as SendPropertyDataPacket;
                    if (sendValueDataPacket != null) {
                        NewResponseVariableChangedReceived(sendValueDataPacket.PropertyName, sendValueDataPacket.PropertyValue);
                        await CommandConfirmVariable(e.ResponsePacket.MessageId, sendValueDataPacket.PropertyName);
                    }
                    break;
                //A confirm action is part of the CommandSendVariable
                case FourByFourConstants.CMD_CONFIRM_PROPERTY:              
                //A confirm action is part of the CommandLaunchAction
                case FourByFourConstants.CMD_CONFIRM_ACTION:
                //A pong command will never be sent on its own.
                case FourByFourConstants.CMD_PONG:
                //4x4 will never get data from the UI.
                case FourByFourConstants.CMD_GET_PROPERTY:
                //4x4 will never launch an action on the UI, it's the other way around.
                case FourByFourConstants.CMD_LAUNCH_ACTION:
                    break;
            }
        }

        public async Task<bool> CommandSendPing()
        {
            if (_UARTService == null || !_UARTService.IsDeviceReady)
                return false;

            //Generate package
            DataPacket packetToSend = new PingDataPacket();

            //Timeout handling
            TaskCompletionSource<DataPacket> signalEventPoke = new TaskCompletionSource<DataPacket>();
            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.MessageId == packetToSend.MessageId)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;

            if (await _UARTService.SendPacket(packetToSend))
            {
                DataPacket reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT_UART_ANSWER);
                _UARTService.UARTPacketReceived -= handler;

                return reception != null;
            }
            else
            {
                //TODO LOG Log.Error("PokeAliveCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return false;
        }

        private async Task<bool> CommandSendPong(ushort sourceMessageId)
        {
            if (_UARTService == null || !_UARTService.IsDeviceReady)
                return false;

            await _UARTService.SendPacket(new PongDataPacket(sourceMessageId));

            return true;
        }

        public async Task<string> CommandGetVariable(string propertyName)
        {
            if (_UARTService == null || !_UARTService.IsDeviceReady)
                return string.Empty;

            //Generate package
            DataPacket packetToSend = new GetPropertyDataPacket(propertyName);

            //Timeout handling
            TaskCompletionSource<DataPacket> signalEventPoke = new TaskCompletionSource<DataPacket>();
            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.MessageId == packetToSend.MessageId)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;

            if (await _UARTService.SendPacket(packetToSend))
            {
                SendPropertyDataPacket reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT_UART_ANSWER) as SendPropertyDataPacket;
                _UARTService.UARTPacketReceived -= handler;

                if (reception != null)
                    await CommandConfirmVariable(reception.MessageId, reception.PropertyName);
                return reception?.PropertyValue;
            }
            else
            {
                //TODO LOG Log.Error("PokeAliveCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return string.Empty;
        }

        private async Task<bool> CommandConfirmVariable(ushort sourceMessageId, string propertyName)
        {
            if (_UARTService == null || !_UARTService.IsDeviceReady)
                return false;

            await _UARTService.SendPacket(new ConfirmVariableDataPacket(sourceMessageId, propertyName));

            return true;
        }

        public async Task<bool> CommandLaunchAction(string actionName)
        {
            return await CommandLaunchAction(actionName, string.Empty);
        }

        public async Task<bool> CommandLaunchAction(string actionName, string parametersJson)
        {
            if (_UARTService == null || !_UARTService.IsDeviceReady)
                return false;

            //Generate package
            DataPacket packetToSend = new LaunchActionDataPacket(actionName, parametersJson);

            //Timeout handling
            TaskCompletionSource<DataPacket> signalEventPoke = new TaskCompletionSource<DataPacket>();
            UARTPacketEventHandler handler = null;
            handler += (sender, e) =>
            {
                if (e.ResponsePacket.MessageId == packetToSend.MessageId)
                {
                    signalEventPoke.SetResult(e.ResponsePacket);
                    _UARTService.UARTPacketReceived -= handler;
                }
            };
            _UARTService.UARTPacketReceived += handler;

            if (await _UARTService.SendPacket(packetToSend))
            {
                DataPacket reception = await TimeoutAsyncEvent.TimeoutAfter(signalEventPoke.Task, TIMEOUT_UART_ANSWER);
                _UARTService.UARTPacketReceived -= handler;

                return reception != null;
            }
            else
            {
                //TODO LOG Log.Error("PokeAliveCommand failed.");
                _UARTService.UARTPacketReceived -= handler;
            }
            return false;
        }

        /// <summary>
        /// Method to be used when the application exits
        /// </summary>
        public void Dispose()
        {
            if (_UARTService != null)
            {
                _UARTService.UARTPacketReceived -= Event_UARTPacketReceived;
                if(_UARTService.SerialPort != null)
                    _UARTService.SerialPort.SerialDeviceReady -= Event_SerialDeviceReady;
                _UARTService.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        private void NewResponseVariableChangedReceived(string variableName, string variableValue)
        {
            VariableModel variable = VariableHelper.Variables[variableName];
            VariableHelper.SetVariableValue(variable, variableValue);

            VariableChanged?.Invoke(this, variable);
        }
    }
}
