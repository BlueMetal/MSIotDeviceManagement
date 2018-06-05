using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;

using Microsoft.Identity.Client;

using Xamarin.Forms;

using Newtonsoft.Json.Linq;

using QXFUtilities;
using QXFUtilities.Navigation;

using MS.IoT.Mobile.Services.Authentication;
using MS.IoT.Mobile.Helpers;
using MS.IoT.Mobile.Services.Notifications;
using MS.IoT.Mobile.Services.DataServices;


namespace MS.IoT.Mobile
{
    public class SplashScreenViewModel : ViewModelBase
    {
        private const bool USE_AUTHENTICATION = true;  //For testing only

        private bool _authenticating = false;
        public bool Authenticating
        {
            get { return _authenticating; }
            private set
            {
                _authenticating = value;
                OnPropertyChanged<bool>();
            }
        }

        public static NavigationSource NavigationSource { get; set; } = NavigationSource.Normal;

        public ICommand PageAppearing { get; set; }
        public ICommand ButtonClicked { get; set; }



        private bool _buttonsVisibile = false;
        public bool ButtonsVisible
        {
            get { return _buttonsVisibile; }
            private set
            {
                _buttonsVisibile = value;
                OnPropertyChanged<bool>();
            }
        }

        private string _buttonText = "GET STARTED";
        public string ButtonText
        {
            get { return _buttonText; }
            private set
            {
                _buttonText = value;
                OnPropertyChanged<string>();
            }
        }

        public SplashScreenViewModel()
        {
            PageAppearing = new Command(async () => await OnPageAppearing());
            ButtonClicked = new Command<string>(async (s) => await OnButtonClicked(s));
        }
        public SplashScreenViewModel(NavigationSource navigationSource)
        {
            PageAppearing = new Command(async () => await OnPageAppearing());
            ButtonClicked = new Command<string>(async (s) => await OnButtonClicked(s));
            NavigationSource = navigationSource;
        }

        private async Task OnPageAppearing()
        {

            // Check to see if authentication in process - else can get called twice
            if (USE_AUTHENTICATION && !Authenticating)
            {

                if (await TryAuthenticateSilentAsync())
                {
                    // Successfully authenticated
                    // Note: This should be moved to after Navigation call, however
                    // a bug in the Syncfusion Carousel control causes it not layout
                    // correctly if laid out from a source list with no data and then 
                    // the list is updated with data.
                    await UpdateRegisteredDeviceRegistration();

                    // Navigate to main page
                    await AuthenticatedNavigationAsync();

                    Authenticating = false;
                }
                else
                {
                    // login failed
                    Authenticating = false;
                    // Display Button to logon
                    ButtonsVisible = true;
                }

            }
        }


        private async Task OnButtonClicked(string id)
        {

            // Ascertain which button was clicked, then process
            switch (id)
            {
                case "Logon":
                    await LogonButtonClicked();
                    break;

                case "TandC":

                    break;

                default:
                    break;
            }
            return;
        }

        private async Task LogonButtonClicked()
        {
            // Disable button
            ButtonsVisible = false;
            if (USE_AUTHENTICATION)
            {
                if (await TryAuthenticateWithCredentialsAsync())
                {
                    // Successfully authenticated

                    await UpdateRegisteredDeviceRegistration();
                    // Note: This should be moved to after Navigation call, however
                    // a bug in the Syncfusion Carousel control causes it not layout
                    // correctly if laid out from a source list with no data and then 
                    // the list is updated with data.
                    // Navigate to main page
                    await AuthenticatedNavigationAsync();

                    Authenticating = false;
                }
                else
                {
                    // login failed
                    Authenticating = false;

                    // Display Login error message
                    await App.Current.MainPage.DisplayAlert("Login Failed", string.Empty, "Ok");

                    // Enable Logon Button
                    ButtonText = "LOGON";
                    ButtonsVisible = true;
                }
            }
            else
            {
                // Not Authenticating (for testing)
                await AuthenticatedNavigationAsync();
            }
        }



