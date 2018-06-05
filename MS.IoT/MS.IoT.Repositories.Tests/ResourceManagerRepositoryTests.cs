using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;

namespace MS.IoT.Repositories.Tests
{
    [TestClass]
    public class ResourceManagerRepositoryTests
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

        [TestInitialize]
        public async Task SetupAsync()
        {
            string authContextURL = "https://login.microsoftonline.com/" + tenantId;
            var authenticationContext = new AuthenticationContext(authContextURL);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync(managementUrl, credential);
            managementAuthToken = result.AccessToken;
        }

        [TestMethod]
        public async Task get_subscriptions_not_null()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task get_subscriptions_token_invalid_null()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken+"safsa");
            Assert.IsNull(subscriptions.SubscriptionList);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Authentication failed. The 'Authorization' header is missing the access token.")]
        public async Task get_subscriptions_token_null()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(null);
            Assert.IsNull(subscriptions.SubscriptionList);
        }

        [TestMethod]
        public async Task get_locations_not_null()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var locations = await repo.GetLocations(subscriptions.SubscriptionList[0].SubscriptionId, managementAuthToken);
            Assert.IsNotNull(locations.LocationList);
        }

        [TestMethod]
        public async Task create_resource_group()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", "msiotunittest", managementAuthToken);
            Assert.AreEqual(resourceGroup.Name, "msiotunittest");
            Assert.AreEqual(resourceGroup.Location, "eastus");
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.Rest.ValidationException))]
        public async Task create_resource_group_validation_exception()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", "msiotu nittest", managementAuthToken);
        }

        [TestMethod]
        public async Task validate_4x4msiotsoultion_azureRM_template()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", "msiotunittest", managementAuthToken);
            Assert.AreEqual(resourceGroup.Name, "msiotunittest");
            Assert.AreEqual(resourceGroup.Location, "eastus");

            DeploymentRequest deployReq = new DeploymentRequest()
            {
                ApplicationName = "msiotunittest",
                ClientId = "1234",
                ClientSecret = "12345",
                SubscriptionId = subscriptions.SubscriptionList[0].SubscriptionId,
                TenantId = "12345",
                Location = "eastus",
                DataPacketDesignerPackageWebZipUri=dataPacketDesignerUrl,
                DeviceManagementPortalPackageWebZipUri=deviceManagementPortalUrl
            };

            var deployment=await repo.Validate4x4MSIoTSolutionUsingAzureRMTemplate(deployReq,managementAuthToken);
            Assert.IsNull(deployment.Error);
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.Rest.Azure.CloudException))]
        public async Task validate_4x4msiotsoultion_azureRM_template_Exception()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", 
                "msiotunittest", managementAuthToken+"123");
            Assert.AreEqual(resourceGroup.Name, "msiotunittest");
            Assert.AreEqual(resourceGroup.Location, "eastus");


            DeploymentRequest deployReq = new DeploymentRequest()
            {
                ApplicationName = "msiotunittest",
                ClientId = "1234",
                ClientSecret = "12345",
                SubscriptionId = subscriptions.SubscriptionList[0].SubscriptionId,
                TenantId = "12345",
                Location = "eastus",
                DataPacketDesignerPackageWebZipUri = dataPacketDesignerUrl
            };

            var deployment = await repo.Validate4x4MSIoTSolutionUsingAzureRMTemplate(deployReq, managementAuthToken);
            Assert.IsNull(deployment.Error);
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
        }

        [TestMethod]
        public async Task validate_streamAnalytics_azureRM_template()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", 
                "msiotunittest", managementAuthToken);
            Assert.AreEqual(resourceGroup.Name, "msiotunittest");
            Assert.AreEqual(resourceGroup.Location, "eastus");

            StreamAnalyticsDeploymentRequest deployReq = new StreamAnalyticsDeploymentRequest()
            {
                IoTHubName = "iothubtestms4x4",
                CosmosDBAccountName = "cosmostestms4x4",
                CosmosDBName = "cosmostestdb",
                CosmosDBMessageCollectionName = "testcollection",
                SubscriptionId = subscriptions.SubscriptionList[0].SubscriptionId,
                ResourceGroupName = "msiotunittest",
                Location = "eastus"
            };

            var deployment = await repo.Validate4x4StreamAnalyticsUsingAzureRMTemplate
                (deployReq, managementAuthToken);

            Assert.IsNull(deployment.Error);
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.Rest.Azure.CloudException))]
        public async Task validate_streamAnalytics_azureRM_template_Exception()
        {
            ResourceManagerRepository repo = new ResourceManagerRepository(managementUrl, armTemplateUrl, armTemplateStreamAnalyticsUrl);
            var subscriptions = await repo.GetSubscriptions(managementAuthToken);
            Assert.IsNotNull(subscriptions.SubscriptionList);

            var resourceGroup = await repo.CreateResoureGroup(subscriptions.SubscriptionList[0].SubscriptionId, "eastus", 
                "msiotunittest", managementAuthToken+"123");
            Assert.AreEqual(resourceGroup.Name, "msiotunittest");
            Assert.AreEqual(resourceGroup.Location, "eastus");

            StreamAnalyticsDeploymentRequest deployReq = new StreamAnalyticsDeploymentRequest()
            {
                IoTHubName = "iothubtestms4x4",
                CosmosDBAccountName = "cosmostestms4x4",
                CosmosDBName = "cosmostestdb",
                CosmosDBMessageCollectionName = "testcollection",
                SubscriptionId = subscriptions.SubscriptionList[0].SubscriptionId,
                ResourceGroupName = "msiotunittest",
                Location = "eastus"
            };

            var deployment = await repo.Validate4x4StreamAnalyticsUsingAzureRMTemplate
                (deployReq, managementAuthToken);

            Assert.IsNull(deployment.Error);
            Assert.AreEqual("Succeeded", deployment.Properties.ProvisioningState);
        }
    }
}

