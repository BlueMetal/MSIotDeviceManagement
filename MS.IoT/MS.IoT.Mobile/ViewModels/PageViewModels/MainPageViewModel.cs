using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Microsoft.Identity.Client;

using Xamarin.Forms;

using Newtonsoft.Json.Linq;

using QXFUtilities;

using MS.IoT.Mobile.Services.DataServices;
using MS.IoT.Mobile.Helpers;


namespace MS.IoT.Mobile
{
    public class MainPageViewModel : ViewModelBase
    {

        public ICommand PageAppearing { get; set; }


        public ICommand NotificationIconClicked { get; set; }
        public ICommand DeviceSelected { get; set; }
        public ICommand ButtonClicked { get; set; }
        public ICommand ListItemTapped { get; set; }



        public static bool ReloadFeatures { get; set; } = false;

        public List<DeviceViewModel> DeviceList = new List<DeviceViewModel>();


        private int _previousDeviceSelection = -1;
        public int PreviousDeviceSelection
        {
            get { return _previousDeviceSelection; }
            private set
            {
                _previousDeviceSelection = value;
                OnPropertyChanged<int>();
            }
        }

        private static int _currentDeviceSelection = -1;
        public int CurrentDeviceSelection
        {
            get { return _currentDeviceSelection; }
            private set
            {
                PreviousDeviceSelection = _currentDeviceSelection;
                _currentDeviceSelection = value;
                CurrentDeviceId = DeviceCollection[value].DeviceId;
                OnPropertyChanged<int>();
                FeatureCollection = new ObservableCollection<FeaturesViewModel>(_fullFeatureLists[value].Where(f => !f.FeatureType.Equals(FeatureType.SubType) || f.Expanded));
            }
        }



        private int _numberDevices = 0;
        public int NumberDevices
        {
            get { return _numberDevices; }
            private set
            {
                _numberDevices = value;
                OnPropertyChanged<int>();
            }
        }

        private string _currentDeviceId = string.Empty;
        public string CurrentDeviceId
        {
            get { return _currentDeviceId; }
            private set
            {
                _currentDeviceId = value;
                OnPropertyChanged<string>();
            }
        }



        private string _notificationIconName = "notifications1.png";
        public string NotificationIconName
        {
            get { return _notificationIconName; }
            private set
            {
                _notificationIconName = value;
                OnPropertyChanged<string>();
            }
        }

        private ObservableCollection<DeviceViewModel> _deviceCollection = new ObservableCollection<DeviceViewModel>();
        public ObservableCollection<DeviceViewModel> DeviceCollection
        {
            get { return _deviceCollection; }
            set
            {
                _deviceCollection = value;
                OnPropertyChanged<ObservableCollection<DeviceViewModel>>();
            }
        }


        public static Dictionary<string, Tuple<object, Type>> Selections { get; private set; } = new Dictionary<string, Tuple<object, Type>>();
        public static void AddOrUpdateSelection(int parentIndex, object value, Type valueType)
        {
            FeaturesViewModel parent = _fullFeatureCollection[_currentDeviceSelection].Where(f => f.Index.Equals(parentIndex)).First();
            Selections[parent.Label] = new Tuple<object, Type>(value, valueType);
            // Update stored value
            Settings.Selections = Selections;
        }






        private ObservableCollection<FeaturesViewModel> _featureCollection = new ObservableCollection<FeaturesViewModel>();
        public ObservableCollection<FeaturesViewModel> FeatureCollection
        {
            get { return _featureCollection; }
            set
            {
                _featureCollection = value;
                OnPropertyChanged<ObservableCollection<FeaturesViewModel>>();
            }
        }


        // TODO Store feature collections in OrderedDictionary

        private static List<List<FeaturesViewModel>> _fullFeatureLists = new List<List<FeaturesViewModel>>(0);

        private static ObservableCollection<List<FeaturesViewModel>> _fullFeatureCollection = new ObservableCollection<List<FeaturesViewModel>>();
        public ObservableCollection<List<FeaturesViewModel>> FullFeatureCollection
        {
            get { return _fullFeatureCollection; }
            set
            {
                _fullFeatureCollection = value;
                OnPropertyChanged<ObservableCollection<List<FeaturesViewModel>>>();
            }
        }


        public MainPageViewModel(INavigation navigation = null)
        {
            this.Navigation = navigation;
            NotificationIconClicked = new Command(async () => await OnNotificationIconClicked());
            PageAppearing = new Command(async () => await OnPageAppearing());
            DeviceSelected = new Command<int>((i) => OnDeviceSelectionChanged(i));
            ButtonClicked = new Command<string>(async (s) => await OnButtonClicked(s));
            ListItemTapped = new Command<FeaturesViewModel>(async (vm) => await OnListItemTapped(vm));

            Settings.NotificationsChangedEvent += OnNotificationsChanged;
            Settings.DevicesChangedEvent += OnDevicesChanged;

            // load the devices
            LoadDevices();
            LoadFeatures(DeviceList);
 
            Selections = Settings.Selections;


        }

