using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MS.IoT.Mobile.Android
{
    [Activity(Label = "NotificationNavigationActivity")]
    public class NotificationNavigationActivity : Activity // global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var test = IsTaskRoot;

            //Check for Notification Intents Goes Here
            if (Intent.Extras != null)
            {
                //
            }

            // Check the Current App Status
            switch (BaseApplication.ApplicationState)
            {

                case (ApplicationState.NotRunning):
                    // App is not running so need to launch app
                    // Launch Main Activity, navigate to Notifications List Page, and insert Main Page into navigation stack 
                    // (note: if we add in a separate SplashscreenActivty to improve the user experience, will need to launch that instead)
                    var launchActivity = new Intent(this, typeof(MainActivity));
                    launchActivity.PutExtra("PreviousAppState", ApplicationState.NotRunning.ToString());
                    launchActivity.PutExtra(nameof(QXFUtilities.Navigation.NavigationSource), QXFUtilities.Navigation.NavigationSource.FromNotification.ToString());
                    launchActivity.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
                    StartActivity(launchActivity);
                    break;

                case (ApplicationState.Background):
                    // Need to bring app to foreground and navigate to Notifications List page.
                    if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault().GetType() != typeof(NotificationsPage))
                        App.Current.MainPage.Navigation.PushAsync(new NotificationsPage());
                    break;

                case (ApplicationState.Foreground):
                    // Navigate to Notifications List page by pushing the page onto the navigation stack
                    // If already on notification page, do nothing else navigate to notification list page
                    if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault().GetType() != typeof(NotificationsPage))
                        App.Current.MainPage.Navigation.PushAsync(new NotificationsPage());
                    break;

                default:
                    // not defined
                    break;
            }

            // Close this activity
            Finish();

        }


    }
}