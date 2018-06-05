using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.NotificationHubs;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.Domain.Interface
{
    public interface INotificationHubRepository
    {
        Task<NotificationOutcome> SendNotification(string payload,string tag);

        Task SendNotificationMultipleDevices(string payload, List<string> tags);

        Task<Boolean> RegisterMobileDevice(MobileDeviceInstallationNotificationHub deviceUpdate);

        Task DeleteDeviceInstallation(string id);

        Task<NotificationOutcome> PushNotification(PushNotificationRequest pushRequest);
    } 
}
