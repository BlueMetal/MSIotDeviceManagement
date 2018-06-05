using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MS.IoT.DeviceManagementMobile.Web.Controllers
{
    [RoutePrefix("api/notification")]
    public class NotificationHubController : BaseApiController
    {
        public readonly INotificationHubRepository _notificationHubRepo;
        public NotificationHubController(INotificationHubRepository notificationHubRepo)
            : base()
        {
            _notificationHubRepo = notificationHubRepo;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterDevice(MobileDeviceInstallationNotificationHub deviceInstallation)
        {
            try
            {
                Log.Information("Register Device in Notification Hub id- {0}",deviceInstallation.InstallationId);
                await _notificationHubRepo.RegisterMobileDevice(deviceInstallation);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Register Device in Notification Hub- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("pushmessage")]
        [HttpPost]
        public async Task<IHttpActionResult> PushNotification(PushNotificationRequest pushRequest)
        {
            try
            {
                Log.Information("Push Notification called message {0}, tags- {1}",pushRequest.Text, pushRequest.Tags);
                await _notificationHubRepo.PushNotification(pushRequest);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Push Notification called Exception {0},for messagebody - {1}, tags- {2}", e.Message, pushRequest.Text, pushRequest.Tags);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }     
    }
}