        private void LoadDevices()
        {
            // get the current registered devices from storage 
            // (note: if demo this should have been padded with dummy devices to ensure at least 3
            var registeredDevices = Settings.RegisteredDevices;
            // Generate View Models for devices and assign to Device Collection
            DeviceList = DeviceViewModel.GetViewModels(registeredDevices);
            NumberDevices = DeviceList.Count;
            DeviceCollection = new ObservableCollection<DeviceViewModel>(DeviceList);
        }


        private void LoadFeatures(List<DeviceViewModel> deviceList = null)
        {
            if (deviceList == null)
            {
                var registeredDevices = Settings.RegisteredDevices;
                // Generate View Models for devices and assign to Device Collection
                deviceList = DeviceViewModel.GetViewModels(registeredDevices);
                NumberDevices = deviceList.Count;
            }
            int index = 0;

            if (deviceList != null)
            {
                _fullFeatureLists = new List<List<FeaturesViewModel>> (0);
                foreach (DeviceViewModel device in deviceList)
                {
                    var fullFeatureList = FeaturesViewModel.GetFeaturesViewModels(device.DeviceFeatures, this);
                    _fullFeatureLists.Add(fullFeatureList);
                }
                FullFeatureCollection = new ObservableCollection<List<FeaturesViewModel>>(_fullFeatureLists);
                if (deviceList.Count > 0)
                {
                    if (CurrentDeviceSelection == -1)
                    {
                        index = (int)Math.Round((deviceList.Count - 1.0) / 2, 1);
                        CurrentDeviceSelection = index;  // will load Feature Collection
                    }
                    else
                    {
                        index = CurrentDeviceSelection;
                        FeatureCollection = new ObservableCollection<FeaturesViewModel>(_fullFeatureLists[index].Where(f => !f.FeatureType.Equals(FeatureType.SubType) || f.Expanded));
                    }
                }

            }
        }

        private async Task LoadFeaturesAsync(List<DeviceViewModel> deviceList = null)
        {
            await Task.Run(() => {
                LoadFeatures(deviceList);
            });
        }


            private async Task OnPageAppearing()
        {
            // Set up the correct notification icon in the toolbar
            NotificationIconName = Settings.NumberUnreadNotifications == 0 ? "notifications1.png" : "notifications2.png";

            if (ReloadFeatures)
                await LoadFeaturesAsync();
            ReloadFeatures = false;
        }

        private async Task OnButtonClicked(string id)
        {

            // Ascertain which button was clicked, then process
            switch (id)
            {


                default:
                    break;
            }
            return;
        }


        public void OnDeviceSelectionChanged(int newSelectionIndex)
        {
            if (CurrentDeviceSelection > -1)
                // There is already a device selected so reduce its scale
                DeviceCollection[CurrentDeviceSelection].IconScale = 0.7;
            DeviceCollection[newSelectionIndex].IconScale = 1.0;
            CurrentDeviceSelection = newSelectionIndex;
        }


