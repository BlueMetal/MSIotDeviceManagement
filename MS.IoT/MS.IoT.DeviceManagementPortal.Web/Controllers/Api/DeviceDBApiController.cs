using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.API
{
    [Route("api/devicetwin")]
    [Authorize]
    public class DeviceDBApiController : BaseApiController
    {
        private readonly IDeviceDBService _deviceServiceDB;
        private readonly INotificationHubRepository _notificationHubRepo;
        private readonly ILogger<DeviceDBApiController> logger;

        public DeviceDBApiController(IUserProfileService userProfile, 
                                     IDeviceDBService deviceServiceDB,
                                     INotificationHubRepository notificationHubRepo,
                                     ILogger<DeviceDBApiController> logger)
            : base(userProfile)
        {
            _deviceServiceDB = deviceServiceDB;
            _notificationHubRepo = notificationHubRepo;
            this.logger = logger;
        }
        
        [Route("devices/{deviceId}")]
        [HttpGet]
        [Produces(typeof(DeviceTwinModel))]
        public async Task<IActionResult> GetDeviceTwin(string deviceId)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin Properties called deviceId-{0}",deviceId);
                var twin = await _deviceServiceDB.GetDeviceTwinAsync(deviceId);
                if (twin == null)
                {
                    return NotFound("Device not found");
                }
                return Ok(twin);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/list")]
        [HttpPost]
        [Produces(typeof(DeviceQueryResponse))]
        public IActionResult GetDevicesTwinList([FromBody]DeviceQueryConfiguration config)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin Properties called");
                DeviceQueryResponse response = _deviceServiceDB.GetDevicesTwinInfoAsync(config);
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/listids")]
        [HttpPost]
        [Produces(typeof(IEnumerable<string>))]
        public IActionResult GetDevicesTwinIds([FromBody]DeviceQueryRuleGroup where)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin Ids Properties called");
                IEnumerable<string> response = _deviceServiceDB.GetDevicesTwinIds(where);
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Ids Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/mapinfo")]
        [HttpGet]
        [Produces(typeof(DeviceMapGroupsInformation))]
        public IActionResult GetMapGroupsInformations()
        {
            try
            {
                logger.LogInformation("Get Map Info called");
                DeviceMapGroupsInformation reponse = _deviceServiceDB.GetMapGroupsInformations();
                return Ok(reponse);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get Map Info Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/map")]
        [HttpPost]
        [Produces(typeof(DeviceMapQueryResponse))]
        public IActionResult GetDevicesTwinMap([FromBody]DeviceMapQueryConfiguration queryMapConfiguration)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin Map Properties called");
                DeviceMapQueryResponse reponse = _deviceServiceDB.GetDevicesTwinMapAsync(queryMapConfiguration);
                return Ok(reponse);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Map Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/maparea")]
        [HttpPost]
        [Produces(typeof(DeviceMapAreaQueryResponse))]
        public IActionResult GetDevicesTwinMapAreaAsync([FromBody]DeviceMapQueryConfiguration queryMapConfiguration)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin Map Area Properties called");
                DeviceMapAreaQueryResponse reponse = _deviceServiceDB.GetDevicesTwinMapAreaAsync(queryMapConfiguration);
                return Ok(reponse);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Map Area Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/summary")]
        [HttpGet]
        [Produces(typeof(DeviceTwinSummaryAggregationsModel))]
        public IActionResult GetDevicesSummaryAggregations(string groupBy)
        {
            try
            {
                logger.LogInformation("Get DeviceTwin summary called");
                DeviceTwinSummaryAggregationsModel summary = _deviceServiceDB.GetDevicesSummaryAggregationsAsync(groupBy);

                return Ok(summary);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get DeviceTwin Properties- Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/update")]
        [HttpPut]
        [Produces(typeof(DeviceUpdateResult))]
        public async Task<IActionResult> UpdateDevicesAsync([FromBody] DeviceTwinUpdateAsyncModel devicesUpdate)
        {
            try
            {
                logger.LogInformation("Async DeviceTwins Update called");
                DeviceUpdateResult result = await _deviceServiceDB.UpdateDevicesAsync(devicesUpdate.DeviceIds, devicesUpdate.JsonDesired, devicesUpdate.JsonTags);
                return Ok(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Async DeviceTwins Update - Exception {message}", e.Message);
                throw;
            }
        }

        [Route("device/update")]
        [HttpPut]
        public async Task<IActionResult> UpdateDeviceSync([FromBody] DeviceTwinUpdateSyncModel deviceUpdate)
        {
            try
            {
                logger.LogInformation("Sync DeviceTwin Update called");
                var result = await _deviceServiceDB.UpdateDeviceSync(deviceUpdate.DeviceId, deviceUpdate.JsonDesired, deviceUpdate.JsonTags);
                return Ok(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Sync DeviceTwin Update - Exception {message}", e.Message);
                throw;
            }
        }

        [Route("properties/features")]
        [HttpPost]
        public async Task<IActionResult> InitializeDeviceTwinDesiredFeatures([FromBody] DeviceTwinDesiredFeaturesModel deviceTwinFeaturesModel)
        {
            try
            {
                logger.LogInformation("Update DeviceTwin Properties Features called");
                await _deviceServiceDB.InitializeDeviceTwinDesiredFeaturesAsync(deviceTwinFeaturesModel);
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Update DeviceTwin Properties Features - Exception {message}", e.Message);
                throw;
            }
        }

        [Route("properties/feature")]
        [HttpPut]
        public async Task<IActionResult> UpdateDeviceTwinDesiredFeature([FromBody] DeviceTwinDesiredSingleFeatureModel deviceTwinFeatureModel)
        {
            try
            {
                logger.LogInformation("Update DeviceTwin Properties Features called for deviceId- {0}, feature- {1}",
                    deviceTwinFeatureModel.DeviceId,deviceTwinFeatureModel.Feature);
                await _deviceServiceDB.UpdateDeviceTwinDesiredFeatureAsync(deviceTwinFeatureModel);
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Update DeviceTwin Properties Features - Exception {0}, for deviceId- {1}, feature- {2}", 
                    e.Message, deviceTwinFeatureModel.DeviceId, deviceTwinFeatureModel.Feature);
                throw;
            }
        }

        [Route("properties/feature/publish")]
        [HttpPost]
        public async Task<IActionResult> FeaturePublish([FromBody] PushNotificationModel featureModel)
        {
            List<string> failureDeviceIds = new List<string>();
            try
            {              
                foreach (var item in featureModel.DeviceIdList)                 
                {
                    logger.LogInformation("Feature Publish push notification called for deviceId- {0}, feature- {1}, description- {2}", item, featureModel.FeatureName, featureModel.FeatureDescription);
                    var jsonNotification = "{\"deviceId\":\""+item+"\",\"notificationType\":\"Feature\", \"id\":\"" + featureModel.Id + "\", \"featureName\":\"" + featureModel.FeatureName + "\",\"methodName\":\""+featureModel.MethodName+"\", \"featureDescription\":\"" + featureModel.FeatureDescription + "\"}";
                    var outcome=await _notificationHubRepo.SendNotification("{\"data\":{\"message\":" + jsonNotification + "}}",
                    item);
                    if (outcome.Results != null)
                    {
                        foreach (RegistrationResult result in outcome.Results)
                        {
                            logger.LogInformation("Notification log result Platform- {0}, registrationId- {1}, outcome- {2}",
                                result.ApplicationPlatform, result.RegistrationId, result.Outcome);
                        }
                    }
                    else
                    {
                        failureDeviceIds.Add(item);
                    }
                }

                if (failureDeviceIds.Count == 0)
                    return Ok(true);
                else {
                    StringBuilder failureMsg = new StringBuilder("Failed to send notification to deviceIds -");
                    foreach (var item in failureDeviceIds)
                    {
                        failureMsg.Append(item + ",");
                    }
                    logger.LogError("Feature Publish push notification message- {0}",failureMsg.ToString());
                    return StatusCode(StatusCodes.Status502BadGateway, failureMsg.ToString());
                }                
            }
            catch (Exception e)
            {
                logger.LogError(e, "Feature Publish push notification called Exception {0}, for deviceId- {1}, feature- {2}, description- {3}",
                    e.Message, featureModel.DeviceIdList.First(), featureModel.FeatureName, featureModel.FeatureDescription);
                throw;
            }
        }

        [Route("devices/{deviceId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDevice(string deviceId)
        {
            try
            {
                logger.LogInformation("Delete Device id- {0} called", deviceId);
                await _deviceServiceDB.DeleteDeviceAsync(deviceId);
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Delete Device id- {0} - Exception {message}", deviceId, e.Message);
                throw;
            }
        }

        [Route("devices")]
        [HttpPost]
        public async Task<IActionResult> DeleteMultipleDevices([FromBody] List<string> deviceIds)
        {
            try
            {
                logger.LogInformation("Delete multiple devices called");
                await _deviceServiceDB.DeleteMultipleDevicesAsync(deviceIds);
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Update Devices DeviceTwin Location - Exception {message}", e.Message);
                throw;
            }
        }

        [Route("devices/import")]
        [HttpPost]
        public async Task<IActionResult> ImportDeviceTwinCSV(IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    List<string> deviceNotFoundList = new List<string>();

                    var config = new Configuration()
                    {
                        SanitizeForInjection = true,
                        HasHeaderRecord = true,
                        HeaderValidated = null,
                        MissingFieldFound = null
                    };
                    using (var csvReader = new CsvReader(new StreamReader(file.OpenReadStream()), config))
                    {         
                        if (!csvReader.Read() || !csvReader.ReadHeader())
                        {
                            return BadRequest("Missing header record");
                        }

                        var headers = csvReader.Context.HeaderRecord;

                        if (headers.Contains("DeviceId") && headers.Contains("ProductFamily") && headers.Contains("ProductName") && headers.Contains("ManufacturedDate")
                            || (headers.Contains("RetailerName") && headers.Contains("ShippedDate") && headers.Contains("RetailerRegion")))
                        {
                            var records = csvReader.GetRecords<DeviceTwinImportModel>();
                            foreach (var record in records)
                            {
                                if (!string.IsNullOrEmpty(record.DeviceId))
                                {
                                    var device = await _deviceServiceDB.ImportInitializeDeviceTwin(record.DeviceId, record);
                                    if (device == null)
                                    {
                                        deviceNotFoundList.Add(record.DeviceId);
                                    }
                                }                              
                            }

                            if (deviceNotFoundList.Count > 0)
                            {
                                StringBuilder errorString = new StringBuilder();
                                errorString.Append("Cannot identify some Device Ids - ");
                                foreach (var item in deviceNotFoundList)
                                {
                                    errorString.Append(item + ", ");
                                }
                                return BadRequest(errorString.ToString());
                            }
                            else
                            {
                                return Ok(true);
                            }
                        }        
                        else
                        {
                            return BadRequest(@"File not valid. Please check the 
                            column names are 'ProductFamily', 'ProductName', 'ManufacturedDate', 'RetailerName', 'ShippingDate', 'RetailerRegion'");
                        }
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, nameof(ImportDeviceTwinCSV));
                return BadRequest(e.Message);
            }

        }

        [Route("devices/fields")]
        [HttpGet]
        public IActionResult GetDeviceFields()
        {
            try
            {
                logger.LogInformation("Get Device Fields");
                var result = _deviceServiceDB.GetDeviceFields();
                return Ok(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get Device Fields Cache - Exception {message}", e.Message);
                throw;
            }
        }
    }
}