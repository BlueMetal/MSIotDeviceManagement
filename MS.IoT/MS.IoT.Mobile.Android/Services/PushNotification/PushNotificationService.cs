using System;
using System.Threading.Tasks;
using Android.Preferences;
using Firebase.Iid;
using Android.App;

using MS.IoT.Mobile.Services.Notifications;


[assembly: Xamarin.Forms.Dependency(typeof(MS.IoT.Mobile.Android.Services.Notifications.PushNotificationService))]
namespace MS.IoT.Mobile.Android.Services.Notifications
{
    
    public class PushNotificationService : IPushNotificationService
    {
        private const string INSTALLATION_ID_KEY = "nhub_install_id";

        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            var installationId = this.GetInstallationId();

            if (FirebaseInstanceId.Instance.Token == null)
                return null;

            var installation = new DeviceInstallation
            {
                InstallationId = installationId,
                Platform = "gcm",
                PushChannel = FirebaseInstanceId.Instance.Token
            };
            // Set up tags to request
            installation.Tags.AddRange(tags);
            // Set up templates to request
            PushTemplate genericTemplate = new PushTemplate
            {
                Body = @"{""data"":{""message"":""$(messageParam)""}}"
            };
            PushTemplate silentTemplate = new PushTemplate
            {
                Body = @"{""data"":{""message"":""$(silentMessageParam)"", ""action"":""$(actionParam)"", ""silent"":""true""}}"
            };
            installation.Templates.Add("genericTemplate", genericTemplate);
            installation.Templates.Add("silentTemplate", silentTemplate);

            return installation;
        }

        public string GetDeviceId()
        {
            return this.GetInstallationId();
        }

        private string GetInstallationId()
        {
            var installationId = string.Empty;

            // Use shared preferences to store Notification Hub InstallationId for reuse between app sessions
            var prefs = PreferenceManager.GetDefaultSharedPreferences(MainActivity.Instance);
            installationId = prefs.GetString(INSTALLATION_ID_KEY, string.Empty);

            if (string.IsNullOrWhiteSpace(installationId))
            {
                installationId = Guid.NewGuid().ToString();

                var editor = prefs.Edit();
                editor.PutString(INSTALLATION_ID_KEY, installationId);
                editor.Apply();

                // If we change the installation id, we need to invalidate the current Firebase Instance Token
                // so that we can deregister from the current installationid/firebasetoken combination
                // This will force a refresh of the Firebase instance token. When the token is available,
                // The FirebaseIIDService will process the refresh with NH
                FirebaseInstanceId.Instance.DeleteInstanceId();
            }

            return installationId;
        }
    }
}
