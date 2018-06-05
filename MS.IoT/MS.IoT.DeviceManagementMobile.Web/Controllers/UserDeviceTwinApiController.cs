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
    [RoutePrefix("api/devices")]
    public class UserDeviceTwinApiController : BaseApiController
    {
        public readonly IUserDeviceTwinRepository _userDeviceTwinRepo;
        public readonly IDeviceTwinRepository _deviceTwinRepo;

        public UserDeviceTwinApiController(IUserDeviceTwinRepository userDeviceTwinRepo, 
            IDeviceTwinRepository deviceTwinRepo)
            : base()
        {
            _userDeviceTwinRepo = userDeviceTwinRepo;
            _deviceTwinRepo = deviceTwinRepo;
        }

        [Route("{deviceId}/users/{{userId}}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserDevice(string deviceId,string userId)
        {
            try
            {
                Log.Information("Get User DeviceTwin Properties called - deviceId- {0} userId- {1}",deviceId,userId);
                var response = await _userDeviceTwinRepo.GetUserDeviceTwinAsync(userId, deviceId);
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error("Get User DeviceTwin Properties called - deviceId- {0} userId- {1}- Exception {2}", deviceId, userId, e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("users/{userId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserDevices(string userId)
        {
            try
            {
                Log.Information("Get User DeviceTwin List Properties called - userId- {0}", userId);
                var response = await _userDeviceTwinRepo.GetUserDevicesTwinAsync(userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error("Get User DeviceTwin List Properties called - userId- {0},- Exception {1}",userId, e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("{deviceId}/features/{methodName}/{methodParameter}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateFeature(string deviceId, string methodName, string methodParameter)
        {
            try
            {
                Log.Information("Get User DeviceTwin update features called - deviceId- {0}, feature method - {1},{2}", deviceId, 
                    methodName, methodParameter);
                DirectMethodResponse response = null;
                switch (methodName)
                {
                    case "launchBrew":
                        response = await _userDeviceTwinRepo.UpdateDeviceFeatureDirectMethod(deviceId, new DirectMethodLaunchBrew());
                        break;
                    case "launchGrindAndBrew":
                        response = await _userDeviceTwinRepo.UpdateDeviceFeatureDirectMethod(deviceId, new DirectMethodLaunchBrewGrind());
                        break;
                    case "changeBrewStrength":
                        int value = 0;
                        if (int.TryParse(methodParameter, out value))
                            response = await _userDeviceTwinRepo.UpdateDeviceFeatureDirectMethod(deviceId, new DirectMethodChangeBrewStrength((DirectMethodChangeBrewStrength.BrewStrength)value));
                        break;
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error("Get User DeviceTwin update features called - deviceId- {0}, feature method - {1},{2}- Exception {3}", deviceId, methodName, methodParameter, e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("feature")]
        [HttpPut]
        public async Task<IHttpActionResult> ActivateFeature(DeviceTwinDesiredSingleFeatureModel deviceTwinFeatureModel)
        {
            try
            {
                Log.Information("Update DeviceTwin Properties Features update called,  deviceId - {0}, feature - {1}",
                    deviceTwinFeatureModel.DeviceId,deviceTwinFeatureModel.Feature);
                await _deviceTwinRepo.UpdateDeviceTwinDesiredFeatureAsync(deviceTwinFeatureModel);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Update DeviceTwin Properties Features update- Exception,,  deviceId - {0}, feature - {1}, {2}", 
                    deviceTwinFeatureModel.DeviceId, deviceTwinFeatureModel.Feature, e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }
    }
}