        public async Task OnListItemTapped(FeaturesViewModel vm)
        {

            // Check if the Feature is an Action
            if (vm.FeatureType == FeatureType.Action)
            {
                // NOTE: Currently hard coded to supported actions which will require a new version of the app to support new actions. 
                // The server view models need to be changed to support the abstract definition of actions so they can be cerated and supported
                // dynamically without a change to the code
                vm.IsBusy = !vm.IsBusy;
                // persist the change in busy status
                FullFeatureCollection[CurrentDeviceSelection][vm.Index].IsBusy = vm.IsBusy;
                if (vm.FeatureName.Contains("brew") || vm.FeatureName.Contains("Brew"))
                {
                    if (vm.IsBusy)
                    {
                        // if it is now busy, invoke the actions

                        // set brew strength 
                        // try to get the brew strength
                        if (!Selections.TryGetValue("Brew Strength", out Tuple<object, Type> brewStrength))
                            brewStrength = new Tuple<object, Type>(1, typeof(int));
                        var response1 = await IoTHubDataService.Instance.InvokeFeature(CurrentDeviceId, "changeBrewStrength", Convert.ToString(brewStrength.Item1));
                        if (response1 == null)
                        {
                            vm.IsBusy = false;
                            await App.Current.MainPage.DisplayAlert("Sorry", "Your Device isn't responding. \nPlease ensure it has power and is connected, and try again.", "Ok");
                        }
                        else if (response1.Status.ToLower() != "success")
                        {
                            vm.IsBusy = false;
                            await App.Current.MainPage.DisplayAlert("Sorry", "Your Device has sent an error. \nPlease check it and try again", "Ok");
                        }
                        else
                        {
                            // Invoke brew related feature action 
                            var response2 = await IoTHubDataService.Instance.InvokeFeature(CurrentDeviceId, vm.FeatureMethodName, vm.FeatureMethodParameter);
                            // if it was actioned successfully
                            if (response2 == null)
                            {
                                vm.IsBusy = false;
                                await App.Current.MainPage.DisplayAlert("Sorry", "Your Device isn't responding. \nPlease ensure it has power and is connected, and try again.", "Ok");
                            }
                            else if (response2.Status.ToLower() != "success")
                            {
                                vm.IsBusy = false;
                                await App.Current.MainPage.DisplayAlert("Sorry", "Your Device has sent an error. \nPlease check it and try again", "Ok");
                            }
                            else
                            {
                                //set an asynchronous timer based upon the returned DateTimeStamp
                                var UTCCompleteTime = response2.TimeTillCompletion;
                                var currentUTCTime = DateTime.Now.ToUniversalTime();
                                var completeInMs = UTCCompleteTime.Subtract(currentUTCTime).TotalMilliseconds > 0 ? UTCCompleteTime.Subtract(currentUTCTime).TotalMilliseconds : 0;

                                TimerStateObjClass StateObj = new TimerStateObjClass { Vm = vm, SelectedDevice = CurrentDeviceSelection };
                                // Could move Timer to be a field of the deviceViewModel
                                TimerCallback TimerDelegate = new System.Threading.TimerCallback((o) =>
                                {
                                    var state = (TimerStateObjClass)o;
                                    state.Vm.IsBusy = false;
                                    // Persist the busy state
                                    FullFeatureCollection[state.SelectedDevice][state.Vm.Index].IsBusy = false;
                                    state.TimerReference.Dispose();
                                });
                                vm.Timer = new Timer(TimerDelegate, StateObj, (int)completeInMs + 2000, Timeout.Infinite);
                                StateObj.TimerReference = vm.Timer;
                            }
                        }
                    }
                    else
                    {
                        // Cancel the action
                        var response2 = await IoTHubDataService.Instance.InvokeFeature(CurrentDeviceId, vm.FeatureMethodName, vm.FeatureMethodParameter);
                        if (vm.Timer != null)
                            vm.Timer.Dispose();
                    }
                }
                else
                {
                    // Invoke feature action - manipulate Is busy as necessary
                    var response2 = await IoTHubDataService.Instance.InvokeFeature(CurrentDeviceId, vm.FeatureMethodName, vm.FeatureMethodParameter);
                    vm.IsBusy = false;
                }
                // persist the resultant busy state
                FullFeatureCollection[CurrentDeviceSelection][vm.Index].IsBusy = vm.IsBusy;
            }

        }


        private async Task SignOutAsync()
        {
            foreach (var user in App.AuthenticationService.PublicClientApplication.Users)
            {
                App.AuthenticationService.PublicClientApplication.Remove(user);
            }
        }


        private async Task<bool> PasswordResetAsync()
        {
            // Do nothing - not supported in this demo
            /*
                        try
                        {
                            AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, (IUser)null, UIBehavior.SelectAccount, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                            UpdateUserInfo(ar);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // Alert if any exception excluding user cancelling sign-in dialog
                            if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                                await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                            return false;
                        }
            */
            return false;
        }


        private async Task OnNotificationIconClicked()
        {
            // Navigate to Notifications Screen
            await NavigationManager.Navigate(Navigation, nameof(MainPageViewModel), NavigationManager.NotificationsButton);
        }
        private void OnNotificationsChanged(object sender, NotificationsChangedEventArgs e)
        {
            // change the icon
            NotificationIconName = Settings.NumberUnreadNotifications == 0 ? "notifications1.png" : "notifications2.png";
        }

        private void OnDevicesChanged(object sender, DevicesChangedEventArgs e)
        {
            if (e.OldCount != e.NewCount)
            {
                // load the devices
                LoadDevices();

                LoadFeatures(DeviceList);

                Selections = Settings.Selections;
            }
        }

        public static async Task OnGrind(object strength)
        {
            // Post to endpoint


        }

        public static async Task OnBrew(object size)
        {
            // Post to endpoint


        }

        public static async Task OnGrindBrew(object strengthAndSize)
        {
            // Post to endpoint


        }

        private class TimerStateObjClass
        {
            // Used to hold parameters for calls to TimerTask.
            public int SelectedDevice = -1;
            public FeaturesViewModel Vm;
            public System.Threading.Timer TimerReference;
        }


    }
}
