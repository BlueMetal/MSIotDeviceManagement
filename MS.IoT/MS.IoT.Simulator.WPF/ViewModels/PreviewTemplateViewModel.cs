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

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// PreviewTemplate ModelView
    /// </summary>
    public class PreviewTemplateViewModel : ObservableViewModel, IPreviewTemplateViewModel
    {
        //Services Members
        private ICosmosDBRepository<Template> _TemplateRepo;
        private IUserService _UserService;
        private IFourByFourService _CodenameService;
        private IIoTHubService _IotHubService;

        //Properties Members
        private Template _Template;
        private string _ConnectionString;
        private string _TextBlockConsole;
        private bool _ReadyToSimulate;
        private bool _CheckError;
        private bool _IsCheckingStatus;
        private List<COMDeviceInfo> _COMDevices;
        private COMDeviceInfo _SelectedCOMDevice;
        private TemplatePacket _TemplatePacket;

        //Commands Members
        private AsyncCommand<bool> _Initialize;
        private RelayCommand _RetryCheck;
        private RelayCommand _DeviceChanged;

        /// <summary>
        /// Property Template Packet used to display the tree of properties for preview
        /// </summary>
        public TemplatePacket TemplatePacket
        {
            get { return _TemplatePacket; }
            set
            {
                _TemplatePacket = value;
                OnPropertyChanged("TemplatePacket");
            }
        }

        /// <summary>
        /// Property Template containing all the properties of the current Template
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
        /// Property IsCheckingStatus used to show a loading animation when the status of the 4x4 is being checked
        /// </summary>
        public bool IsCheckingStatus
        {
            get { return _IsCheckingStatus; }
            set
            {
                _IsCheckingStatus = value;
                OnPropertyChanged("IsCheckingStatus");
            }
        }

        /// <summary>
        /// Property ReadyToSimulate used to enable/disable to simulate button
        /// </summary>
        public bool ReadyToSimulate
        {
            get { return _ReadyToSimulate; }
            set
            {
                _ReadyToSimulate = value;
                OnPropertyChanged("ReadyToSimulate");
            }
        }

        /// <summary>
        /// Property CheckError used to show a failure button
        /// </summary>
        public bool CheckError
        {
            get { return _CheckError; }
            set
            {
                _CheckError = value;
                OnPropertyChanged("CheckError");
            }
        }

        /// <summary>
        /// Property TextBlockConsole used to show the checks progress
        /// </summary>
        public string TextBlockConsole
        {
            get { return _TextBlockConsole; }
            set
            {
                _TextBlockConsole = value;
                OnPropertyChanged("TextBlockConsole");
            }
        }

        /// <summary>
        /// Property COMDeviceInfo used to list the COM Devices on the computer. 
        /// It does not need to be observable as it is only set during initialization.
        /// </summary>
        public List<COMDeviceInfo> COMDevices
        {
            get
            {
                return _COMDevices;
            }
            set
            {
                _COMDevices = value;
                OnPropertyChanged("COMDevices");
            }
        }

        /// <summary>
        /// Property SelectedCOMDevice used to indicate the selected COM Device.
        /// </summary>
        public COMDeviceInfo SelectedCOMDevice
        {
            get
            {
                return _SelectedCOMDevice;
            }
            set
            {
                _SelectedCOMDevice = value;
                OnPropertyChanged("SelectedCOMDevice");
            }
        }



        /// <summary>
        /// Accessor to Initialize command, used to:
        /// - Retrieve list of USB COM Devices
        /// - Load the template passed in parameter to the view
        /// - Attempt to check the 4x4 status
        /// </summary>
        public AsyncCommand<bool> Initialize
        {
            get
            {
                return _Initialize;
            }
        }

        /// <summary>
        /// Accessor to RetryCheck used to check the 4x4 status
        /// </summary>
        public RelayCommand RetryCheck
        {
            get
            {
                return _RetryCheck;
            }
        }

        /// <summary>
        /// Accessor to DeviceChanged, used to check the 4x4 status on a specific COM device
        /// </summary>
        public RelayCommand DeviceChanged
        {
            get
            {
                return _DeviceChanged;
            }
        }



        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="userService">User Service</param>
        /// <param name="codenameService">4x4 Service</param>
        /// <param name="iotHubService">IoT Hub Service</param>
        public PreviewTemplateViewModel(ICosmosDBRepository<Template> templateRepo, IUserService userService, IFourByFourService codenameService, IIoTHubService iotHubService)
        {
            Log.Debug("PreviewTemplate ViewModel called");
            _TemplateRepo = templateRepo;
            _UserService = userService;
            _IotHubService = iotHubService;
            _CodenameService = codenameService;

            //Set up chip default deviceID
            //_CodenameService.FourByFourPacketReceived += _CodenameService_CodenamePacketReceived;
            COMDevices = _CodenameService.GetCOMDevices();
            _SelectedCOMDevice = _COMDevices.Find(p => p.DeviceID == _CodenameService.SelectCOMDevice.DeviceID);
            
            //Set commands
            _Initialize = new AsyncCommand<bool>(() => InitializeView(Parameter));
            _RetryCheck = new RelayCommand(p => InitializeChip());
            _DeviceChanged = new RelayCommand(p => _CodenameService.SetCOMDevice(_SelectedCOMDevice.DeviceID));
        }

        /// <summary>
        /// Initialize the View with a specific template ID
        /// </summary>
        /// <param name="parameter">templateId</param>
        /// <returns></returns>
        private async Task<bool> InitializeView(object parameter)
        {
            ReadyToSimulate = false;

            if (parameter == null)
                return false;

            string templateId = parameter.ToString();
            Template = await GetUserTemplate(templateId);
            TemplatePacket = TemplateInstanceHelper.GetTemplatePacketFromTemplate(_Template);

            InitializeChip();

            return true;
        }

        /// <summary>
        /// Runs all the checks to ensure that the chip is properly connected, 
        /// the connection string is properly copied in the device and that IoT Hub is accessible.
        /// </summary>
        private async void InitializeChip()
        {
            if (_IsCheckingStatus)
                return;

            try
            {
                IsCheckingStatus = true;
                TextBlockConsole = string.Empty;
                CheckError = false;
                ReadyToSimulate = false;

                //Register Device to IoT Hub
                _ConnectionString = await _IotHubService.RegisterDeviceInIoTHubAsync(AppConfig.IoTHub.SimulatorDeviceName);

                //Check UART connection status
                if (!(await _CodenameService.TestUARTConnection()))
                {
                    SendStatusMessage("No UART Connection. Please make sure that the UART device selected is the right one and that the pins are properly connected.");
                    CheckError = true;
                    IsCheckingStatus = false;
                    return;
                }

                //Check IoT Hub configuration on the device
                string connectionString = await _CodenameService.GetConnectionStringFromChip();
                if (string.IsNullOrEmpty(connectionString) || connectionString != _ConnectionString)
                {
                    //Setting IoT Hub Connection string on the device
                    if (!(await _CodenameService.SetConfigOnChip(_ConnectionString)))
                    {
                        SendStatusMessage("The IoT Hub connection string could not be set on your 4x4 chip.");
                        CheckError = true;
                        IsCheckingStatus = false;
                        return;
                    }
                }

                //Test IoT Hub response
                if (!(await _CodenameService.TestIoTHubConnection()))
                {
                    SendStatusMessage("The connection to IoT Hub failed. Please make sure that your device is connected to Wifi.");
                    CheckError = true;
                    IsCheckingStatus = false;
                    return;
                }

                //The connection to IoT Hub succeeded. Ready to simulate.
                ReadyToSimulate = true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while checking the status of the 4x4 Chip.");
            }
            finally
            {
                IsCheckingStatus = false;
            }
        }

        /// <summary>
        /// Retrieve a User Template from a TemplateId 
        /// </summary>
        /// <param name="templateId">Template ID</param>
        /// <returns></returns>
        private async Task<Template> GetUserTemplate(string templateId)
        {
            try
            {
                //User currentUser = _UserService.GetCurrentUser();
                return await _TemplateRepo.GetItemAsync(templateId);
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("Error while retrieving the template {0}.", templateId));
                return null;
            }
        }

        /// <summary>
        /// Send message status to the UI in case of error
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="args">String format arguments</param>
        private void SendStatusMessage(string message, params object[] args)
        {
            TextBlockConsole += string.Format(message, args) + "\n";
        }
    }
}