        private async Task<bool> TryAuthenticateSilentAsync()
        {
            Authenticating = true;
            try
            {
                // Try to authenticate
                AuthenticationResult ar = await App.AuthenticationService.AuthenticateSilentlyAsync(AuthenticationSettings.Scopes);

                // Extract information
                var authenticationInfo = App.AuthenticationService.GetAuthenticationInfo(ar);
                var userInfo = App.AuthenticationService.GetUserInfo(ar);


                // Temporary - will store in settings or secure Key Store
                Settings.UserId = ar.UniqueId;
                App.AuthenticationService.UniqueUserId = ar.UniqueId;
                App.AuthenticationService.AddAccessToken("ReadAndWrite", ar.AccessToken);


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> TryAuthenticateWithCredentialsAsync()
        {
            Authenticating = true;
            try
            {
                // Try to authenticate
                AuthenticationResult ar = await App.AuthenticationService.AuthenticateWithCredentialsAsync(AuthenticationSettings.Scopes);
                // Extract information
                var authenticationInfo = App.AuthenticationService.GetAuthenticationInfo(ar);
                var userInfo = App.AuthenticationService.GetUserInfo(ar);


                // Temporary - will store in settings or secure Key Store
                Settings.UserId = ar.UniqueId;
                App.AuthenticationService.UniqueUserId = ar.UniqueId;
                App.AuthenticationService.AddAccessToken("ReadAndWrite", ar.AccessToken);


                return true;
            }
            catch (Exception ex)
            {
                // Checking the exception message should ONLY be done for B2C reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    await PasswordResetAsync();
                // Alert if any exception excluding user cancelling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await App.Current.MainPage.DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                return false;
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


        private async Task UpdateRegisteredDeviceRegistration()
        {
            var deviceIds = new List<string>();
            var devices = new List<Device>();
            var notifications = new List<Notification>();
            // Register devices as tags to receive notifications
            // Get Device Installation from local storage
            var deviceInstallation = Settings.DeviceInstallation;

            if (USE_AUTHENTICATION)
            {
                //Get registered Devices from server
                var rawDevices = await IoTHubDataService.Instance.GetRegisteredDevicesAsync();
                //Store for reference
                Settings.RawRegisteredDevices = rawDevices;

                // Extract and store registered devices and Device IDs
                if (rawDevices != null)
                {
                    foreach (ReturnedDevice rawDevice in rawDevices)
                    {
                        Device device = new Device(rawDevice);
                        devices.Add(device);
                        deviceIds.Add(rawDevice.DeviceId);
                    }
                }
                // Store Device IDs in local storage
                if (deviceIds.Count > 0)
                    Settings.RegisteredDeviceIds = deviceIds;
            }

#if DEMO
            // Add Demo Devices - don't check to see if demo devices already added
            // as devices fetched each time and persisted store is overwritten,
            // so will need to add demo device each time
            DummyDemoData.AddDemoDevices(devices);
#endif

            // Store Devices in local storage
            if (devices.Count > 0)
                Settings.RegisteredDevices = devices;


#if DEMO
            //Get Demo Notifications
            if (!Settings.DemoNotificationsAdded)
                notifications = DummyDemoData.GetDemoNotifications();

            // Get any previously stored notifications from local storage
            List<Notification> _notifications = Settings.Notifications;
            if (_notifications?.Count > 0)
                // Add into notifications list
                notifications.AddRange(_notifications);

            // Store notifications
            Settings.Notifications = notifications;
#endif

#if TEST1
            // For Testing
            await App.Current.MainPage.DisplayAlert("Registered Device Ids returned from server:\n", String.Join(";\n", deviceIds), "Ok");
#endif

            // if deviceInstallation exists, register the tags for it
            if (deviceInstallation != null)
            {
                // RegisterDevice with Notification Hub to update tags with deviceIds
                deviceIds.Add("Test");
                await NotificationRegistrationService.Instance.RegisterDeviceAsync(deviceInstallation, deviceIds);
            }

        }


        private async Task AuthenticatedNavigationAsync()
        {
            await NavigationManager.Navigate(Navigation, nameof(SplashScreenViewModel), NavigationManager.AuthenticationSuccess, NavigationSource);
            NavigationSource = NavigationSource.Normal;
        }


    }
}
