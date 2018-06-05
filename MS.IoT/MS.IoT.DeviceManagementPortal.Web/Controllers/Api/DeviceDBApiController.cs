using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using MS.IoT.Common;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Web.Http.Description;
using Microsoft.Azure.NotificationHubs;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.Api
{
    [RoutePrefix("api/devicetwin")]
    [Authorize]
    public class DeviceDBApiController : BaseApiController
    {
        private readonly IDeviceDBService _deviceServiceDB;
        private readonly INotificationHubRepository _notificationHubRepo;

        public HttpContext HttpContext { get; set; }
        public DeviceDBApiController(IUserProfileService userProfile, IDeviceDBService deviceServiceDB,
            INotificationHubRepository notificationHubRepo)
            : base(userProfile)
        {
            _deviceServiceDB = deviceServiceDB;
            _notificationHubRepo = notificationHubRepo;
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        [Route("devices/{deviceId}")]
        [HttpGet]
        [ResponseType(typeof(DeviceTwinModel))]
        public async Task<IHttpActionResult> GetDeviceTwin(string deviceId)
        {
            try
            {
                Log.Information("Get DeviceTwin Properties called deviceId-{0}",deviceId);
                var twin = await _deviceServiceDB.GetDeviceTwinAsync(deviceId);
                if (twin == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Device not found"));
                }
                return Ok(twin);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/list")]
        [HttpPost]
        [ResponseType(typeof(DeviceQueryResponse))]
        public IHttpActionResult GetDevicesTwinList([FromBody]DeviceQueryConfiguration config)
        {
            try
            {
                Log.Information("Get DeviceTwin Properties called");
                DeviceQueryResponse response = _deviceServiceDB.GetDevicesTwinInfoAsync(config);
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/listids")]
        [HttpPost]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetDevicesTwinIds([FromBody]DeviceQueryRuleGroup where)
        {
            try
            {
                Log.Information("Get DeviceTwin Ids Properties called");
                IEnumerable<string> response = _deviceServiceDB.GetDevicesTwinIds(where);
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Ids Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/mapinfo")]
        [HttpGet]
        [ResponseType(typeof(DeviceMapGroupsInformation))]
        public IHttpActionResult GetMapGroupsInformations()
        {
            try
            {
                Log.Information("Get Map Info called");
                DeviceMapGroupsInformation reponse = _deviceServiceDB.GetMapGroupsInformations();
                return Ok(reponse);
            }
            catch (Exception e)
            {
                Log.Error("Get Map Info Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/map")]
        [HttpPost]
        [ResponseType(typeof(DeviceMapQueryResponse))]
        public IHttpActionResult GetDevicesTwinMap([FromBody]DeviceMapQueryConfiguration queryMapConfiguration)
        {
            try
            {
                Log.Information("Get DeviceTwin Map Properties called");
                DeviceMapQueryResponse reponse = _deviceServiceDB.GetDevicesTwinMapAsync(queryMapConfiguration);
                return Ok(reponse);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Map Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/maparea")]
        [HttpPost]
        [ResponseType(typeof(DeviceMapAreaQueryResponse))]
        public IHttpActionResult GetDevicesTwinMapAreaAsync([FromBody]DeviceMapQueryConfiguration queryMapConfiguration)
        {
            try
            {
                Log.Information("Get DeviceTwin Map Area Properties called");
                DeviceMapAreaQueryResponse reponse = _deviceServiceDB.GetDevicesTwinMapAreaAsync(queryMapConfiguration);
                return Ok(reponse);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Map Area Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/summary")]
        [HttpGet]
        [ResponseType(typeof(DeviceTwinSummaryAggregationsModel))]
        public IHttpActionResult GetDevicesSummaryAggregations([FromUri]string groupBy)
        {
            try
            {
                Log.Information("Get DeviceTwin summary called");
                DeviceTwinSummaryAggregationsModel summary = _deviceServiceDB.GetDevicesSummaryAggregationsAsync(groupBy);

                return Ok(summary);
            }
            catch (Exception e)
            {
                Log.Error("Get DeviceTwin Properties- Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/update")]
        [HttpPut]
        [ResponseType(typeof(DeviceUpdateResult))]
        public async Task<IHttpActionResult> UpdateDevicesAsync([FromBody] DeviceTwinUpdateAsyncModel devicesUpdate)
        {
            try
            {
                Log.Information("Async DeviceTwins Update called");
                DeviceUpdateResult result = await _deviceServiceDB.UpdateDevicesAsync(devicesUpdate.DeviceIds, devicesUpdate.JsonDesired, devicesUpdate.JsonTags);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error("Async DeviceTwins Update - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("device/update")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateDeviceSync([FromBody] DeviceTwinUpdateSyncModel deviceUpdate)
        {
            try
            {
                Log.Information("Sync DeviceTwin Update called");
                var result = await _deviceServiceDB.UpdateDeviceSync(deviceUpdate.DeviceId, deviceUpdate.JsonDesired, deviceUpdate.JsonTags);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error("Sync DeviceTwin Update - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("properties/features")]
        [HttpPost]
        public async Task<IHttpActionResult> InitializeDeviceTwinDesiredFeatures(DeviceTwinDesiredFeaturesModel deviceTwinFeaturesModel)
        {
            try
            {
                Log.Information("Update DeviceTwin Properties Features called");
                await _deviceServiceDB.InitializeDeviceTwinDesiredFeaturesAsync(deviceTwinFeaturesModel);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Update DeviceTwin Properties Features - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("properties/feature")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateDeviceTwinDesiredFeature(DeviceTwinDesiredSingleFeatureModel deviceTwinFeatureModel)
        {
            try
            {
                Log.Information("Update DeviceTwin Properties Features called for deviceId- {0}, feature- {1}",
                    deviceTwinFeatureModel.DeviceId,deviceTwinFeatureModel.Feature);
                await _deviceServiceDB.UpdateDeviceTwinDesiredFeatureAsync(deviceTwinFeatureModel);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Update DeviceTwin Properties Features - Exception {0}, for deviceId- {1}, feature- {2}", 
                    e.Message, deviceTwinFeatureModel.DeviceId, deviceTwinFeatureModel.Feature);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("properties/feature/publish")]
        [HttpPost]
        public async Task<IHttpActionResult> FeaturePublish(PushNotificationModel featureModel)
        {
            List<string> failureDeviceIds = new List<string>();
            try
            {              
                foreach (var item in featureModel.DeviceIdList)                 
                {
                    Log.Information("Feature Publish push notification called for deviceId- {0}, feature- {1}, description- {2}", item, featureModel.FeatureName, featureModel.FeatureDescription);
                    var jsonNotification = "{\"deviceId\":\""+item+"\",\"notificationType\":\"Feature\", \"id\":\"" + featureModel.Id + "\", \"featureName\":\"" + featureModel.FeatureName + "\",\"methodName\":\""+featureModel.MethodName+"\", \"featureDescription\":\"" + featureModel.FeatureDescription + "\"}";
                    var outcome=await _notificationHubRepo.SendNotification("{\"data\":{\"message\":" + jsonNotification + "}}",
                    item);
                    if (outcome.Results != null)
                    {
                        foreach (RegistrationResult result in outcome.Results)
                        {
                            Log.Information("Notification log result Platform- {0}, registrationId- {1}, outcome- {2}",
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
                    Log.Error("Feature Publish push notification message- {0}",failureMsg.ToString());
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, failureMsg.ToString()));
                }                
            }
            catch (Exception e)
            {
                Log.Error("Feature Publish push notification called Exception {0}, for deviceId- {1}, feature- {2}, description- {3}",
                    e.Message, featureModel.DeviceIdList.First(), featureModel.FeatureName, featureModel.FeatureDescription);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/refreshDB")]
        [HttpGet]
        public async Task<IHttpActionResult> RefreshDeviceDBCache()
        {
            try
            {
                Log.Information("Refresh DeviceDB Cache");
                var result = await _deviceServiceDB.RefreshDeviceDBCache();
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error("Refresh DeviceDB Cache - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/{deviceId}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteDevice(string deviceId)
        {
            try
            {
                Log.Information("Delete Device id- {0} called", deviceId);
                await _deviceServiceDB.DeleteDeviceAsync(deviceId);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Delete Device id- {0} - Exception {message}", deviceId, e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteMultipleDevices([FromBody] List<string> deviceIds)
        {
            try
            {
                Log.Information("Delete multiple devices called");
                await _deviceServiceDB.DeleteMultipleDevicesAsync(deviceIds);
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error("Update Devices DeviceTwin Location - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("devices/import")]
        [HttpPost]
        public async Task<IHttpActionResult> ImportDeviceTwinCSV()
        {
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = HttpContext.Current.Request.Files["file"];

                    List<string> deviceNotFoundList = new List<string>();

                    using (TextFieldParser csvReader = new TextFieldParser(httpPostedFile.InputStream))
                    {
                        csvReader.SetDelimiters(new string[] { "," });
                        csvReader.HasFieldsEnclosedInQuotes = true;
                        string[] colFields = csvReader.ReadFields();

                        Dictionary<string, int> columnDict = new Dictionary<string, int>();

                        int index = 0;
                        foreach (string column in colFields)
                        {
                            if(column!="")
                                columnDict.Add(column, index++);
                        }
                      
                        if (columnDict.ContainsKey("DeviceId") && columnDict.ContainsKey("ProductFamily") && columnDict.ContainsKey("ProductName") && columnDict.ContainsKey("ManufacturedDate")
                            || (columnDict.ContainsKey("RetailerName") && columnDict.ContainsKey("ShippedDate") && columnDict.ContainsKey("RetailerRegion")))
                        {
                            while (!csvReader.EndOfData)
                            {
                                string[] fieldData = csvReader.ReadFields();
                                if (fieldData.All(col => col == ""))
                                {
                                    break;
                                }

                                var tags = new DeviceTwinTagsModel()
                                {
                                    ProductFamily = fieldData[columnDict["ProductFamily"]],
                                    ProductName = fieldData[columnDict["ProductName"]],
                                    RetailerName = columnDict.ContainsKey("RetailerName") ? fieldData[columnDict["RetailerName"]] : null,
                                    ManufacturedDate =(columnDict.ContainsKey("ManufacturedDate") ? (fieldData[columnDict["ManufacturedDate"]] != ""
                                                ? Convert.ToDateTime(fieldData[columnDict["ManufacturedDate"]]) : (DateTime?)null) : (DateTime?)null),
                                    ShippedDate = (columnDict.ContainsKey("ShippedDate") ? (fieldData[columnDict["ShippedDate"]] != ""
                                                ? Convert.ToDateTime(fieldData[columnDict["ShippedDate"]]) : (DateTime?)null) : (DateTime?)null),
                                    RetailerRegion = columnDict.ContainsKey("RetailerRegion") ? fieldData[columnDict["RetailerRegion"]] : null
                                };
                                
                                var device= await _deviceServiceDB.ImportInitializeDeviceTwin(fieldData[columnDict["DeviceId"]], tags);
                                if (device == null)
                                {
                                    deviceNotFoundList.Add(fieldData[columnDict["DeviceId"]]);
                                }
                            }
                            if (deviceNotFoundList.Count > 0)
                            {
                                StringBuilder errorString = new StringBuilder();
                                errorString.Append("Cannot identify some Device Ids - ");
                                foreach (var item in deviceNotFoundList)
                                {
                                    errorString.Append(item +", ");
                                }
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, errorString.ToString()));
                            }
                            else
                            {
                                return Ok(true);
                            }
                            
                        }        
                        else
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, @"File not valid. Please check the 
                            column names are 'ProductFamily', 'ProductName', 'ManufacturedDate', 'RetailerName', 'ShippingDate', 'RetailerRegion'"));
                        }
                    }
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "File not valid"));
                }
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }

        }

        [Route("devices/fields")]
        [HttpGet]
        public IHttpActionResult GetDeviceFields()
        {
            try
            {
                Log.Information("Get Device Fields");
                var result = _deviceServiceDB.GetDeviceFields();
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error("Get Device Fields Cache - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }
    }
}