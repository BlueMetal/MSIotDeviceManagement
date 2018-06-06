using Microsoft.Azure.Documents.Client;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Rest.Azure;

namespace MS.IoT.Repositories
{

    public class ResourceManagerRepository : IResourceManagerRepository
    {
        private readonly IArmClientFactory clientFactory;
        private readonly ILogger<ResourceManagerRepository> logger;
        private readonly ArmOptions armOptions;

        public ResourceManagerRepository(IOptions<ArmOptions> armOptions, 
                                         IArmClientFactory clientFactory, 
                                         ILogger<ResourceManagerRepository> logger) 
        {
            this.armOptions = armOptions.Value;
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        private static async Task<IReadOnlyList<T>> GetAzureItems<T>(Func<CancellationToken, Task<IPage<T>>> getFunc, 
                                                                     Func<string, CancellationToken, Task<IPage<T>>> nextFunc, 
                                                                     CancellationToken cancellationToken = default)
        {
            var total = new List<T>();

            var items = await getFunc(cancellationToken).ConfigureAwait(false);
            total.AddRange(items);

            var nextlink = items.NextPageLink;

            while (!string.IsNullOrWhiteSpace(nextlink))
            {
                items = await nextFunc(nextlink, cancellationToken).ConfigureAwait(false);
                total.AddRange(items);
                nextlink = items.NextPageLink;
            }
            return total;
        }

        public async Task<IReadOnlyList<Subscription>> GetSubscriptions()
        {
            try
            {
                var subClient = await clientFactory.GetSubscriptionClientAsync();
                logger.LogInformation("Get Subscription Repo");
                return await GetAzureItems(subClient.Subscriptions.ListAsync, subClient.Subscriptions.ListNextAsync);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get Subscription Repo error {error}", e.Message);
                throw;
            }
        }

        public async Task<IReadOnlyList<Location>> GetLocations(string subscriptionId)
        {
            try
            {
                logger.LogInformation("Get Locations for subscription: {subscriptionId}", subscriptionId);
                var subClient = await clientFactory.GetSubscriptionClientAsync();

                var locations = await subClient.Subscriptions.ListLocationsAsync(subscriptionId);
                return locations.ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Get Locations for subscription error: {error}", e.Message);
                throw;
            }
        }

        public async Task<ResourceGroup> CreateResoureGroup(string subscriptionId, string location, string resourceGroupName)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(subscriptionId);

                var resourceGroup = new ResourceGroup { Location = location };
                var result = await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(
                  resourceGroupName,
                  resourceGroup);
                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Create Resource Group error {error}", e.Message);
                throw;
            }
        }

        public async Task<DeploymentExtended> Deploy4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(depReq.SubscriptionId);

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = Get4x4TemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(armOptions.TemplateUrl),
                    Parameters = parameterTemplateJson
                };

                // initiates the deployement in async 
                var deploymentOutput = await resourceManagementClient.Deployments.BeginCreateOrUpdateAsync(
                  depReq.ApplicationName,
                  depReq.ApplicationName,
                  deployment
                );

                return deploymentOutput;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Create Template Deployment error {error}", e.Message);
                throw;
            }
        }

        public async Task<DeploymentExtended> GetDeploymentStatus(DeploymentStatusRequest deployStatus)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(deployStatus.SubscriptionId);

                // Get the deployment status, return output properties when Succeeded
                var deploymentOutput = await resourceManagementClient.Deployments.GetAsync(
                    deployStatus.ApplicationName, deployStatus.ApplicationName);

                //TODO: uncomment this to add this just to check status of each resource deployment
                //var deploymentDetails = await resourceManagementClient.DeploymentOperations.ListAsync(deployStatus.ApplicationName, 
                //    deployStatus.ApplicationName);

                return deploymentOutput;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Status of Deployment error {error}", e.Message);
                throw;
            }
        }

        public async Task<DeploymentValidateResult> Validate4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(depReq.SubscriptionId);

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = Get4x4TemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(armOptions.TemplateUrl),
                    Parameters = parameterTemplateJson
                };

                // This is for unit testing and validating the ARM template to be deployed
                var deploymentOutput = await resourceManagementClient.Deployments.ValidateAsync(
                  depReq.ApplicationName,
                  depReq.ApplicationName,
                  deployment
                );

                return deploymentOutput;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Validate Template Deployment error {error}", e.Message);
                throw;
            }
        }

        public async Task<DeploymentExtended> Deploy4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(depReq.SubscriptionId);

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = GetStreamAnalyticsTemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(armOptions.TemplateStreamAnalyticsUrl),
                    Parameters = parameterTemplateJson
                };

                // initiates the deployement in async 
                return await resourceManagementClient.Deployments.BeginCreateOrUpdateAsync(depReq.ResourceGroupName,
                    depReq.ResourceGroupName, deployment);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Create Stream Analytics error {error}", e.Message);
                throw;
            }
        }

        public async Task<DeploymentValidateResult> Validate4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq)
        {
            try
            {
                var resourceManagementClient = await clientFactory.GetResourceMangementClientAsync(depReq.SubscriptionId);

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = GetStreamAnalyticsTemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(armOptions.TemplateStreamAnalyticsUrl),
                    Parameters = parameterTemplateJson
                };

                // This is for unit testing and validating the ARM template to be deployed
                return await resourceManagementClient.Deployments.ValidateAsync(depReq.ResourceGroupName,
                    depReq.ResourceGroupName, deployment);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Validate Stream Analytics error {error}", e.Message);
                throw;
            }
        }

        public static string Get4x4TemplateParameterJson(DeploymentRequest depReq)
        {
            // generates parameter json string from the request object
            TemplateParameterRequest template = new TemplateParameterRequest()
            {
                Parameters = new ParameterModel()
                {
                    ClientId = new ParameterValue() { Value = depReq.ClientId },
                    ClientSecret = new ParameterValue() { Value = depReq.ClientSecret },
                    TenantId = new ParameterValue() { Value = depReq.TenantId },
                    DataPacketDesignerpPackageWebZipUri = new ParameterValue() { Value = depReq.DataPacketDesignerPackageWebZipUri },
                    DeviceManagementPortalPackageWebZipUri = new ParameterValue() { Value = depReq.DeviceManagementPortalPackageWebZipUri },
                }
            };
            return JsonConvert.SerializeObject(template);
        }

        public static string GetStreamAnalyticsTemplateParameterJson(StreamAnalyticsDeploymentRequest depReq)
        {
            // generates parameter json string from the request object
            TemplateStreamAnalyticsParameterRequest template = new TemplateStreamAnalyticsParameterRequest()
            {
                Parameters = new ParameterStreamAnalyticsModel()
                {
                    IotHubName = new ParameterStreamAnalyticsValue() { Value = depReq.IoTHubName },
                    CosmosDbAccountName = new ParameterStreamAnalyticsValue() { Value = depReq.CosmosDBAccountName },
                    CosmosDbName = new ParameterStreamAnalyticsValue() { Value = depReq.CosmosDBName },
                    CosmosDBMessageCollectionName = new ParameterStreamAnalyticsValue() { Value = depReq.CosmosDBMessageCollectionName },
                }
            };
            return JsonConvert.SerializeObject(template);
        }
    }
}
