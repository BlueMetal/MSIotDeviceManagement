using MS.IoT.UWP.CoffeeMaker.Helpers;
using MS.IoT.UWP.CoffeeMaker.Models;
using MS.IoT.UWP.CoffeeMaker.Services;
using MS.IoT.UWP.CoffeeMaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Reflection;

namespace MS.IoT.UWP.CoffeeMaker.ViewModels
{
    /// <summary>
    /// Main Window model view, used to control navigation, login and footer features
    /// </summary>
    public class MainWindowViewModel : ObservableViewModel, IDisposable
    {
        #region Members
        //Services Members
        private IFourByFourService _FourByFourService;

        //Panels
        private bool _IsPanelMainVisible;
        private bool _IsPanelWifiVisible;
        private bool _IsPanelLoadingVisible;
        private bool _IsPanelErrorVisible;
        private bool _IsPanelDebugVisible;

        //Features
        private bool _IsFeatureBrewStrengthActivated = false;
        private bool _IsFeatureBrewActivated = false;
        private bool _IsFeatureGrindAndBrewActivated = false;
        private bool _IsFeatureWifiActivated = false;
        private bool _IsFeatureDebugActivated = false;

        //Stats
        private DateTime _DtLastBrewedDate;
        private string _LblLastBrewedDate = string.Empty;
        private int _LblPotsBrewedToday = 0;
        private int _LblPotsBrewedWeek = 0;
        private int _LblPotsBrewedTotal = 0;

        //Clock
        private string _LblCurrentDateTime;
        private bool _ClockAnimToggle = false;

        //Errors
        private bool _IsErrorUART;

        //Feature Brew Strength
        private bool _IsActionBrewStrengthLoading = false;
        private int _ActionBrewStrengthValue;
        private string _ActionBrewStrengthDescription = string.Empty;
        private ImageSource _ActionBrewStrengthImage;       

        //Feature Brew
        private bool _IsActionBrewLoading = false;
        private bool _IsActionBrewLaunched = false;
        private string _ActionBrewDescription = string.Empty;
        private ImageSource _ActionBrewImage;
        private DateTime _DtActionBrewETA = DateTime.MinValue;

        //Feature Grind & Brew
        private bool _IsActionGrindAndBrewLoading = false;
        private bool _IsActionGrindAndBrewLaunched = false;
        private string _ActionGrindAndBrewDescription = string.Empty;
        private ImageSource _ActionGrindAndBrewImage;
        private DateTime _DtActionGrindAndBrewETA = DateTime.MinValue;

        //Wifi
        private bool _IsWifiConnected = false;
        private string _WifiSSID;
        private string _WifiPassword;
        private ImageSource _WifiStatusImage;

        //Debug
        private string _DebugListVariables;
        private string _DebugUIConsole;

        //Timer
        private DispatcherTimer _NetworkTimer;
        private DispatcherTimer _ClockTimer;
        #endregion

        #region Properties
        //Pabel
        public bool IsPanelMainVisible
        {
            get { return _IsPanelMainVisible; }
            set
            {
                _IsPanelMainVisible = value;
                OnPropertyChanged("IsPanelMainVisible");
            }
        }

        public bool IsPanelWifiVisible
        {
            get { return _IsPanelWifiVisible; }
            set
            {
                _IsPanelWifiVisible = value;
                OnPropertyChanged("IsPanelWifiVisible");
            }
        }

        public bool IsPanelDebugVisible
        {
            get { return _IsPanelDebugVisible; }
            set
            {
                _IsPanelDebugVisible = value;
                if(_IsPanelDebugVisible)
                    WriteListVariable();
                OnPropertyChanged("IsPanelDebugVisible");
            }
        }

        public bool IsPanelLoadingVisible
        {
            get { return _IsPanelLoadingVisible; }
            set
            {
                _IsPanelLoadingVisible = value;
                OnPropertyChanged("IsPanelLoadingVisible");
            }
        }

        public bool IsPanelErrorVisible
        {
            get { return _IsPanelErrorVisible; }
            set
            {
                _IsPanelErrorVisible = value;
                OnPropertyChanged("IsPanelErrorVisible");
            }
        }


        //Features
        public bool IsFeatureBrewStrengthActivated
        {
            get { return _IsFeatureBrewStrengthActivated; }
            set
            {
                _IsFeatureBrewStrengthActivated = value;
                OnPropertyChanged("IsFeatureBrewStrengthActivated");
            }
        }

