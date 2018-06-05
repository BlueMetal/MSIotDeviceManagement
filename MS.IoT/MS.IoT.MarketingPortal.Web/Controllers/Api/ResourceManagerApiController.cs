using System;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using Newtonsoft.Json;
using System.Web.Http;
using MS.IoT.Domain.Model;
using System.Net;
using System.Net.Http;
using MS.IoT.Common;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [RoutePrefix("api/resourcemanager")]
    [Authorize]
    public class ResourceManagerApiController : BaseApiController
    {
        public HttpContext HttpContext { get; set; }

        public ResourceManagerApiController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo) : base(servicePrincipalRepo, resourceManagerRepo)
        {
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        [Route("subscriptions")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSubscriptions()
        {
            try
            {
                Log.Information("Get Subscriptions- token value {token}", AuthConfig.SessionItems.ManagementAuthToken);
                var subscriptions = await _resourceManagerRepo.GetSubscriptions(AuthConfig.SessionItems.ManagementAuthToken);
                if (subscriptions.SubscriptionList.Count > 0)
                {
                    SubscriptionResponseModel subscriptionModel = new SubscriptionResponseModel()
                    {
                        SubscriptionList = subscriptions.SubscriptionList.Select(s => new SubscriptionModel()
                        {
                            DisplayName = s.DisplayName,
                            Id = s.Id,
                            SubscriptionId = s.SubscriptionId
                        }).ToList(),
                    };
                    return Ok(subscriptionModel);
                }
                else
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,"No Subscription found"));
            }
            catch (Exception e)
            {
                Log.Error("Get Subscriptions- Exception {message}",e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }        
        }

        [Route("subscriptions/{subscriptionId}/locations")]
        [HttpGet]
        public LocationResponseModel GetLocations(string subscriptionId)
        {
            var locationModel = LocationListHelper.GetLocationList();         
            return locationModel;
        }

        [Route("deploy")]
        [HttpPost]
        public async Task<IHttpActionResult> Deploy4x4IoTSolution(DeploymentModel depMod)
        {
            try
            {
                Log.Information("Deploy application: {application}", depMod.ApplicationName);
                DeploymentRequest deployReq = new DeploymentRequest()
                {
                    ApplicationName = depMod.ApplicationName,
                    ClientId = depMod.ClientId,
                    ClientSecret = depMod.ClientSecret,
                    SubscriptionId = depMod.SubscriptionId,
                    TenantId = depMod.TenantId,
                    Location = depMod.Location,
                    DataPacketDesignerPackageWebZipUri = AppConfig.ConfigurationItems.DataPacketDesignerPackageWebZipuri,
                    DeviceManagementPortalPackageWebZipUri=AppConfig.ConfigurationItems.DeviceManagementPortalPackageWebZipUri
                };
                await _resourceManagerRepo.CreateResoureGroup(depMod.SubscriptionId,
                depMod.Location, depMod.ApplicationName, AuthConfig.SessionItems.ManagementAuthToken);
                var deployment = await _resourceManagerRepo.Deploy4x4MSIoTSolutionUsingAzureRMTemplate(deployReq, AuthConfig.SessionItems.ManagementAuthToken);

                Log.Information("Deploy application Begin-Accepted");
                return Ok(deployment);
            }
            catch (Exception e)
            {
                Log.Error("Deploy application - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }        
        }

        [Route("deploy/status")]
        [HttpPost]
        public async Task<IHttpActionResult> GetDeployStatus(DeploymentStatusModel deployStatus)
        {
            try
            {
                Log.Information("Status of deployed application: {application}", deployStatus.ApplicationName);
                DeploymentStatusRequest deployReq = new DeploymentStatusRequest()
                {
                    ApplicationName = deployStatus.ApplicationName,
                    SubscriptionId = deployStatus.SubscriptionId,
                };
               
                var deployment = await _resourceManagerRepo.GetDeploymentStatus(deployReq, AuthConfig.SessionItems.ManagementAuthToken);

                if (deployment.Properties.ProvisioningState.Equals("Succeeded") &&
                    deployment.Properties.Outputs != null)
                {
                    var deploymentOutput = JsonConvert.DeserializeObject<DeploymentTemplateOutputModel>(deployment.Properties.Outputs.ToString());
                    deploymentOutput.properties = new Properties() { provisioningState = "Succeeded" };
                    Log.Information("Status of deployed completed - application url is: {url}", deploymentOutput.DataPacketDesignerURL.Value);
                    return Ok(deploymentOutput);
                }
                else
                {
                    Log.Information("Deployment status is Running: {url}");
                    return Ok(deployment);                    
                }          
            }
            catch (Exception e)
            {
                Log.Error("Deploy application - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }


        [Route("deploystreamanalytics")]
        [HttpPost]
        public async Task Deploy4x4IoTStreamAnalytics(StreamAnalyticsDeploymentModel depMod)
        {
            try
            {
                Log.Information("Deploying Stream Analytics : {iothub}", depMod.IoTHubName);
                StreamAnalyticsDeploymentRequest deployReq = new StreamAnalyticsDeploymentRequest()
                {
                    IoTHubName = depMod.IoTHubName,
                    CosmosDBAccountName = depMod.CosmosDBAccountName,
                    CosmosDBName = depMod.CosmosDBName,
                    CosmosDBMessageCollectionName = depMod.CosmosDBMessageCollectionName,
                    SubscriptionId = depMod.SubscriptionId,
                    ResourceGroupName = depMod.ResourceGroupName,
                    Location = depMod.Location
                };

                var deployment = await _resourceManagerRepo.Deploy4x4StreamAnalyticsUsingAzureRMTemplate
                    (deployReq, AuthConfig.SessionItems.ManagementAuthToken);
            }
            catch (Exception e)
            {
                Log.Error("Deploy application - Exception {message}", e.Message);
            }
        }
    }
}