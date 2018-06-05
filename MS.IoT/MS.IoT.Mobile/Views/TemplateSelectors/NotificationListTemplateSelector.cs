using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MS.IoT.Mobile
{
    public class NotificationListTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate standardNotificationDataTemplate;
        private readonly DataTemplate twoButtonNotificationDataTemplate;


        public NotificationListTemplateSelector()
        {
            standardNotificationDataTemplate = new DataTemplate(typeof(StandardNotificationViewCell));
            twoButtonNotificationDataTemplate = new DataTemplate(typeof(TwoButtonNotificationViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var vm = item as NotificationViewModel;
            if (vm == null)
                return null;

            switch (vm.NotificationType)
            {
                case NotificationType.Feature:
                    return twoButtonNotificationDataTemplate;

                case NotificationType.FeatureUpdate:
                    return twoButtonNotificationDataTemplate;

                default:
                    return standardNotificationDataTemplate;

            }

        }
    }
}
