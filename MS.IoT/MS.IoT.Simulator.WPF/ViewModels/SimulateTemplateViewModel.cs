using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using MS.IoT.Simulator.WPF.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// SimulateTemplate ModelView
    /// </summary>
    public class SimulateTemplateViewModel : ObservableViewModel, ISimulateTemplateViewModel
    {
        //Services Members
        private ICosmosDBRepository<Template> _TemplateRepo;
        private ICosmosDBRepository<CosmosDBMessage> _MessageRepo;
        private IUserService _UserService;
        private IFourByFourService _FourByFourService;

        //Commands and Property Members
        private AsyncCommand<bool> _InitializeCommand;
        private Template _Template;
        private string _TextSendTextBox;
        private string _TextReceiveTextBox;
        private TemplatePacket _LastTemplatePacketSent;
        private CosmosDBMessage _LastMessageReceived;
        private Guid _CurrentDemoInstanceId;

        //Private members used for the simulation
        private int _MessagesSent;
        private int _MessagesReceived;
        private int _MessagesInterval = 20;
        private bool _SendingCosmosMessages = false;
        private bool _RetrievingCosmosMessages = false;
        private Timer _TimerSending;
        private Timer _TimerReceiving;



        /// <summary>
        /// Property LastTemplatePacketSent used to display the tree of properties for preview
        /// </summary>
        public TemplatePacket LastTemplatePacketSent
        {
            get { return _LastTemplatePacketSent; }
            set
            {
                _LastTemplatePacketSent = value;
                OnPropertyChanged("LastTemplatePacketSent");
            }
        }

        /// <summary>
        /// Property LastMessageReceived used to display a message received from CosmosDB
        /// </summary>
        public CosmosDBMessage LastMessageReceived
        {
            get { return _LastMessageReceived; }
            set
            {
                _LastMessageReceived = value;
                OnPropertyChanged("LastMessageReceived");
            }
        }

        /// <summary>
        /// Property MessagesSent used to display a counter of how messages were sent
        /// </summary>
        public int MessagesSent
        {
            get { return _MessagesSent; }
            set
            {
                _MessagesSent = value;
                OnPropertyChanged("MessagesSent");
            }
        }

        /// <summary>
        /// Property MessagesReceived used to display a counter of how messages were received
        /// </summary>
        public int MessagesReceived
        {
            get { return _MessagesReceived; }
            set
            {
                _MessagesReceived = value;
                OnPropertyChanged("MessagesReceived");
            }
        }

        /// <summary>
        /// Property MessagesInterval used to display a counter of how often messages are sent
        /// </summary>
        public int MessagesInterval
        {
            get { return _MessagesInterval; }
            set
            {
                _MessagesInterval = value;
                if (_TimerSending != null)
                    _TimerSending.Interval = _MessagesInterval * 100;
                OnPropertyChanged("MessagesInterval");
            }
        }

        /// <summary>
        /// Property Template that contains information about the template being simulated
        /// </summary>
        public Template Template
        {
            get { return _Template; }
            set
            {
                _Template = value;
                OnPropertyChanged("Template");
            }
        }

        /// <summary>
        /// Console Sender
        /// </summary>
        public string TextSendTextBox
        {
            get { return _TextSendTextBox; }
            set
            {
                _TextSendTextBox = value;
                OnPropertyChanged("TextSendTextBox");
            }
        }

        /// <summary>
        /// Console Receiver
        /// </summary>
        public string TextReceiveTextBox
        {
            get { return _TextReceiveTextBox; }
            set
            {
                _TextReceiveTextBox = value;
                OnPropertyChanged("TextReceiveTextBox");
            }
        }



        /// <summary>
        /// Accessor to Initiliaze that reset the timers, textboxes and counters and starts a new simulation
        /// </summary>
        public AsyncCommand<bool> Initialize
        {
            get
            {
                return _InitializeCommand;
            }
        }



        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="messageRepo">Repository Message Service</param>
        /// <param name="userService">User Service</param>
        /// <param name="fourByFourService">FourByFour Service</param>
        public SimulateTemplateViewModel(ICosmosDBRepository<Template> templateRepo, ICosmosDBRepository<CosmosDBMessage> messageRepo,IUserService userService, IFourByFourService fourByFourService)
        {
            Log.Debug("SimulateTemplate ViewModel called");
            _TemplateRepo = templateRepo;
            _MessageRepo = messageRepo;
            _UserService = userService;
            _FourByFourService = fourByFourService;
            _InitializeCommand = new AsyncCommand<bool>(() =>
                InitializeSimulation(Parameter)
            );

            //Sending timer
            _TimerSending = new Timer();
            _TimerSending.Elapsed += Event_SendMessages;
            _TimerSending.Interval = _MessagesInterval * 100;

            //Receiving timer
            _TimerReceiving = new Timer();
            _TimerReceiving.Elapsed += Event_ReceiveMessages;
            _TimerReceiving.Interval = 3000;

        }



        /// <summary>
        /// Reset the timers, textboxes and counters and starts a new simulation
        /// </summary>
        /// <param name="parameter">Template ID</param>
        /// <returns></returns>
        private async Task<bool> InitializeSimulation(object parameter)
        {
            _TimerSending.Stop();
            _TimerReceiving.Stop();

            if (parameter == null)
                return false;

            if (parameter != null)
            {
                string templateId = parameter.ToString();
                Template = await GetUserTemplate(templateId);
                if (_Template == null)
                    throw new Exception("The template could not be loaded.");
            }

            //Reset sending
            TextSendTextBox = string.Empty;
            MessagesSent = 0;
            LastTemplatePacketSent = null;

            //Reset receiving
            TextReceiveTextBox = string.Empty;
            MessagesReceived = 0;
            LastMessageReceived = null;
            await DeleteOldCosmosMessages(); // Reset Cosmos DB messages

            _CurrentDemoInstanceId = Guid.NewGuid();
            _TimerSending.Start();
            _TimerReceiving.Start();

            return true;
        }

        /// <summary>
        /// Callback of the Sending timer. Generate a new TemplatePacker and send it through UART
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_SendMessages(object sender, ElapsedEventArgs e)
        {
            if (!_SendingCosmosMessages)
            {
                _SendingCosmosMessages = true;
                TemplatePacket newPacket = TemplateInstanceHelper.GetTemplatePacketFromTemplate(_Template);
                newPacket.DemoId = _MessagesSent;
                newPacket.DemoInstanceId = _CurrentDemoInstanceId;
                LastTemplatePacketSent = newPacket;
                _FourByFourService.SendTemplateCommand(_LastTemplatePacketSent);
                SendFromMessage("Sending message '{0}' to the chip.", _MessagesSent);
                MessagesSent++;
                if (_MessagesSent == 100)
                    _TimerSending.Stop();
                _SendingCosmosMessages = false;
            }
        }

        /// <summary>
        /// Callback of the Receiving timer. Check Cosmos DB for new message and display them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_ReceiveMessages(object sender, ElapsedEventArgs e)
        {
            if(!_RetrievingCosmosMessages)
                UpdateCosmosMessages();
        }

        /// <summary>
        /// Delete all messages from the collection "Message" before attempting a new simulation
        /// </summary>
        /// <returns></returns>
        private async Task DeleteOldCosmosMessages()
        {
            IEnumerable<CosmosDBMessage> messageEnum = await _MessageRepo.GetItemsAsync(p => p.IoTHub != null && p.IoTHub.ConnectionDeviceId == AppConfig.IoTHub.SimulatorDeviceName);
            if (messageEnum != null)
            {
                IEnumerator<CosmosDBMessage> enumerator = messageEnum.GetEnumerator();
                while (enumerator.MoveNext())
                    await _MessageRepo.DeleteItemAsync(enumerator.Current.Id.ToString());
            }
            return;
        }

        /// <summary>
        /// Check Cosmos DB for new message and display them.
        /// </summary>
        private async void UpdateCosmosMessages()
        {
            _RetrievingCosmosMessages = true;

            try
            {
                //IEnumerable<Message> messageEnum = await _MessageRepo.GetItemsAsync(p => p.IoTHub != null && p.IoTHub.ConnectionDeviceId == FourByFourConstants.SHIP_DEMO_DEVICE_ID && p.IoTHub.EnqueuedTime > _DateCheckMessages);
                IEnumerable<CosmosDBMessage> messageEnum = await _MessageRepo.GetItemsAsync(p => p.IoTHub != null && p.IoTHub.ConnectionDeviceId == AppConfig.IoTHub.SimulatorDeviceName && p.DemoId >= _MessagesReceived && p.DemoInstanceId == _CurrentDemoInstanceId);
                if (messageEnum != null)
                {
                    IEnumerator<CosmosDBMessage> enumerator = messageEnum.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        CosmosDBMessage message = enumerator.Current;
                        SendToMessage("Received message '{0}' in CosmosDB", message.DemoId);
                        LastMessageReceived = message;
                        MessagesReceived++;
                    }
                }
                if (_MessagesReceived == 100)
                    _TimerSending.Stop();
            }
            catch(Exception e)
            {
                Log.Error(e, "Error while retrieving Cosmos DB messages.");
            }

            _RetrievingCosmosMessages = false;
        }

        /// <summary>
        /// Display a message in the sending console
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="args">String format parameters</param>
        private void SendFromMessage(string message, params object[] args)
        {
            TextSendTextBox += string.Format(message, args) + "\n";
        }

        /// <summary>
        /// Display a message in the receiving console
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="args">String format parameters</param>
        private void SendToMessage(string message, params object[] args)
        {
            TextReceiveTextBox += string.Format(message, args) + "\n";
        }

        /// <summary>
        /// Overload of the OnUnload method that stops the current simulation (timers)
        /// </summary>
        public override void OnUnload()
        {
            base.OnUnload();

            _TimerSending.Stop();
            _TimerReceiving.Stop();
        }

        /// <summary>
        /// Retrieve a User Template from a TemplateId 
        /// </summary>
        /// <param name="parameter">Template ID</param>
        /// <returns></returns>
        private async Task<Template> GetUserTemplate(object parameter)
        {
            if (parameter == null)
                return null;

            string templateId = parameter.ToString();

            try
            {
                //User currentUser = _UserService.GetCurrentUser();
                Template template = await _TemplateRepo.GetItemAsync(templateId);
                Template = template;
                return template;
            }
            catch(Exception e)
            {
                Log.Error(e, string.Format("Error while retrieving the template {0}.", templateId));
                return null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OnUnload();
            if(_TimerReceiving != null)
                _TimerReceiving.Dispose();
            if (_TimerSending != null)
                _TimerSending.Dispose();
        }
    }
}
