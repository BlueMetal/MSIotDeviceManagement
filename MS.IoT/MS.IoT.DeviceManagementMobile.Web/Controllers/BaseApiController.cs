using MS.IoT.Domain.Interface;
using System.Web.Http;

namespace MS.IoT.DeviceManagementMobile.Web.Controllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        //public readonly IUserDeviceTwinRepository _userDeviceTwinRepo;
        //public readonly IDeviceTwinRepository _deviceTwinRepo;
        //public readonly INotificationHubRepository _notificationHubRepo;

        //public BaseApiController(IUserDeviceTwinRepository userDeviceTwinRepo, IDeviceTwinRepository deviceTwinRepo, INotificationHubRepository notificationHubRepo)
        //{
        //    this._userDeviceTwinRepo = userDeviceTwinRepo;
        //    _deviceTwinRepo = deviceTwinRepo;
        //    _notificationHubRepo = notificationHubRepo;
        //}
        public BaseApiController()
        { }
    }
}
