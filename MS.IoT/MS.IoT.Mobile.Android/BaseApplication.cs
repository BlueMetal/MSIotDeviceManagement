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
    [Application]
    public class BaseApplication : Application , Application.IActivityLifecycleCallbacks
    {

        public int NumberActivitiesActive { get; private set; }

        public static ApplicationState ApplicationState { get; private set; } = ApplicationState.NotRunning;

        public static DateTime AppEnteredTime { get; private set; } = DateTime.MinValue;
        public static DateTime AppLeftTime { get; private set; } = DateTime.MinValue;

        public static double TimeInAppMs
        {
            get {
                if (AppLeftTime == DateTime.MinValue)
                    return DateTime.Now.Subtract(AppEnteredTime).TotalMilliseconds;
                else
                    return AppLeftTime.Subtract(AppEnteredTime).TotalMilliseconds;
            }
        }


        public BaseApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            ApplicationState = ApplicationState.NotRunning;
        }

        void IActivityLifecycleCallbacks.OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        void IActivityLifecycleCallbacks.OnActivityDestroyed(Activity activity) // Exclude this activity because it only relates to a notification created in response to a  push notification
        {
            if (activity.LocalClassName != "NotificationNavigationActivity")
            {
                if (NumberActivitiesActive <= 0)
                {
                    ApplicationState = ApplicationState.NotRunning;
                }
            }
        }

        void IActivityLifecycleCallbacks.OnActivityPaused(Activity activity)
        {
        }

        void IActivityLifecycleCallbacks.OnActivityResumed(Activity activity)
        {
        }

        void IActivityLifecycleCallbacks.OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        void IActivityLifecycleCallbacks.OnActivityStarted(Activity activity)
        {
            if (activity.LocalClassName != "NotificationNavigationActivity")  // Exclude this activity because it only relates to a notification created in response to a  push notification
            {
                if (NumberActivitiesActive == 0)
                {
                     AppEnteredTime = DateTime.Now;
                    ApplicationState = ApplicationState.Foreground;
                }
                NumberActivitiesActive++;
            }
        }

        void IActivityLifecycleCallbacks.OnActivityStopped(Activity activity)
        {
            if (activity.LocalClassName != "NotificationNavigationActivity") // Exclude this activity because it only relates to a notification created in response to a  push notification
            {
                NumberActivitiesActive--;
                if (NumberActivitiesActive == 0)
                {
                    AppLeftTime = DateTime.Now;
                    if (activity.IsFinishing)
                        ApplicationState = ApplicationState.NotRunning;
                    else
                        ApplicationState = ApplicationState.Background;
                }
            }
        }
    }

    public enum ApplicationState
    {
        Background,
        Foreground,
        NotRunning,
        Unknown
    }

    public enum NavigationSource
    {
        Normal,
        FromNotification
    }

}