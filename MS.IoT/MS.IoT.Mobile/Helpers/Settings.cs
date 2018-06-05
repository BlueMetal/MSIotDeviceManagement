using Plugin.Settings;
using Plugin.Settings.Abstractions;

using Newtonsoft.Json;
using MS.IoT.Mobile.Services.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MS.IoT.Mobile.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
    public static class Settings
    {
        public delegate void NotificationsChangedEventHandler(object sender, NotificationsChangedEventArgs e);
        public static event NotificationsChangedEventHandler NotificationsChangedEvent;

        public delegate void DevicesChangedEventHandler(object sender, DevicesChangedEventArgs e);
        public static event DevicesChangedEventHandler DevicesChangedEvent;


        private static ISettings AppSettings => CrossSettings.Current;


        public static string UserName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }
        public static string UserId
        {
            get => AppSettings.GetValueOrDefault(nameof(UserId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserId), value);
        }
        public static bool IsAuthenticated
        {
            get => AppSettings.GetValueOrDefault(nameof(IsAuthenticated), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsAuthenticated), value);
        }

        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AccessToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AccessToken), value);
        }

        public static string NotificationRefreshToken
        {
            get => AppSettings.GetValueOrDefault(nameof(NotificationRefreshToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(NotificationRefreshToken), value);
        }


        public static DeviceInstallation DeviceInstallation
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(DeviceInstallation), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return null;
                return JsonConvert.DeserializeObject<DeviceInstallation>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(DeviceInstallation), json);
            }
        }



#region RegisteredDevices
        // REGISTERED DEVICES
        public static List<string> RegisteredDeviceIds
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(RegisteredDeviceIds), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<string>(0);
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(RegisteredDeviceIds), json);
            }
        }


        public static List<ReturnedDevice> RawRegisteredDevices
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(RawRegisteredDevices), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<ReturnedDevice>(0);
                return JsonConvert.DeserializeObject<List<ReturnedDevice>>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(RawRegisteredDevices), json);
            }
        }


        public static List<Device> RegisteredDevices
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(RegisteredDevices), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Device>(0);
                return JsonConvert.DeserializeObject<List<Device>>(json);
            }
            set
            {
                var oldCount = RegisteredDevices.Count;
                var newCount = value.Count;
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(RegisteredDevices), json);
                // raise event
                var handler = DevicesChangedEvent;
                handler?.Invoke(typeof(Settings), new DevicesChangedEventArgs(oldCount, newCount));
            }
        }
        public static void AddRegisteredDevice(Device device)
        {
            var registeredDevices = RegisteredDevices;
            registeredDevices.Add(device);
            RegisteredDevices = registeredDevices;
        }
        public static async Task AddRegisteredDeviceAsync(Device device)
        {
            await Task.Run(() =>
            {
                AddRegisteredDevice(device);
            });
        }

        public static void AddOrUpdateRegisteredDevice(Device device)
        {
            var registeredDevices = RegisteredDevices;
            var deviceId = device.DeviceId;
            var oldDevice = registeredDevices.Where(n => n.DeviceId == deviceId).FirstOrDefault();
            if (oldDevice == null)
                registeredDevices.Add(device);
            else
            {
                int index = registeredDevices.IndexOf(oldDevice);
                if (index > -1)
                {
                    registeredDevices.RemoveAt(index);
                    registeredDevices.Insert(index, device);
                }
                else
                    // should never get here
                    registeredDevices.Add(device);
            }
            RegisteredDevices = registeredDevices;
        }
        public static async Task AddOrUpdateRegisteredDeviceAsync(Device device)
        {
            await Task.Run(() =>
            {
                AddOrUpdateRegisteredDevice(device);
            });
        }

        public static void DeleteRegisteredDevice(Device device)
        {
            var registeredDevices = RegisteredDevices;
            if (registeredDevices.Remove(device))
                RegisteredDevices = registeredDevices;
        }
        public static async Task DeleteRegisteredDeviceAsync(Device device)
        {
            await Task.Run(() =>
            {
                DeleteRegisteredDevice(device);
            });
        }

#endregion


        #region Selections
        //SELECTIONS

        public static Dictionary<string, Tuple<object, Type>> Selections
        {
            get { 
                var json = AppSettings.GetValueOrDefault(nameof(Selections), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new Dictionary<string, Tuple<object, Type>>();
                return JsonConvert.DeserializeObject<Dictionary<string, Tuple<object, Type>>>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(Selections), json);
            }
        }
        public static void AddOrUpdateSelection(KeyValuePair<string, Tuple<object, Type>> selection)
        {
            Dictionary<string, Tuple<object, Type>> selections = Selections;
            selections[selection.Key] = selection.Value;
            Selections = selections;
        }
        public static async Task AddOrUpdateSelectionAsync(KeyValuePair<string, Tuple<object, Type>> selection)
        {
            await Task.Run(() =>
            {
                AddOrUpdateSelection(selection);
            });
        }
        public static void AddOrUpdateSelection(string key, Tuple<object, Type> value)
        {
            Dictionary<string, Tuple<object, Type>> selections = Selections;
            selections[key] = value;
            Selections = selections;
        }
        public static async Task AddOrUpdateSelectionAsync(string key, Tuple<object, Type> value)
        {
            await Task.Run(() =>
            {
                AddOrUpdateSelection(key, value);
            });
        }


        public static void DeleteSelection(KeyValuePair<string, Tuple<object, Type>> selection)
        {
            Dictionary<string, Tuple<object, Type>> selections = Selections;
            if (selections.Remove(selection.Key))
                Selections = selections;
        }
        public static async Task DeleteSelectionAsync(KeyValuePair<string, Tuple<object, Type>> selection)
        {
            await Task.Run(() =>
            {
                DeleteSelection(selection);
            });
        }
        public static void DeleteSelection(string key)
        {
            Dictionary<string, Tuple<object, Type>> selections = Selections;
            if (selections.Remove(key))
                Selections = selections;
        }
        public static async Task DeleteSelectionAsync(string key)
        {
            await Task.Run(() =>
            {
                DeleteSelection(key);
            });
        }

