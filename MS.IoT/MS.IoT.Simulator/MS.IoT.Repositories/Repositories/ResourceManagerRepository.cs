using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.Rest.Azure;
using MS.IoT.Common;

namespace MS.IoT.Repositories
{

    public class ResourceManagerRepository : IResourceManagerRepository
    {
        private string _managementEndpoint;
        private string _armTemplateUrl;
        private string _armTemplateStreamAnalyticsUrl;

        public ResourceManagerRepository(string managementEndpoint, string armTemplateUrl, string armTemplateStreamAnalyticsUrl)
        {
            _managementEndpoint = managementEndpoint;
            _armTemplateUrl = armTemplateUrl;
            _armTemplateStreamAnalyticsUrl = armTemplateStreamAnalyticsUrl;
        }

        public async Task<SubscriptionResponse> GetSubscriptions(string token)
        {
            try
            {
                Log.Information("Get Subscription Repo token {@data}", token);
                var client = new RestClient(_managementEndpoint);
                var request = new RestRequest("/subscriptions", Method.GET);
                request.AddQueryParameter("api-version", "2017-05-10");

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var response = await client.ExecuteTaskAsync(request);
                if (response.StatusCode.ToString().Equals("OK"))
                {
                    var subscriptions = JsonConvert.DeserializeObject<SubscriptionResponse>(response.Content);
                    Log.Debug("Get Subscription Repo data {@data}", subscriptions);
                    return subscriptions;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    throw new Exception(error.Error.Message);
                }
            }
            catch (Exception e)
            {
                Log.Error("Get Subscription Repo error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<LocationResponse> GetLocations(string subscriptionId, string token)
        {
            try
            {
                Log.Information("Get Locations for subscription: {subscriptionId}", subscriptionId);
                var client = new RestClient(_managementEndpoint);
                var endPoint = String.Format("/subscriptions/{0}/locations", subscriptionId);
                var request = new RestRequest(endPoint, Method.GET);
                request.AddQueryParameter("api-version", "2017-05-10");

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var response = await client.ExecuteTaskAsync(request);
                var locations = JsonConvert.DeserializeObject<LocationResponse>(response.Content);
                return locations;
            }
            catch (Exception e)
            {
                Log.Error("Get Locations for subscription error: {@error}", e.Message);
                throw e;
            }
        }

        public async Task<ResourceGroup> CreateResoureGroup(string subscriptionId, string location, string resourceGroupName, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = subscriptionId };

                var resourceGroup = new ResourceGroup { Location = location };
                var result = await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(
                  resourceGroupName,
                  resourceGroup);
                return result;
            }
            catch (Exception e)
            {
                Log.Error("Create Resource Group error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeploymentExtended> Deploy4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = depReq.SubscriptionId };

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = Get4x4TemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(_armTemplateUrl),
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
                Log.Error("Create Template Deployment error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeploymentExtended> GetDeploymentStatus(DeploymentStatusRequest deployStatus, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = deployStatus.SubscriptionId };

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
                Log.Error("Status of Deployment error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeploymentValidateResult> Validate4x4MSIoTSolutionUsingAzureRMTemplate(DeploymentRequest depReq, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = depReq.SubscriptionId };

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = Get4x4TemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(_armTemplateUrl),
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
                Log.Error("Validate Template Deployment error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeploymentExtended> Deploy4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = depReq.SubscriptionId };

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = GetStreamAnalyticsTemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(_armTemplateStreamAnalyticsUrl),
                    Parameters = parameterTemplateJson
                };

                // initiates the deployement in async 
                return await resourceManagementClient.Deployments.BeginCreateOrUpdateAsync(depReq.ResourceGroupName,
                    depReq.ResourceGroupName, deployment);
            }
            catch (Exception e)
            {
                Log.Error("Create Stream Analytics error {@error}", e.Message);
                throw e;
            }
        }

        public async Task<DeploymentValidateResult> Validate4x4StreamAnalyticsUsingAzureRMTemplate(StreamAnalyticsDeploymentRequest depReq, string token)
        {
            try
            {
                var credential = new TokenCredentials(token);
                var resourceManagementClient = new ResourceManagementClient(credential)
                { SubscriptionId = depReq.SubscriptionId };

                // generates a parameter json required for ARM template deployment
                var parameterTemplateJson = GetStreamAnalyticsTemplateParameterJson(depReq);

                var deployment = new Deployment();
                deployment.Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    TemplateLink = new TemplateLink(_armTemplateStreamAnalyticsUrl),
                    Parameters = parameterTemplateJson
                };

                // This is for unit testing and validating the ARM template to be deployed
                return await resourceManagementClient.Deployments.ValidateAsync(depReq.ResourceGroupName,
                    depReq.ResourceGroupName, deployment);
            }
            catch (Exception e)
            {
                Log.Error("Validate Stream Analytics error {@error}", e.Message);
                throw e;
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