        public bool IsFeatureBrewActivated
        {
            get { return _IsFeatureBrewActivated; }
            set
            {
                _IsFeatureBrewActivated = value;
                OnPropertyChanged("IsFeatureBrewActivated");
            }
        }

        public bool IsFeatureWifiActivated
        {
            get { return _IsFeatureWifiActivated; }
            set
            {
                _IsFeatureWifiActivated = value;
                OnPropertyChanged("IsFeatureWifiActivated");
            }
        }

        public bool IsFeatureDebugActivated
        {
            get { return _IsFeatureDebugActivated; }
            set
            {
                _IsFeatureDebugActivated = value;
                OnPropertyChanged("IsFeatureDebugActivated");
            }
        }

        public bool IsFeatureGrindAndBrewActivated
        {
            get { return _IsFeatureGrindAndBrewActivated; }
            set
            {
                _IsFeatureGrindAndBrewActivated = value;
                OnPropertyChanged("IsFeatureGrindAndBrewActivated");
            }
        }


        //Stats
        public DateTime DtLastBrewedDate
        {
            get { return _DtLastBrewedDate; }
            set
            {
                _DtLastBrewedDate = value;
                if (_DtLastBrewedDate.Year == 1970)
                    LblLastBrewedDate = string.Empty;
                else
                    LblLastBrewedDate = _DtLastBrewedDate.ToLocalTime().ToString("hh:mm tt");
            }
        }

        public string LblLastBrewedDate
        {
            get { return _LblLastBrewedDate; }
            set
            {
                _LblLastBrewedDate = value;
                OnPropertyChanged("LblLastBrewedDate");
            }
        }

        public int LblPotsBrewedToday
        {
            get { return _LblPotsBrewedToday; }
            set
            {
                _LblPotsBrewedToday = value;
                OnPropertyChanged("LblPotsBrewedToday");
            }
        }

        public int LblPotsBrewedWeek
        {
            get { return _LblPotsBrewedWeek; }
            set
            {
                _LblPotsBrewedWeek = value;
                OnPropertyChanged("LblPotsBrewedWeek");
            }
        }

        public int LblPotsBrewedTotal
        {
            get { return _LblPotsBrewedTotal; }
            set
            {
                _LblPotsBrewedTotal = value;
                OnPropertyChanged("LblPotsBrewedTotal");
            }
        }


        //Clock
        public string LblCurrentDateTime
        {
            get { return _LblCurrentDateTime; }
            set
            {
                _LblCurrentDateTime = value;
                OnPropertyChanged("LblCurrentDateTime");
            }
        }


        //Errors
        public bool IsErrorUART
        {
            get { return _IsErrorUART; }
            set
            {
                _IsErrorUART = value;
                OnPropertyChanged("IsErrorUART");
                SetErrorPanel();
            }
        }


        //Feature Brew Strength
        public bool IsActionBrewStrengthLoading
        {
            get { return _IsActionBrewStrengthLoading; }
            set
            {
                _IsActionBrewStrengthLoading = value;
                OnPropertyChanged("IsActionBrewStrengthLoading");
            }
        }

        public int ActionBrewStrengthValue
        {
            get { return _ActionBrewStrengthValue; }
            set
            {
                _ActionBrewStrengthValue = value;
                switch (_ActionBrewStrengthValue)
                {
                    case 0:
                        ActionBrewStrengthImage = FourByFourConstants.ICON_BREW_STRENGTH_REGULAR;
                        ActionBrewStrengthDescription = FourByFourConstants.BREW_STRENGTH_REGULAR;
                        break;
                    case 1:
                        ActionBrewStrengthImage = FourByFourConstants.ICON_BREW_STRENGTH_BOLD;
                        ActionBrewStrengthDescription = FourByFourConstants.BREW_STRENGTH_BOLD;
                        break;
                    case 2:
                        ActionBrewStrengthImage = FourByFourConstants.ICON_BREW_STRENGTH_STRONG;
                        ActionBrewStrengthDescription = FourByFourConstants.BREW_STRENGTH_STRONG;
                        break;
                }
                IsActionBrewStrengthLoading = false;
                OnPropertyChanged("ActionBrewStrengthValue");
            }
        }

