using System;
using System.Threading.Tasks;


namespace MS.IoT.Mobile.Services.Notifications
{
    public interface IPushNotificationService
    {
        DeviceInstallation GetDeviceRegistration(params string[] tags);
        string GetDeviceId();
    }
}
