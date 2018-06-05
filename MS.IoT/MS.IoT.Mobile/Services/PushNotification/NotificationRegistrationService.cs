using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using QXFUtilities.Communication;
using MS.IoT.Mobile.Helpers;

namespace MS.IoT.Mobile.Services.Notifications
{
    public class NotificationRegistrationService : HttpCommunicationService
    {

        private static string ServerBaseAddress = $"https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/";
        private static string RegisterNotificationTokenApiUrl = $"api/notification/register";

        private static NotificationRegistrationService instance;
        public static NotificationRegistrationService Instance => instance ?? (instance = new NotificationRegistrationService());

        private NotificationRegistrationService(): base()
        {
            Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
        }



        public Task<bool> RegisterDeviceAsync(params string[] tags)
        {
            // Resolve dependency with whatever IOC container
            var pushNotificationService = Xamarin.Forms.DependencyService.Get<IPushNotificationService>();

            // Get our registration information
            var deviceInstallation = pushNotificationService?.GetDeviceRegistration(tags);

            if (deviceInstallation == null)
                return Task.FromResult(true);


            // If user is authenticated
            if (Settings.IsAuthenticated)
            {
                // Authenticated so register with Tags top receive notifications
                // Get Device IDs from Local Storage  // Note: Max number of tags = 20?
                var currentTags = Settings.RegisteredDeviceIds;
                // Add the Test Tag
                currentTags.Add("Test");
                // Replace Tags in Device Installation with Device IDs and Test
                deviceInstallation.Tags = currentTags;
                // Save Device Installation to Local Storage
                Settings.DeviceInstallation = deviceInstallation;
            }
            else
            {
                // Not authenticated, so register without any tags so registered but wont receive active notifications
                deviceInstallation.Tags.Clear();
                // Save Device Installation to Local Storage
                Settings.DeviceInstallation = deviceInstallation;
            }

            // Put the device information to the server
            return PostAsync<bool, DeviceInstallation>(ServerBaseAddress, RegisterNotificationTokenApiUrl, deviceInstallation);
        }
        public Task<bool> RegisterDeviceAsync(DeviceInstallation deviceInstallation, List<string> tags)
        {
            if (deviceInstallation == null)
                return Task.FromResult(true);

            // Resolve existing tags from Device Installation
            deviceInstallation.Tags.Clear();
            // Add in new tags
            deviceInstallation.Tags = tags;

            // Put the device information to the server
            return PostAsync<bool, DeviceInstallation>(ServerBaseAddress, RegisterNotificationTokenApiUrl, deviceInstallation);
        }


        public Task DeregisterDeviceAsync()
        {
            var pushNotificationService = Xamarin.Forms.DependencyService.Get<IPushNotificationService>();

            // Get device installationId for notification hub
            var deviceId = pushNotificationService.GetDeviceId();

            if (deviceId == null)
                return Task.FromResult(false);

            // Delete that installation id from our NH
            return DeleteAsync<object>(ServerBaseAddress, $"api/register/{deviceId}");
        }
    }
}