        public string ActionBrewStrengthDescription
        {
            get { return _ActionBrewStrengthDescription; }
            set
            {
                _ActionBrewStrengthDescription = value;
                OnPropertyChanged("ActionBrewStrengthDescription");
            }
        }

        public ImageSource ActionBrewStrengthImage
        {
            get { return _ActionBrewStrengthImage; }
            set
            {
                _ActionBrewStrengthImage = value;
                OnPropertyChanged("ActionBrewStrengthImage");
            }
        }


        //Feature Brew
        public bool IsActionBrewLoading
        {
            get { return _IsActionBrewLoading; }
            set
            {
                _IsActionBrewLoading = value;
                OnPropertyChanged("IsActionBrewLoading");
            }
        }

        public bool IsActionBrewLaunched
        {
            get { return _IsActionBrewLaunched; }
            set
            {
                _IsActionBrewLaunched = value;
                IsActionBrewLoading = false;
                ActionBrewDescription = string.Empty;
                ActionBrewImage = _IsActionBrewLaunched ? FourByFourConstants.ICON_BREW_ACTIVE : FourByFourConstants.ICON_BREW_INACTIVE;
                if (_IsActionBrewLaunched)
                    ActionBrewStrengthImage = FourByFourConstants.ICON_BREW_STRENGTH_DISABLED;
                else
                    ActionBrewStrengthValue = ActionBrewStrengthValue;
                OnPropertyChanged("IsActionBrewLaunched");
            }
        }

        public DateTime DtActionBrewETA
        {
            get { return _DtActionBrewETA; }
            set
            {
                _DtActionBrewETA = value;
                OnPropertyChanged("DtActionBrewETA");
            }
        }

        public string ActionBrewDescription
        {
            get { return _ActionBrewDescription; }
            set
            {
                _ActionBrewDescription = value;
                OnPropertyChanged("ActionBrewDescription");
            }
        }

        public ImageSource ActionBrewImage
        {
            get { return _ActionBrewImage; }
            set
            {
                _ActionBrewImage = value;
                OnPropertyChanged("ActionBrewImage");
            }
        }


        //Feature Grind & Brew
        public bool IsActionGrindAndBrewLoading
        {
            get { return _IsActionGrindAndBrewLoading; }
            set
            {
                _IsActionGrindAndBrewLoading = value;
                OnPropertyChanged("IsActionGrindAndBrewLoading");
            }
        }

        public bool IsActionGrindAndBrewLaunched
        {
            get { return _IsActionGrindAndBrewLaunched; }
            set
            {
                _IsActionGrindAndBrewLaunched = value;
                IsActionGrindAndBrewLoading = false;
                ActionGrindAndBrewDescription = string.Empty;
                ActionGrindAndBrewImage = _IsActionGrindAndBrewLaunched ? FourByFourConstants.ICON_GRIND_BREW_ACTIVE : FourByFourConstants.ICON_GRIND_BREW_INACTIVE;
                if (_IsActionGrindAndBrewLaunched)
                    ActionBrewStrengthImage = FourByFourConstants.ICON_BREW_STRENGTH_DISABLED;
                else
                    ActionBrewStrengthValue = ActionBrewStrengthValue;
                OnPropertyChanged("IsActionGrindAndBrewLaunched");
            }
        }

        public DateTime DtActionGrindAndBrewETA
        {
            get { return _DtActionGrindAndBrewETA; }
            set
            {
                _DtActionGrindAndBrewETA = value;
                OnPropertyChanged("DtActionGrindAndBrewETA");
            }
        }

        public string ActionGrindAndBrewDescription
        {
            get { return _ActionGrindAndBrewDescription; }
            set
            {
                _ActionGrindAndBrewDescription = value;
                OnPropertyChanged("ActionGrindAndBrewDescription");
            }
        }

        public ImageSource ActionGrindAndBrewImage
        {
            get { return _ActionGrindAndBrewImage; }
            set
            {
                _ActionGrindAndBrewImage = value;
                OnPropertyChanged("ActionGrindAndBrewImage");
            }
        }


        //Wifi
        public bool IsWifiConnected
        {
            get { return _IsWifiConnected; }
            set
            {
                _IsWifiConnected = value;
                WifiStatusImage = _IsWifiConnected ? FourByFourConstants.ICON_WIFI_CONNECTED : FourByFourConstants.ICON_WIFI_DISCONNECTED;
                OnPropertyChanged("IsWifiConnected");
            }
        }

