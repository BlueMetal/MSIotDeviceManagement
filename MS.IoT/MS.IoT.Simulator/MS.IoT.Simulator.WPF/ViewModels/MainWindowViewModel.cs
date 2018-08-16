using Microsoft.Practices.Unity;
using MS.IoT.Common;
using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Models.Packets;
using MS.IoT.Simulator.WPF.Services;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using System;
using System.Timers;

namespace MS.IoT.Simulator.WPF.ViewModels
{
    /// <summary>
    /// Main Window model view, used to control navigation, login and footer features
    /// </summary>
    public class MainWindowViewModel : ObservableViewModel
    {
        //Services Members
        private IUnityContainer _Container;
        private IUserService _UserService;
        private IFourByFourService _CodenameService;

        //Properties Members
        private User _CurrentUser;
        private bool _IsAuthenticated;
        private bool _ChipNetworkReady;
        private bool _ChipUARTResponding;
        private ObservableViewModel _CurrentViewModel;

        //Commands Members
        private RelayCommand _ChooseTemplateCommand;
        private RelayCommand _SimulateTemplateCommand;
        private RelayCommand _PreviewTemplateCommand;
        private AsyncCommand<AzureConnectionStatus> _LoginCommand;
        private RelayCommand _LogoutCommand;

        //ViewModels Members
        private HomeViewModel _HomeViewModel;
        private SelectTemplateViewModel _SelectTemplateViewModel;
        private PreviewTemplateViewModel _PreviewTemplateViewModel;
        private SimulateTemplateViewModel _SimulateTemplateViewModel;

        //Timer
        private System.Timers.Timer _NetworkTimer;
        private const int POKE_INTERVAL = 5000;


        /// <summary>
        /// Property used to navigate between views
        /// </summary>
        public ObservableViewModel CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set
            {
                _CurrentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }
        
        /// <summary>
        /// Property CurrentUser used to display the current user's name
        /// </summary>
        public User CurrentUser
        {          
            get
            {
                return _CurrentUser;
            }
            set
            {
                _CurrentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }
        
        /// <summary>
        /// Property IsAuthenticated used to show/hide the footer and Sign Out
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _IsAuthenticated;
            }
            set
            {
                _IsAuthenticated = value;
                OnPropertyChanged("IsAuthenticated");
            }
        }

        /// <summary>
        /// Property ChipNetworkReady used in the footer to notify if the chip is connected to the OpenVPN Network SL0
        /// </summary>
        public bool ChipNetworkReady
        {
            get
            {
                return _ChipNetworkReady;
            }
            set
            {
                _ChipNetworkReady = value;
                OnPropertyChanged("ChipNetworkReady");
            }
        }

        /// <summary>
        /// Property ChipResponding used in the footer to notify if the chip is responding through UART connection
        /// </summary>
        public bool ChipUARTResponding
        {
            get
            {
                return _ChipUARTResponding;
            }
            set
            {
                _ChipUARTResponding = value;
                OnPropertyChanged("ChipUARTResponding");
            }
        }



        /// <summary>
        /// Accessor to Login Command, redirects to UserService.SignIn()
        /// </summary>
        public AsyncCommand<AzureConnectionStatus> LoginCommand
        {
            get
            {
                return _LoginCommand;
            }
        }

        /// <summary>
        /// Accessor to Logout Command, redirects to UserService.SignOut()
        /// </summary>
        public RelayCommand LogoutCommand
        {
            get
            {
                return _LogoutCommand;
            }
        }

        /// <summary>
        /// Accessor to ChooseTemplateCommand, redirects to SelectTemplateViewModel view
        /// </summary>
        public RelayCommand ChooseTemplateCommand
        {
            get
            {
                return _ChooseTemplateCommand;
            }
        }

        /// <summary>
        /// Accessor to PreviewTemplateCommand, redirects to PreviewTemplateViewModel view
        /// </summary>
        public RelayCommand PreviewTemplateCommand
        {
            get
            {
                return _PreviewTemplateCommand;
            }
        }

