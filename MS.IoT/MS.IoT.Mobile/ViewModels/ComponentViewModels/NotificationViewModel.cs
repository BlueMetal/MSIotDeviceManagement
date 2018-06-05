using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using QXFUtilities;

using MS.IoT.Mobile.Helpers;
using MS.IoT.Mobile.Services.DataServices;

namespace MS.IoT.Mobile
{
    public class NotificationViewModel : BaseViewModel
    {

        private string UnreadFont = GlobalResources.Fonts.SegeoWPBold;
        private string ReadFont = GlobalResources.Fonts.SegeoWPLight;

        private ImageSource FeatureIcon = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-new-feature.svg");
        private ImageSource OfferIcon = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-savings.svg");
        private ImageSource ScheduleIcon = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-schedule.svg");





        public NotificationType NotificationType { get; set; } = NotificationType.Message;

        public string Id { get; set; }

        public ImageSource Icon
        {
            get
            {
                if (CustomIcon != null)
                {
                    // TODO Get custom icon from file system or load from server
                }
                switch (NotificationType)
                {
                    case NotificationType.Feature:
                        return FeatureIcon;

                    case NotificationType.FeatureUpdate:
                        return FeatureIcon;

                    case NotificationType.Offer:
                        return OfferIcon;         
                        
                    case NotificationType.Maintenance:
                        return ScheduleIcon; 

                    case NotificationType.Schedule:
                        return ScheduleIcon;

                    default:
                        return null;
                }
            }
        }


        private bool _isUnread = true;
        public bool IsUnread
        {
            get { return _isUnread; }
            set
            {
                _isUnread = value;
                OnPropertyChanged<bool>();
                if (value)
                    FontFamily = UnreadFont;
                else
                    FontFamily = ReadFont;
            }
        }

        private string _fontFamily;
        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value;
                OnPropertyChanged<string>();
            }
        }


        public string Label { get; set; } = string.Empty;

        public string Message  { get; set; } = null;

        public DateTime ReceivedDTS { get; set; } = DateTime.Now;



        public string Received
        {
            get {
                TimeSpan span = DateTime.Now - ReceivedDTS;
                var hrs = span.TotalHours;
                if (hrs < 1.0 )
                    return span.Minutes.ToString() + "m";
                if (hrs < 24.0)
                    return ((int)hrs).ToString() + "h";
                return ReceivedDTS.ToString("MMM-dd");
            }
            set
            {
                // TODO How to trigger from timer
                OnPropertyChanged<string>();
            }
        }

        public string DeviceId { get; set; } = null;

        public string FeatureName { get; set; } = null;

        public string FeatureId { get; set; } = null;

        public FeatureType FeatureType { get; set; } = FeatureType.Action;

        public string MethodName { get; set; } = null;

        public string UrlLink { get; set; } = null;


        public string CustomIcon { get; set; } = null;



        public ICommand ButtonClicked { get; set; }

        public NotificationViewModel()
        {
            ButtonClicked = new Command<string>(async (s) => await OnButtonClicked(s));
        }

        public NotificationViewModel(Notification notification, BaseViewModel parentViewModel = null)
        {
            ButtonClicked = new Command<string>(async (s) => await OnButtonClicked(s));

            NotificationType = notification.NotificationType;
            Id = notification.Id;
            Label = notification.FeatureName + ". " + notification.Title;
            Message = notification.Message;
            ReceivedDTS = notification.ReceivedDTS;
            IsUnread = !notification.Read;
            DeviceId = notification.DeviceId;
            FeatureName = notification.FeatureName;
            FeatureId = notification.FeatureId;
            FeatureType = notification.FeatureType;
            MethodName = notification.MethodName;
            UrlLink = notification.UrlLink;
            CustomIcon = notification.CustomIcon;
            ParentViewModel = parentViewModel;
        }



        private async Task OnButtonClicked(string s)
        {
            if (s == "YesButton")
            {
                // Activate Feature
                bool activated = await IoTHubDataService.Instance.ActivateFeature(DeviceId, FeatureId);

                if (activated)
                {
                    // Add new feature to features list for device and update the device in settings
                    List<Device> devices = Settings.RegisteredDevices;
                    // get the device with this id
                    Device device = devices.Where(d => d.DeviceId == DeviceId).FirstOrDefault();
                    if (device != null)
                    {                       
                        Feature newFeature = new Feature(this);
                        Feature oldFeature = device.Features.Where(f => f.FeatureId == FeatureId).FirstOrDefault();
                        if (oldFeature == null)
                        {
                            // Feature doesn't exist for device so add
                            device.Features.Add(newFeature);
                        }
                        else
                        {
                            // Feature exists for device so update it by replacing it with the new definition
                            var index = device.Features.IndexOf(oldFeature);
                            device.Features.RemoveAt(index);
                            device.Features.Insert(index, newFeature);
                        }
                        await Settings.AddOrUpdateRegisteredDeviceAsync(device);

                        MainPageViewModel.ReloadFeatures = true;
                    }

                    // Remove Notification from list
                    var vm = ParentViewModel as NotificationsPageViewModel;
                    vm?.RemoveNotification(Id);

                    // remove notification from storage
                    await Settings.DeleteNotificationAsync(Id);

                }
                else
                {
                    // inform user that activation wasn't possible
                    await App.Current.MainPage.DisplayAlert("Sorry", "Feature activation wasn't possible at this time. Is your device connected to the internet? \nPlease try again later", "Ok");

                }



            }
            if (s == "LaterButton")
            {
                // Postpone
                IsUnread = false;
                // Might need to update in NotificationPageViewModelCollection
                // update stored version
                await Settings.UpdateNotificationAsync(Id, !IsUnread);
            }


        }




    }

}
