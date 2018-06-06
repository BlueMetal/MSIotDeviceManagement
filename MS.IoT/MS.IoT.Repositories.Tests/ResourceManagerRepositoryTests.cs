using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using Xunit;

public class ResourceManagerRepositoryTests : IAsyncLifetime
{
    public static readonly string tenantId = "72f43d57-b980-4152-b703-e2d8666a3ea9";
    public static readonly string clientId = "fd17f56b-cdd7-467f-ad3a-6b8b7bc7bc72";
    public static readonly string clientSecret = "ck30+q2nXvjfvAct8XC0ivWuZ3i6j/+n1SILJ4uGpGM=";
    public static string managementAuthToken;
    public static readonly string managementUrl = "https://management.azure.com/";
    public static readonly string armTemplateUrl = "https://msiotsolutiondev.blob.core.windows.net/template/MSIoTProvisioningTemplate.json";
    public static readonly string armTemplateStreamAnalyticsUrl = "https://msiotsolutiondev.blob.core.windows.net/template/streamAnalyticsAzuredeploy.json";
    public static readonly string dataPacketDesignerUrl = "https://msiotsolutiondev.blob.core.windows.net/webpublish/MS.IoT.DataPacketDesigner.Web.zip";
    public static readonly string deviceManagementPortalUrl = "https://msiotsolutiondev.blob.core.windows.net/webpublish/MS.IoT.DeviceManagementPortal.Web.zip";

    public async Task InitializeAsync()
    {
        string authContextURL = "https://login.microsoftonline.com/" + tenantId;
        var authenticationContext = new AuthenticationContext(authContextURL);
        var credential = new ClientCredential(clientId, clientSecret);
        var result = await authenticationContext.AcquireTokenAsync(managementUrl, credential);
        managementAuthToken = result.AccessToken;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task create_resource_group()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        var resourceGroup = await repo.CreateResoureGroup(subscriptions[0]
                                                              .SubscriptionId,
                                                          "eastus",
                                                          "msiotunittest");
        Assert.Equal("msiotunittest", resourceGroup.Name);
        Assert.Equal("eastus", resourceGroup.Location);
    }

    [Fact]
    public async Task create_resource_group_validation_exception()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        await Assert.ThrowsAsync<ValidationException>(async () => await repo.CreateResoureGroup(subscriptions[0].SubscriptionId,
                                                                                                "eastus",
                                                                                                "msiotu nittest"));
    }

    [Fact]
    public async Task get_locations_not_null()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        var locations = await repo.GetLocations(subscriptions[0]
                                                    .SubscriptionId);
        Assert.NotNull(locations);
    }

    [Fact]
    public async Task get_subscriptions_not_null()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);
    }

    [Fact]
    public async Task validate_4x4msiotsoultion_azureRM_template()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        var resourceGroup = await repo.CreateResoureGroup(subscriptions[0]
                                                              .SubscriptionId,
                                                          "eastus",
                                                          "msiotunittest");
        Assert.Equal("msiotunittest", resourceGroup.Name);
        Assert.Equal("eastus", resourceGroup.Location);

        DeploymentRequest deployReq = new DeploymentRequest
        {
            ApplicationName = "msiotunittest",
            ClientId = "1234",
            ClientSecret = "12345",
            SubscriptionId = subscriptions[0]
                .SubscriptionId,
            TenantId = "12345",
            Location = "eastus",
            DataPacketDesignerPackageWebZipUri = dataPacketDesignerUrl,
            DeviceManagementPortalPackageWebZipUri = deviceManagementPortalUrl
        };

        var deployment = await repo.Validate4x4MSIoTSolutionUsingAzureRMTemplate(deployReq);
        Assert.Null(deployment.Error);
        Assert.Equal("Succeeded", deployment.Properties.ProvisioningState);
    }

    [Fact]
    public async Task validate_4x4msiotsoultion_azureRM_template_Exception()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        await Assert.ThrowsAsync<CloudException>(async () => await repo.CreateResoureGroup(subscriptions[0].SubscriptionId,
                                                                                           "eas24455tus",
                                                                                           "msiotunittest"));

        var deployReq = new DeploymentRequest
        {
            ApplicationName = "msiotunittest",
            ClientId = "1234",
            ClientSecret = "12345",
            SubscriptionId = subscriptions[0]
                .SubscriptionId,
            TenantId = "12345",
            Location = "eastus",
            DataPacketDesignerPackageWebZipUri = dataPacketDesignerUrl
        };

        var deployment = await repo.Validate4x4MSIoTSolutionUsingAzureRMTemplate(deployReq);
        Assert.Null(deployment.Error);
        Assert.Equal("Succeeded", deployment.Properties.ProvisioningState);
    }

    [Fact]
    public async Task validate_streamAnalytics_azureRM_template()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        var resourceGroup = await repo.CreateResoureGroup(subscriptions[0].SubscriptionId,
                                                          "eastus",
                                                          "msiotunittest");
        Assert.Equal("msiotunittest", resourceGroup.Name);
        Assert.Equal("eastus", resourceGroup.Location);

        StreamAnalyticsDeploymentRequest deployReq = new StreamAnalyticsDeploymentRequest
        {
            IoTHubName = "iothubtestms4x4",
            CosmosDBAccountName = "cosmostestms4x4",
            CosmosDBName = "cosmostestdb",
            CosmosDBMessageCollectionName = "testcollection",
            SubscriptionId = subscriptions[0]
                .SubscriptionId,
            ResourceGroupName = "msiotunittest",
            Location = "eastus"
        };

        var deployment = await repo.Validate4x4StreamAnalyticsUsingAzureRMTemplate
                             (deployReq);

        Assert.Null(deployment.Error);
        Assert.Equal("Succeeded", deployment.Properties.ProvisioningState);
    }

    [Fact]
    public async Task validate_streamAnalytics_azureRM_template_Exception()
    {
        var repo = CreateRepository();
        var subscriptions = await repo.GetSubscriptions();
        Assert.NotNull(subscriptions);

        await Assert.ThrowsAsync<CloudException>(async () => await repo.CreateResoureGroup(subscriptions[0]
                                                                                               .SubscriptionId,
                                                                                           "e4455astus",
                                                                                           "msiotunittest"));

        var deployReq = new StreamAnalyticsDeploymentRequest
        {
            IoTHubName = "iothubtestms4x4",
            CosmosDBAccountName = "cosmostestms4x4",
            CosmosDBName = "cosmostestdb",
            CosmosDBMessageCollectionName = "testcollection",
            SubscriptionId = subscriptions[0]
                .SubscriptionId,
            ResourceGroupName = "msiotunittest",
            Location = "eastus"
        };

        var deployment = await repo.Validate4x4StreamAnalyticsUsingAzureRMTemplate(deployReq);

        Assert.Null(deployment.Error);
        Assert.Equal("Succeeded", deployment.Properties.ProvisioningState);
    }

    ResourceManagerRepository CreateRepository()
    {
        var armClient = new ArmClientFactory(() => Task.FromResult(managementAuthToken), managementAuthToken);
        var logger = new LoggerFactory().CreateLogger<ResourceManagerRepository>();
        var opts = new ArmOptions
        {
            TemplateStreamAnalyticsUrl = armTemplateStreamAnalyticsUrl,
            TemplateUrl = armTemplateUrl
        };

        return new ResourceManagerRepository(Options.Create(opts), armClient, logger);
    }
}