        public ImageSource WifiStatusImage
        {
            get { return _WifiStatusImage; }
            set
            {
                _WifiStatusImage = value;
                OnPropertyChanged("WifiStatusImage");
            }
        }

        public string WifiSSID
        {
            get { return _WifiSSID; }
            set
            {
                _WifiSSID = value;
                OnPropertyChanged("WifiSSID");
            }
        }

        public string WifiPassword
        {
            get { return _WifiPassword; }
            set
            {
                _WifiPassword = value;
                OnPropertyChanged("WifiPassword");
            }
        }

        //Debug
        public string DebugListVariables
        {
            get { return _DebugListVariables; }
            set
            {
                _DebugListVariables = value;
                OnPropertyChanged("DebugListVariables");
            }
        }

        public string DebugUIConsole
        {
            get { return _DebugUIConsole; }
            set
            {
                _DebugUIConsole = value;
                OnPropertyChanged("DebugUIConsole");
            }
        }
        #endregion

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            //Set FourByFour callback
            _FourByFourService = new FourByFourService();
            _FourByFourService.Init();
            _FourByFourService.SerialDeviceReady += Event_SerialDeviceReady;
            _FourByFourService.VariableChanged += Event_VariableChanged;

            //Set Timer to test UART connection
            _NetworkTimer = new DispatcherTimer();
            _NetworkTimer.Interval = TimeSpan.FromSeconds(FourByFourConstants.PING_INTERVAL_SECONDS_IF_DISCONNECTED);
            _NetworkTimer.Tick += Event_SendPokeAlive_Tick;
            _NetworkTimer.Start();

            //Set clock
            _ClockTimer = new DispatcherTimer();
            _ClockTimer.Interval = TimeSpan.FromSeconds(1);
            _ClockTimer.Tick += Event_UpdateClock_Tick;
            _ClockTimer.Start();
        }

        /// <summary>
        /// Initialization sequence. Request variables state from the 4x4
        /// </summary>
        public async void Init()
        {
            LogHelper.LogInfo("Initialize model...");

            try
            {
                VariableHelper.Init();
                foreach (VariableModel variable in VariableHelper.Variables.Values)
                {
                    VariableHelper.SetVariableValue(variable, await _FourByFourService.CommandGetVariable(variable.VariableName));
                    Event_VariableChanged(this, variable);
                }
                LogHelper.LogInfo("Variables loaded.");
            }
            catch (Exception e)
            {
                LogHelper.LogError(e.Message);
            }

            IsPanelLoadingVisible = false;
            IsPanelMainVisible = true;
        }

        /// <summary>
        /// Launch Action Brew Strength
        /// </summary>
        public void LaunchActionBrewStrength()
        {
            LogHelper.LogInfo(string.Format("LaunchActionBrewStrength with parameter {0}", _ActionBrewStrengthValue));

            IsActionBrewStrengthLoading = true;
            int newState = _ActionBrewStrengthValue + 1;
            if (newState >= 3)
                newState = 0;
            _FourByFourService.CommandLaunchAction(FourByFourConstants.METHOD_CHANGE_BREW_STRENGTH, "{\"strength\": " + newState.ToString() + "}");
        }

        /// <summary>
        /// Launch Action Brew
        /// </summary>
        public void LaunchActionBrew()
        {
            if (IsActionGrindAndBrewLaunched)
                return;

            LogHelper.LogInfo("LaunchActionBrew");

            IsActionBrewLoading = true;
            _FourByFourService.CommandLaunchAction(FourByFourConstants.METHOD_ACTION_BREW);
        }

        /// <summary>
        /// Launch Action Grind and Brew
        /// </summary>
        public void LaunchActionGrindBrew()
        {
            if (IsActionBrewLaunched)
                return;

            LogHelper.LogInfo("LaunchActionGrindBrew");

            IsActionGrindAndBrewLoading = true;
            _FourByFourService.CommandLaunchAction(FourByFourConstants.METHOD_ACTION_GRIND_AND_BREW);
        }

