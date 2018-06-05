using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;

using QXFUtilities;
using System.Threading.Tasks;
using MS.IoT.Mobile.Helpers;

namespace MS.IoT.Mobile
{
    public class NotificationsPageViewModel : ViewModelBase
    {
        public ICommand PageAppearing { get; set; }

        public ICommand ListItemTapped { get; set; }


        private string _notificationIconName = "blank.png";
        public string NotificationIconName
        {
            get { return _notificationIconName; }
            private set
            {
                _notificationIconName = value;
                OnPropertyChanged<string>();
            }
        }


        private static ObservableCollection<NotificationViewModel> _notificationCollection = new ObservableCollection<NotificationViewModel>();
        public ObservableCollection<NotificationViewModel> NotificationCollection
        {
            get
            {
                return new ObservableCollection<NotificationViewModel>(_notificationCollection.ToList().OrderByDescending(x => x.ReceivedDTS.Ticks));
            }
            set
            {
                _notificationCollection = value;
                OnPropertyChanged<ObservableCollection<NotificationViewModel>>();
            }
        }
        public void AddNotification(NotificationViewModel notification)
        {
            _notificationCollection.Add(notification);
            OnPropertyChanged<ObservableCollection<NotificationViewModel>>(nameof(NotificationCollection));
        }
        public void RemoveNotification(NotificationViewModel notification)
        {
            if (_notificationCollection.Remove(notification))
                OnPropertyChanged<ObservableCollection<NotificationViewModel>>(nameof(NotificationCollection));
        }
        public void RemoveNotification(string id)
        {
            NotificationViewModel notification = _notificationCollection.Where(n => n.Id == id).FirstOrDefault();
            if (notification != null)
                RemoveNotification(notification);
        }


        public NotificationsPageViewModel(INavigation navigation = null)
        {
            this.Navigation = navigation;
            PageAppearing = new Command(async () => await OnPageAppearing());
            ListItemTapped = new Command<NotificationViewModel>(async (vm) => await OnListItemTapped(vm));

            Settings.NotificationsChangedEvent += OnNotificationsChanged;

            // Reload notifications from storage
            ReloadNotifications();
        }


        private void ReloadNotifications()
        {
            // Reload notifications from storage

            // Clear current notifications list
            _notificationCollection.Clear();
            // get notifications from storage
            List<Notification> notifications = Settings.Notifications;
            if (notifications?.Count > 0)
            {
                List<NotificationViewModel> nvms = new List<NotificationViewModel>(0);
                foreach (Notification notification in notifications)
                {
                    nvms.Add(new NotificationViewModel(notification, this));
                }
                NotificationCollection = new ObservableCollection<NotificationViewModel>(nvms);
            }
        }



        private async Task OnPageAppearing()
        {
            // change the icon
            NotificationIconName = Settings.NumberUnreadNotifications == 0 ? "blank.png" : "notifications2.png";
        }


        public async Task OnListItemTapped(NotificationViewModel vm)
        {

            switch (vm.NotificationType)
            {
                // depending  on notification type and other entries navigate to new page, or do something
                case NotificationType.Feature:
                    // should be actioned by button press
                    break;
                case NotificationType.FeatureUpdate:
                    // should be actioned by button press
                    break;
                case NotificationType.Offer:
                    if (!string.IsNullOrWhiteSpace(vm.UrlLink))
                    {
                        // Navigate to url in WebView
                    }
                    break;

                default:
                    if (!string.IsNullOrWhiteSpace(vm.Message))
                    {
                        // Navigate to Message page
                    }
                    break;

            }

            if (vm.IsUnread)
            {
                // change state to read
                vm.IsUnread = false;
                // update stored entry to persist the state (note: Notification has a Read property, not UnRead) 
                await Settings.UpdateNotificationAsync(vm.Id, true);
            }
        }



        private void OnNotificationsChanged(object sender, NotificationsChangedEventArgs e)
        {
            // Reload Notifications from storage
            ReloadNotifications();
            // change the icon
            NotificationIconName = Settings.NumberUnreadNotifications == 0 ? "blank.png" : "notifications2.png";

        }

    }

}
