using System;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;
using System.Collections.Generic;
using Xunit;
using MS.IoT.Repositories;

public class NotificationHubRepositoryTests
    {
        public static readonly string notificationHubConString = "Endpoint=sb://msiot-notificationhub-dev.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=8xqIZQFAhSWRy27dEaKliaejZ8kDWuR4lgZO9um44Bo=";


        [Fact]
        public async Task register_device_notification_Hub()
        {
            NotificationHubRepository repo = new NotificationHubRepository(notificationHubConString);

            var registration = new MobileDeviceInstallationNotificationHub()
            {
                InstallationId = "instaltestid",
                Platform = "gcm",
                PushChannel = "testToken",
                Tags = new List<string> { "tag1", "tag2" },
                Templates = new System.Collections.Generic.Dictionary<string, NotificationPushTemplate>
                {
                    { "key1", new NotificationPushTemplate(){
                        Body="{\"data\":{\"message\":\"Notification Hub test notification\"}}"
                    } }
                }
            };

            await repo.RegisterMobileDevice(registration);
           // await repo.SendNotification("{\"data\":{\"message\":\"Notification Hub test notification\"}}", "testtag");
        }

        [Fact]
        public async Task push_message_notification_Hub()
        {
            NotificationHubRepository repo = new NotificationHubRepository(notificationHubConString);

            //var registration = new MobileDeviceInstallationNotificationHub()
            //{
            //    InstallationId = "instaltestid",
            //    Platform = "gcm",
            //    PushChannel = "testToken",
            //    Tags = new List<string> { "tag1", "tag2" },
            //    Templates = new System.Collections.Generic.Dictionary<string, NotificationPushTemplate>
            //    {
            //        { "genericTemplate", new NotificationPushTemplate(){
            //            Body="{\"data\":{\"message\":\"Notification Hub test notification\"}}"
            //        }
            //        }
            //    }
            //};

            // await repo.RegisterMobileDevice(registration);

            //"{ "genericTemplate":{ "body":"{\"data\":{\"message\":\"$(messageParam)\"}}"}"

            var pushRequest = new PushNotificationRequest()
            {
                Text = "{\"data\":{\"message\":\"sample test template\"}",
                Action = "sample action",
                Silent = false,
                Tags =new List<string> { "Test"}
            };

            await repo.PushNotification(pushRequest);
            //await repo.SendNotification("{\"data\":{\"message\":\"Notification Hub test notification\"}}", "testtag");
        }

        [Fact]
        public async Task send_notification_stringAsync()
        {
            NotificationHubRepository repo = new NotificationHubRepository(notificationHubConString);

           // List<string> tags = new List<string> { "Test", "Test1" };
            var response=await repo.SendNotification("{\"data\":{\"message\":\"Notification from gcm\"}}",
                "Test");            
        }

        [Fact]
        public async Task send_notification_string_multiple_Async()
        {
            NotificationHubRepository repo = new NotificationHubRepository(notificationHubConString);

            List<string> tags = new List<string> { "Test", "Test1", "Test2", "Test3", "Test4", "Test5",
                "Test6", "Test7","Test8", "Test9","Test10", "Test11","Test12", "Test13","Test14", "Test15","Test16", "Test17","Test18", "Test19",
            "Test20", "Test21","Test22", "Test23"};
            await repo.SendNotificationMultipleDevices("{\"data\":{\"message\":\"Notification from gcm\"}}",
                tags);
        }
    }

