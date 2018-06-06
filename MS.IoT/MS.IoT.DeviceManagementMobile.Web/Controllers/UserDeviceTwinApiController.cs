using Microsoft.AspNetCore.Mvc;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DeviceManagementMobile.Web.Controllers
{
    [Authorize]
    [Route("api/devices")]
    public class UserDeviceTwinApiController : Controller
    {
        public readonly IUserDeviceTwinRepository _userDeviceTwinRepo;
        public readonly IDeviceTwinRepository _deviceTwinRepo;
        private readonly ILogger<UserDeviceTwinApiController> logger;

        public UserDeviceTwinApiController(IUserDeviceTwinRepository userDeviceTwinRepo, IDeviceTwinRepository deviceTwinRepo, ILogger<UserDeviceTwinApiController> logger)
        {
            _userDeviceTwinRepo = userDeviceTwinRepo;
            _deviceTwinRepo = deviceTwinRepo;
            this.logger = logger;
        }

        [Route("{deviceId}/users/{{userId}}")]
        [Produces(typeof(DeviceTwinModel))]
        [HttpGet]
        public async Task<IActionResult> GetUserDevice(string deviceId,string userId)
        {
            try
            {
                logger.LogInformation("Get User DeviceTwin Properties called - deviceId- {deviceId} userId- {userId}", deviceId, userId);
                var response = await _userDeviceTwinRepo.GetUserDeviceTwinAsync(userId, deviceId);
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get User DeviceTwin Properties called - deviceId- {deviceId} userId- {userId}", deviceId, userId);
                throw;
            }
        }

        [Route("users/{userId}")]
        [Produces(typeof(List<DeviceTwinModel>))]
        [HttpGet]
        public async Task<IActionResult> GetUserDevices(string userId)
        {
            try
            {
                logger.LogInformation("Get User DeviceTwin List Properties called - userId- {userId}", userId);
                var response = await _userDeviceTwinRepo.GetUserDevicesTwinAsync(userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get User DeviceTwin List Properties called - userId- {userId}", userId);
                throw;
            }
        }

        [Route("{deviceId}/features/{methodName}/{methodParameter}")]
        [Produces(typeof(DirectMethodResponse))]
        [HttpPut]
        public async Task<IActionResult> UpdateFeature(string deviceId, string methodName, string methodParameter)
        {
            try
            {
                logger.LogInformation("Get User DeviceTwin update features called - deviceId- {deviceId}, feature method - {methodName}, {methodParameter}", deviceId, 
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

                if(response == null)
                    return BadRequest();

                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get User DeviceTwin update features called - deviceId- {deviceId}, feature method - {methodName},{methodParameter}", deviceId, methodName, methodParameter);
                throw;
            }
        }

        [Route("feature")]
        [HttpPut]
        public async Task<IActionResult> ActivateFeature([FromBody] DeviceTwinDesiredSingleFeatureModel deviceTwinFeatureModel)
        {
            try
            {
                logger.LogInformation("Update DeviceTwin Properties Features update called,  deviceId - {deviceId}, feature - {feature}",
                    deviceTwinFeatureModel.DeviceId,deviceTwinFeatureModel.Feature);
                await _deviceTwinRepo.UpdateDeviceTwinDesiredFeatureAsync(deviceTwinFeatureModel);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Update DeviceTwin Properties Features update- Exception,,  deviceId - {deviceId}, feature - {feature}", 
                    deviceTwinFeatureModel.DeviceId, deviceTwinFeatureModel.Feature);
                throw;
            }
        }
    }
}
