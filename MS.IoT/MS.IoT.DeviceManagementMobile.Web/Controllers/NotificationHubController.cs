using Microsoft.AspNetCore.Mvc;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DeviceManagementMobile.Web.Controllers
{
    [Authorize]
    [Route("api/notification")]
    public class NotificationHubController : Controller
    {
        private readonly INotificationHubRepository _notificationHubRepo;
        private readonly ILogger<NotificationHubController> logger;

        public NotificationHubController(INotificationHubRepository notificationHubRepo, ILogger<NotificationHubController> logger)
        {
            _notificationHubRepo = notificationHubRepo;
            this.logger = logger;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterDevice([FromBody] MobileDeviceInstallationNotificationHub deviceInstallation)
        {
            try
            {
                logger.LogInformation("Register Device in Notification Hub id- {0}",deviceInstallation.InstallationId);
                await _notificationHubRepo.RegisterMobileDevice(deviceInstallation);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Register Device in Notification Hub- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("pushmessage")]
        [HttpPost]
        public async Task<IActionResult> PushNotification([FromBody] PushNotificationRequest pushRequest)
        {
            try
            {
                logger.LogInformation("Push Notification called message {message}, tags- {tags}",pushRequest.Text, pushRequest.Tags);
                await _notificationHubRepo.PushNotification(pushRequest);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Push Notification called Exceptionfor messagebody - {body}, tags- {tags}", pushRequest.Text, pushRequest.Tags);
                throw;
            }
        }     
    }
}
