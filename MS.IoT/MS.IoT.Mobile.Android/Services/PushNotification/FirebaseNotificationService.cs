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
using Android.Media;
using Android.Support.V4.App;
using Android.Util;

using Firebase.Messaging;
using Newtonsoft.Json;

using MS.IoT.Mobile.Helpers;


namespace MS.IoT.Mobile.Android.Services.Notifications
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseNotificationService : FirebaseMessagingService
    {
        const string TAG = "FirebaseNotificationService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            // Get the Apps current application state
            var appStatet = BaseApplication.ApplicationState;


            if (message.Data.ContainsKey("silent"))
            {
                var action = message.Data["action"];
                Log.Debug(TAG, $"Notification Message Action: {action}");
                PerformSilentNotification(action);
            }
            else
            {
                // Pull message body out of the template
                string messageBody = string.Empty;
                if (!message.Data.TryGetValue("message", out messageBody))
                    return;

                if (string.IsNullOrWhiteSpace(messageBody))
                    return;

                Log.Debug(TAG, "Notification message body: " + messageBody);

                // Store Notification
                string id = Guid.NewGuid().ToString();
                StoreNotification(messageBody, id);

                // Send Notification
                SendLocalNotification(messageBody, id);
            }

        }

        private void SendLocalNotification(string messageBody, string id)
        {
            var intent = new Intent(this, typeof(NotificationNavigationActivity));
            intent.AddFlags(ActivityFlags.PreviousIsTop);//.ClearTop);
            intent.PutExtra("Notification Type", "Notification");
            intent.PutExtra("Id", id);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle("New Feature Available")
                .SetTicker("New Feature Available")
                .SetContentText("New Feature Available")
                .SetContentIntent(pendingIntent)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0 /* ID of Notification */, notificationBuilder.Build());

        }



        private void PerformSilentNotification(string action)
        {
            System.Diagnostics.Debug.WriteLine($"[PNS] Perform action of type: {action}");

            // put supported actions here via switch statement



        }


        private void StoreNotification(string message, string id)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Extract information from message and add to notifications list
                var dtoNotification = JsonConvert.DeserializeObject<PushNotificationDto>(message);
                // store notification
                Notification notification = new Notification(dtoNotification) { Id = id }; // Guid.NewGuid().ToString() };

                Settings.AddOrUpdateNotification(notification);
            }
        }


    }
}