using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Widget;using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

using FFImageLoading.Forms.Droid;
using Newtonsoft.Json;

using QXFUtilities;

using MS.IoT.Mobile;
using MS.IoT.Mobile.Services.Authentication;
using MS.IoT.Mobile.Helpers;



namespace MS.IoT.Mobile.Android
{
    [Activity(Label = "MS.IoT.Mobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public static Context Instance { get; private set; }

        protected override async void OnCreate(Bundle bundle)
        {

            Instance = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //Check for Notification Intents Goes Here
            if (Intent.Extras != null)
            {
                await OnPushNotificationReceived();
            }

            // initializations
            CachedImageRenderer.Init();
  //          CarouselViewRenderer.Init();

            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = false,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
                Logger = new CustomLogger(),
            };
            FFImageLoading.ImageService.Instance.Initialize(config);


            global::Xamarin.Forms.Forms.Init(this, bundle);
            //var assembly = System.Reflection.Assembly.Load(typeof(Xamarin.Forms.CarouselView).FullName);

            // This should be a new instance of the app being loaded
            // check to see if this is a result of a notification being clicked or a normal launch
            if (Intent.Extras?.GetString("PreviousAppState") == "ApplicationState.NotRunning.ToString()" &&
                            Intent.Extras?.GetString(nameof(QXFUtilities.Navigation.NavigationSource)) == QXFUtilities.Navigation.NavigationSource.FromNotification.ToString())
            {
                // Notification launch causing app launch
#if DEBUG
                Toast.MakeText(this, "**** App launched from notification ****  Navigation Source = " + QXFUtilities.Navigation.NavigationSource.FromNotification, ToastLength.Long).Show();
#endif
                LoadApplication(new App(QXFUtilities.Navigation.NavigationSource.FromNotification));
            }
            else
                // Normal app launch
#if DEBUG
                Toast.MakeText(this, "App launched Normally", ToastLength.Long).Show();
#endif
            LoadApplication(new App());

            AuthenticationService.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity);
            var state = BaseApplication.ApplicationState;
            bool test = true;
            /*
             //           Manually get FCM token - for testing. Throwing exception.
                        var token1 = MS.IoT.Mobile.Android.Services.Notifications.FirebaseRegistrationService.RegisterForToken(this);
                        var token2 = Firebase.Iid.FirebaseInstanceId.Instance?.Token;
                        bool test = true;
            */
            /*
            global::Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            */
        }


        // Required for MSAL / B2C Authentication redirection
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
            var state = BaseApplication.ApplicationState;
            bool test = true;
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Check if the current item id equals the back button id = 16908332
            if (item.ItemId == 16908332)  // Android.Resource.Id.
            {
                // Retrieve the current Xamarin forms page instance
                var currentpage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault() as BaseContentPage;

                // check if page has subscribed to the custom back button event
                if (currentpage?.EnableBackButtonOverride != null && (bool)currentpage?.EnableBackButtonOverride && currentpage?.BackButtonAction != null)
                {
                    // Invoke the back button action
                    currentpage?.BackButtonAction.Invoke();
                    // Disable the default back button action
                    return false;
                }

                // If its not subscribed then invoke the default back button action
                return base.OnOptionsItemSelected(item);
            }
            else
            {
                // Not the back button click, pass the event to the base
                return base.OnOptionsItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            // Android has both Navigation bar back button and physical back button.
            // Cover both scenarios for safety 

            // Retrieve the current Xamarin forms page instance
            var currentpage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault() as BaseContentPage;

            // check if page has subscribed to the custom back button event
            if (currentpage?.BackButtonAction != null)
            {
                // Invoke the back button action
                currentpage?.BackButtonAction.Invoke();
            }
            else
            {
                // If its not subscribed then invoke the default back button action
                base.OnBackPressed();
            }
        }


        private async Task OnPushNotificationReceived()
        {
            await Task.Run(async () =>
            {
                var id = Intent.Extras.GetString("Id");
                global::Android.Util.Log.Debug("Push Notification Received", "Id: {0}", id);
            });
        }



        public class CustomLogger : FFImageLoading.Helpers.IMiniLogger
        {
            public void Debug(string message)
            {
                Console.WriteLine(message);
            }

            public void Error(string errorMessage)
            {
                Console.WriteLine(errorMessage);
            }

            public void Error(string errorMessage, Exception ex)
            {
                Error(errorMessage + System.Environment.NewLine + ex.ToString());
            }
        }


        //TODO Remove this when fixed by Xamarin
        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        protected async override void OnResume()
        {
            base.OnResume();


        }

        protected async override void OnRestart()
        {
            base.OnRestart();


        }


    }
}

