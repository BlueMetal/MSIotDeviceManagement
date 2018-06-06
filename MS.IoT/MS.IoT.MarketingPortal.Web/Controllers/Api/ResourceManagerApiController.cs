using System;
using System.Linq;
using System.Threading.Tasks;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using Newtonsoft.Json;
using MS.IoT.Domain.Model;
using MS.IoT.MarketingPortal.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Route("api/resourcemanager")]
    [Authorize]
    public class ResourceManagerApiController : BaseApiController
    {
        private readonly IOptions<UrlOptions> urlOptions;
        private readonly ILogger<ResourceManagerApiController> logger;

        public ResourceManagerApiController(IServicePrincipalRepository servicePrincipalRepo, 
                                            IResourceManagerRepository resourceManagerRepo,
                                            IOptions<UrlOptions> urlOptions,
                                            ILogger<ResourceManagerApiController> logger) 
            : base(servicePrincipalRepo, resourceManagerRepo)
        {
            this.urlOptions = urlOptions;
            this.logger = logger;
        }


        [Route("subscriptions")]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                logger.LogInformation("Get Subscriptions- token value");
                var subscriptions = await _resourceManagerRepo.GetSubscriptions();
                if (subscriptions.Count > 0)
                {
                    SubscriptionResponseModel subscriptionModel = new SubscriptionResponseModel()
                    {
                        SubscriptionList = subscriptions.Select(s => new SubscriptionModel()
                        {
                            DisplayName = s.DisplayName,
                            Id = s.Id,
                            SubscriptionId = s.SubscriptionId
                        }).ToList(),
                    };
                    return Ok(subscriptionModel);
                }
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get Subscriptions- Exception {message}",e.Message);
                throw;
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
        public async Task<IActionResult> Deploy4x4IoTSolution([FromBody]DeploymentModel depMod)
        {
            try
            {
                logger.LogInformation("Deploy application: {application}", depMod.ApplicationName);
                DeploymentRequest deployReq = new DeploymentRequest()
                {
                    ApplicationName = depMod.ApplicationName,
                    ClientId = depMod.ClientId,
                    ClientSecret = depMod.ClientSecret,
                    SubscriptionId = depMod.SubscriptionId,
                    TenantId = depMod.TenantId,
                    Location = depMod.Location,
                    DataPacketDesignerPackageWebZipUri = urlOptions.Value.DataPacketDesignerPackage,
                    DeviceManagementPortalPackageWebZipUri= urlOptions.Value.DeviceManagementPortalPackage
                };
                await _resourceManagerRepo.CreateResoureGroup(depMod.SubscriptionId, depMod.Location, depMod.ApplicationName);
                var deployment = await _resourceManagerRepo.Deploy4x4MSIoTSolutionUsingAzureRMTemplate(deployReq);

                logger.LogInformation("Deploy application Begin-Accepted");
                return Ok(deployment);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Deploy application - Exception {message}", e.Message);
                throw;
            }        
        }

        [Route("deploy/status")]
        [HttpPost]
        public async Task<IActionResult> GetDeployStatus([FromBody]DeploymentStatusModel deployStatus)
        {
            try
            {
                logger.LogInformation("Status of deployed application: {application}", deployStatus.ApplicationName);
                DeploymentStatusRequest deployReq = new DeploymentStatusRequest()
                {
                    ApplicationName = deployStatus.ApplicationName,
                    SubscriptionId = deployStatus.SubscriptionId,
                };
               
                var deployment = await _resourceManagerRepo.GetDeploymentStatus(deployReq);

                if (deployment.Properties.ProvisioningState.Equals("Succeeded") &&
                    deployment.Properties.Outputs != null)
                {
                    var deploymentOutput = JsonConvert.DeserializeObject<DeploymentTemplateOutputModel>(deployment.Properties.Outputs.ToString());
                    deploymentOutput.properties = new Properties() { provisioningState = "Succeeded" };
                    logger.LogInformation("Status of deployed completed - application url is: {url}", deploymentOutput.DataPacketDesignerURL.Value);
                    return Ok(deploymentOutput);
                }
                else
                {
                    logger.LogInformation("Deployment status is Running: {url}");
                    return Ok(deployment);                    
                }          
            }
            catch (Exception e)
            {
                logger.LogError(e, "Deploy application - Exception {message}", e.Message);
                throw;
            }
        }


        [Route("deploystreamanalytics")]
        [HttpPost]
        public async Task Deploy4x4IoTStreamAnalytics([FromBody]StreamAnalyticsDeploymentModel depMod)
        {
            try
            {
                logger.LogInformation("Deploying Stream Analytics : {iothub}", depMod.IoTHubName);
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

                var deployment = await _resourceManagerRepo.Deploy4x4StreamAnalyticsUsingAzureRMTemplate(deployReq);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Deploy application - Exception {message}", e.Message);
            }
        }
    }
}