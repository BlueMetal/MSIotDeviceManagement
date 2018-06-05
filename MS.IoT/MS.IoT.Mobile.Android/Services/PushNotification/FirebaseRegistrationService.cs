using System;
using Android.App;
using Android.Content;
using Android.Util;

using Firebase.Iid;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Messaging;

using MS.IoT.Mobile.Services.Notifications;
using Android.Widget;

namespace MS.IoT.Mobile.Android.Services.Notifications
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseRegistrationService : FirebaseInstanceIdService
    {
        private const string PROJECT_ID = "msiotdev-d8133";  //  Manually copy from Firebase console OR google-services.json
        private const string SCOPE = "GCM";


        const string TAG = "FirebaseRegistrationService";

        const string TestTag = "Test";

        public List<string> SubscribedTopics { get; private set; } = new List<string>();

        /**
         * Called when:
            The app is installed or uninstalled.
            The user deletes app data.
            The app erases the Instance ID.
            The security of the token has been compromised.
         */
        public override void OnTokenRefresh()
        {
            // Get updated InstanceID token.
            
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);

            // TODO: Implement this method to send any registration to your app's servers.
            SendRegistrationTokenToServer();
        }


/*
        // TODO: Manually get token - currently throwing exception
        public static string RegisterForToken(Activity activity, bool forceNew = false)
        {
            string token = null;
            if (!forceNew)
            {
                try
                {
                    token = FirebaseInstanceId.Instance?.Token;
                }
                catch (Exception e)
                {
                    bool test = true;
                }
            }

            var bld = new Firebase.FirebaseOptions.Builder();
            bld.SetApiKey("AIzaSyA7105wI5XsSC4-19E2bMghBDK3nsKkZbY");
            bld.SetApplicationId("1:114097432181:android:32dd05715f2d01bf");
            bld.SetDatabaseUrl("https://msiotdev-d8133.firebaseio.com");
            bld.SetGcmSenderId("114097432181");
            bld.SetStorageBucket("msiotdev-d8133.appspot.com");
            var options = bld.Build();

            while (token == null)
            {
                var fba = Firebase.FirebaseApp.Instance;
                if (fba == null)
                {
                    try
                    {
                        fba = Firebase.FirebaseApp.InitializeApp(activity.ApplicationContext, options);
                    }
                    catch (Exception ex1)
                    {
                        bool test1 = true;
                    }
                }
                        
                var IIdInstance = FirebaseInstanceId.GetInstance(fba);
                try
                {
                    token = IIdInstance.GetToken(PROJECT_ID, SCOPE);
                }
                catch (Exception ex)
                {
                    bool test = true;
                }
            }
            return token;
        }
*/


        /**
         * Persist token to third-party servers.
         *
         * Modify this method to associate the user's FCM InstanceID token with any server-side account
         * maintained by your application.
         */
        void SendRegistrationTokenToServer()
        {

            // Update notification hub registration
            Task.Run(async () =>
            {
                await NotificationRegistrationService.Instance.RegisterDeviceAsync();
            });
        }

        public void SubscribeToNotificationTopic(string topic)
        {
            if (SubscribedTopics.Contains(topic))
            {
                global::Android.Util.Log.Debug("ConfigurePushNotifications Push Notifications", topic + " topic already subscribed to");
            }
            else
            {
                SubscribedTopics.Add(topic);
                FirebaseMessaging.Instance.SubscribeToTopic(topic);
                global::Android.Util.Log.Debug("ConfigurePushNotifications Push Notifications", "Subscribed to " + topic + " topic");
            }
        }
        public void SubscribeToNotificationTopics(List<string> topics)
        {
            foreach (string topic in topics)
            {
                SubscribeToNotificationTopic(topic);
            }
        }

        public void UnSubscribeFromNotificationTopic(string topic)
        {
            if (SubscribedTopics.Contains(topic))
            {
                SubscribedTopics.Remove(topic);
                FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                global::Android.Util.Log.Debug("ConfigurePushNotifications Push Notifications", "Unsubscribed from " + topic + " topic");
            }
        }
        public void UnSubscribeFromNotificationTopics(List<string> topics)
        {
            foreach (string topic in topics)
            {
                UnSubscribeFromNotificationTopic(topic);
            }
        }
        public void UnSubscribeFromAllNotificationTopics()
        {
            foreach (string topic in SubscribedTopics)
            {
                FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                global::Android.Util.Log.Debug("ConfigurePushNotifications Push Notifications", "Unsubscribed from " + topic + " topic");
            }
            SubscribedTopics.Clear();
        }

    }
}