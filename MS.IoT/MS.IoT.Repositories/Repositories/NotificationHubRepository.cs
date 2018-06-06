using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;

namespace MS.IoT.Repositories
{
    public class NotificationHubRepository : INotificationHubRepository
    {
        private static string _notificationHubConnectionString;
        private static NotificationHubClient _hubClient;

        public NotificationHubRepository(string notificationHubConnectionString)
        {
            _notificationHubConnectionString = notificationHubConnectionString;
            _hubClient= NotificationHubClient
                                .CreateClientFromConnectionString(notificationHubConnectionString, "msiot-notificationhub-dev",true);
        }


        public async Task<Boolean> RegisterMobileDevice(MobileDeviceInstallationNotificationHub deviceUpdate)
        {
            Dictionary<string, InstallationTemplate> templates = new Dictionary<string, InstallationTemplate>();
            foreach (var t in deviceUpdate.Templates)
            {
                templates.Add(t.Key, new InstallationTemplate { Body = t.Value.Body });
            }
            Installation installation = new Installation()
            {
                InstallationId = deviceUpdate.InstallationId,
                PushChannel = deviceUpdate.PushChannel,
                Tags = deviceUpdate.Tags,
                Templates = templates
            };
            switch (deviceUpdate.Platform)
            {
                case "apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "gcm":
                    installation.Platform = NotificationPlatform.Gcm;
                    break;
                default:
                    throw new ArgumentException("Bad Request"); 
            }
            installation.Tags = new List<string>(deviceUpdate.Tags);
            try
            {
                await _hubClient.CreateOrUpdateInstallationAsync(installation);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDeviceInstallation(string id)
        {
            try
            {
                await _hubClient.DeleteInstallationAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NotificationOutcome> PushNotification(PushNotificationRequest pushRequest)
        {          
            // Sending the message so that all template registrations that contain "messageParam", "silentMessageParam", or "actionParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();

            if (pushRequest.Silent)
            {
                templateParams["silentMessageParam"] = "1";
                templateParams["actionParam"] = pushRequest.Action;
            }
            else
            {
                templateParams["message"] = pushRequest.Text;
            }
            try
            {
                // Send the push notification and log the results.
                var result = await _hubClient.SendTemplateNotificationAsync(templateParams, string.Join(" || ", pushRequest.Tags));
                return result;
            }
            catch (Exception ex)
            {             
                throw ex;
            }
        }

        public async Task<NotificationOutcome> SendNotification(string payload, string tag)
        {
            try
            {
                var response = await _hubClient.SendGcmNativeNotificationAsync(payload, tag);
                return response;
            }
            catch (Exception e)
            {
                Log.Error("Send Notification error {@error}", e.Message);
                throw e;
            }
        }

        public async Task SendNotificationMultipleDevices(string payload,List<string> tags)
        {
            try
            {              
                var splitTagsList = SplitList(tags, 20);
                // mag taglist size is 20
                foreach (var tagList in splitTagsList)
                {
                    var response=await _hubClient.SendGcmNativeNotificationAsync(payload, tagList);
                }           
            }
            catch (Exception e)
            {
                Log.Error("Send Notification error {@error}", e.Message);
                throw e;
            }
        }

        private static List<List<string>> SplitList(List<string> tags, int size)
        {
            var list = new List<List<string>>();
            for (int i = 0; i < tags.Count; i += size)
                list.Add(tags.GetRange(i, Math.Min(size, tags.Count - i)));
            return list;
        }
    }
}