        /// <summary>
        /// Accessor to SimulateTemplateCommand, redirects to SimulateTemplateViewModel view
        /// </summary>
        public RelayCommand SimulateTemplateCommand
        {
            get
            {
                return _SimulateTemplateCommand;
            }
        }



        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="container">Container Unity for DI</param>
        /// <param name="userService">User Service</param>
        /// <param name="codenameService">4x4 Service</param>
        public MainWindowViewModel(IUnityContainer container, IUserService userService, IFourByFourService codenameService)
        {
            Log.Debug("Main Window called");
            _Container = container;

            //Set Current User Callback
            _UserService = userService;
            _UserService.ConnectionStatusChanged += Event_UserService_ConnectionStatusChanged;
            IsAuthenticated = false;

            //Set Codename callback
            _CodenameService = codenameService;
            _CodenameService.Init();

            //Initialize different views
            _HomeViewModel = container.Resolve<HomeViewModel>();
            _SelectTemplateViewModel = container.Resolve<SelectTemplateViewModel>();
            _PreviewTemplateViewModel = container.Resolve<PreviewTemplateViewModel>();
            _SimulateTemplateViewModel = container.Resolve<SimulateTemplateViewModel>();

            //Set home view
            Navigate(_HomeViewModel);

            //Set commands
            _LoginCommand = new AsyncCommand<AzureConnectionStatus>(() => _UserService.SignIn());
            _LogoutCommand = new RelayCommand(p => _UserService.SignOut());
            _ChooseTemplateCommand = new RelayCommand(p => Navigate(_SelectTemplateViewModel));
            _PreviewTemplateCommand = new RelayCommand(p => Navigate(_PreviewTemplateViewModel, p));
            _SimulateTemplateCommand = new RelayCommand(p => Navigate(_SimulateTemplateViewModel, p));

            //Init Connection Timer - This timer will poke the 4x4 to check for responsivity, as well as the network sl0
            _NetworkTimer = new System.Timers.Timer();
            _NetworkTimer.Elapsed += Event_RefreshConnectionInformation;
            _NetworkTimer.Interval = POKE_INTERVAL;
            _NetworkTimer.Start();
            Event_RefreshConnectionInformation(this, null);
        }

        

        /// <summary>
        /// Event that notifies any connection change with the current user. This event is typically thrown after SignIn / SignOut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_UserService_ConnectionStatusChanged(object sender, RelayItemEventArgs<AzureConnectionObject> e)
        {
            CurrentUser = e.Object.User;
            IsAuthenticated = e.Object.ConnectionStatus == AzureConnectionStatus.Authenticated;
            if (IsAuthenticated)
            {
                Log.Debug(string.Format("User {0} logged in", _CurrentUser.Name));
                Navigate(_SelectTemplateViewModel);
            }
            else
            {
                Log.Debug("User logged out");
                _SelectTemplateViewModel.ResetControl();
                Navigate(_HomeViewModel);
            }
        }

        /// <summary>
        /// Event on a timer that will check the connection with UART and the SL0 network.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_RefreshConnectionInformation(object sender, ElapsedEventArgs e)
        {
            ChipNetworkReady = _CodenameService.PokeNetworkAvailability();
            TestUARTConnection();
        }

        

        /// <summary>
        /// Method Navigated used to change the current ViewModel context.
        /// </summary>
        /// <param name="viewModel">Instance of the ObservableViewModel to swap to</param>
        /// <param name="parameter">Parameter sent to the ViewModel</param>
        private void Navigate(ObservableViewModel viewModel, object parameter = null)
        {
            if (_CurrentViewModel != null)
                _CurrentViewModel.OnUnload();
            CurrentViewModel = viewModel;
            CurrentViewModel.Parameter = parameter;
            _CurrentViewModel.OnLoad();
        }

        /// <summary>
        /// Async method that test if the UART connection is established and wait for an answer
        /// </summary>
        private async void TestUARTConnection()
        {
            ChipUARTResponding = await _CodenameService.TestUARTConnection();
        }
    }
}