        /// <summary>
        /// Launch Action Set Wifi
        /// </summary>
        public void LaunchActionSetWifi()
        {
            LogHelper.LogInfo("LaunchActionSetWifi");

            this._FourByFourService.CommandLaunchAction(FourByFourConstants.METHOD_ACTION_WIFI, "{\"ssid\" : \"" + _WifiSSID + "\", \"password\": \"" + _WifiPassword + "\", \"security\": \"psk\"}");
            CloseWifiPanel();
        }

        /// <summary>
        /// Show the error panel if an action or uart error occured
        /// </summary>
        private void SetErrorPanel()
        {
            IsPanelErrorVisible = _IsErrorUART;
        }
        
        /// <summary>
        /// Open wifi panel and reset password string
        /// </summary>
        public void OpenWifiPanel()
        {
            WifiPassword = string.Empty;
            IsPanelMainVisible = false;
            IsPanelWifiVisible = true;
        }

        /// <summary>
        /// Open debug panel and reset password string
        /// </summary>
        public void OpenDebugPanel()
        {
            IsPanelDebugVisible = true;
            IsPanelMainVisible = false;
        }

        /// <summary>
        /// Close wifi panel
        /// </summary>
        public void CloseWifiPanel()
        {
            IsPanelMainVisible = true;
            IsPanelWifiVisible = false;
        }

        /// <summary>
        /// Close debug panel
        /// </summary>
        public void CloseDebugPanel()
        {
            IsPanelDebugVisible = false;
            IsPanelMainVisible = true;
        }

        public void WriteListVariable()
        {
            DebugListVariables = string.Empty;
            if (VariableHelper.Variables == null)
                return;

            foreach (VariableModel variable in VariableHelper.Variables.Values)
            {
                DebugListVariables += string.Format("{0,-32} {1}\n", variable.PropertyName, variable.PropertyValue);
            }
        }

        /// <summary>
        /// Wait till the Serial Device is ready before Init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_SerialDeviceReady(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// Receive an event that signifies a variable was changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="variable"></param>
        private void Event_VariableChanged(object sender, VariableModel variable)
        {
            LogHelper.LogInfo(string.Format("Variable: {0}. New Value: {1}", variable.PropertyName, variable.PropertyValue));

            PropertyInfo property = this.GetType().GetProperty(variable.PropertyName);
            if (property == null)
                return;
            property.SetValue(this, variable.PropertyValue);

            if (IsPanelDebugVisible)
                WriteListVariable();
        }

        /// <summary>
        /// Async method that test if the UART connection is established and wait for an answer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Event_SendPokeAlive_Tick(object sender, object e)
        {
            if (!_FourByFourService.IsDeviceReady)
                return;

            bool result = await _FourByFourService.CommandSendPing();
            IsErrorUART = !result;
            if (IsErrorUART)
                _NetworkTimer.Interval = TimeSpan.FromSeconds(FourByFourConstants.PING_INTERVAL_SECONDS_IF_DISCONNECTED);
            else
                _NetworkTimer.Interval = TimeSpan.FromSeconds(FourByFourConstants.PING_INTERVAL_SECONDS);
        }

        /// <summary>
        /// Method that updates the timer for action brew and grind brew as well as the clock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_UpdateClock_Tick(object sender, object e)
        {
            //Clock
            if (_ClockAnimToggle)
                LblCurrentDateTime = DateTime.Now.ToString("hh:mm tt");
            else
                LblCurrentDateTime = DateTime.Now.ToString("hh mm tt");
            _ClockAnimToggle = !_ClockAnimToggle;

            //Timers
            if (_DtActionBrewETA > DateTime.Now)
                ActionBrewDescription = string.Format("Approx. {0} minutes", _DtActionBrewETA.Subtract(DateTime.Now).Minutes + 1);
            else
                ActionBrewDescription = string.Empty;

            if (_DtActionGrindAndBrewETA > DateTime.Now)
                ActionGrindAndBrewDescription = string.Format("Approx. {0} minutes", _DtActionGrindAndBrewETA.Subtract(DateTime.Now).Minutes + 1);
            else
                ActionGrindAndBrewDescription = string.Empty;
        }

        /// <summary>
        /// Release resources for services and timers when the application exits
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_FourByFourService != null)
                _FourByFourService.Dispose();
            if (_NetworkTimer != null)
            {
                _NetworkTimer.Stop();
                _NetworkTimer = null;
            }
        }
    }
}