#endregion


#region Notifications
        // NOTIFICATIONS
        public static List<Notification> Notifications
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(Notifications), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Notification>(0);
                return JsonConvert.DeserializeObject<List<Notification>>(json);                
            }
            set
            {
                var oldCount = Notifications.Count;
                var newCount = value.Count;
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(Notifications), json);
                // raise event
                var handler = NotificationsChangedEvent;
                handler?.Invoke(typeof(Settings), new NotificationsChangedEventArgs(oldCount, newCount));
            }
        }
        public static void AddNotification(Notification notification)
        {
            List<Notification> notifications = Notifications;
            notifications.Add(notification);
            Notifications = notifications;
        }
        public static async Task AddNotificationAsync(Notification notification)
        {
            await Task.Run(() =>
            {
                AddNotification(notification);
            });
        }
        public static void DeleteNotification(Notification notification)
        {
            List<Notification> notifications = Notifications;
            if (notifications.Remove(notification))
                Notifications = notifications;
        }
        public static async Task DeleteNotificationAsync(Notification notification)
        {
            await Task.Run(() =>
            {
                DeleteNotification(notification);
            });
        }
        public static void DeleteNotification(string notificationId)
        {
            List<Notification> notifications = Notifications;
            var notification = notifications.Where(n => n.Id == notificationId).FirstOrDefault();
            if (notification != null && notifications.Remove(notification))
                Notifications = notifications;
        }
        public static async Task DeleteNotificationAsync(string notificationId)
        {
            await Task.Run(() =>
            {
                DeleteNotification(notificationId);
            });
        }
        
        public static void UpdateNotification(string notificationId, bool isRead)
        {
            List<Notification> notifications = Notifications;
            var notification = notifications.Where(n => n.Id == notificationId).FirstOrDefault();
            if (notification != null)
            {
                notification.Read = isRead;
                Notifications = notifications;
            }
        }
        public static async Task UpdateNotificationAsync(string notificationId, bool isRead)
        {
            await Task.Run(() =>
            {
                UpdateNotification(notificationId, isRead);
            });
        }

        public static void AddOrUpdateNotification(Notification notification)
        {
            List<Notification> notifications = Notifications;
            var nf = notifications.Where(n => n.Id == notification.Id).FirstOrDefault();
            if (nf == null)
                notifications.Add(notification);
            else
            {
                var index = notifications.IndexOf(nf);
                notifications.RemoveAt(index);
                notifications.Insert(index, notification);
            }
            Notifications = notifications;
        }
        public static async Task AddOrUpdateNotificationAsync(Notification notification)
        {
            await Task.Run(() =>
            {
                AddOrUpdateNotification(notification);
            });
        }

        public static int NumberUnreadNotifications
        {
            get
            {
                return Notifications.Where(n => n.Read == false).Count();
            }
        }
        public static int NumberCurrentNotifications
        {
            get
            {
                return Notifications.Count;
            }
        }

#endregion


#if DEMO
        public static bool DemoDevicesAdded
        {
            get => AppSettings.GetValueOrDefault(nameof(DemoDevicesAdded), false);
            set => AppSettings.AddOrUpdateValue(nameof(DemoDevicesAdded), value);
        }
        public static bool DemoNotificationsAdded
        {
            get => AppSettings.GetValueOrDefault(nameof(DemoNotificationsAdded), false);
            set => AppSettings.AddOrUpdateValue(nameof(DemoNotificationsAdded), value);
        }
#endif



    }


    public class NotificationsChangedEventArgs : EventArgs
    {
        public int OldCount { get; set; } = -1;
        public int NewCount { get; set; } = -1;

        public NotificationsChangedEventArgs(int oldCount, int newCount)
        {
            OldCount = oldCount;
            NewCount = NewCount;
        }

    }

    public class DevicesChangedEventArgs : EventArgs
    {
        public int OldCount { get; set; } = -1;
        public int NewCount { get; set; } = -1;

        public DevicesChangedEventArgs(int oldCount, int newCount)
        {
            OldCount = oldCount;
            NewCount = NewCount;
        }

    }




}
