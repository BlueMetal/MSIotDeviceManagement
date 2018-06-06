using System;
using MS.IoT.Repositories;
using Moq;
using MS.IoT.Domain.Interface;
using MS.IoT.MarketingPortal.Web.Controllers;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Model;
using System.Threading.Tasks;
using MS.IoT.MarketingPortal.Web.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Logging;
using MS.IoT.MarketingPortal.Web;
using MS.IoT.MarketingPortal.Web.Helpers;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace MS.IoT.MarketingPortal.Tests
{
    public class ResourceManagerApiControllerTests
    {

        [Fact]
        public async Task get_subscription_controller()
        {
            var servicePrinRepo = new Mock<IServicePrincipalRepository>();
            var resourceManagerRepo = new Mock<IResourceManagerRepository>();
            resourceManagerRepo.Setup(ur => ur.GetSubscriptions())
                .Returns(MockedGetSubscriptions());

            var opts = new UrlOptions();
            var logger = new LoggerFactory().CreateLogger<ResourceManagerApiController>();

            var resCtrl = new ResourceManagerApiController(servicePrinRepo.Object, resourceManagerRepo.Object, Options.Create(opts), logger);
           
            var subscriptions = await resCtrl.GetSubscriptions();
            var contentResult = subscriptions.OkayContent<SubscriptionResponseModel>();

            Assert.Equal("Subscription MSDN", contentResult.SubscriptionList[0].DisplayName);
            Assert.Equal("1234", contentResult.SubscriptionList[0].Id);
            Assert.Equal("Enabled", contentResult.SubscriptionList[0].State.ToString());
            Assert.Equal("1234", contentResult.SubscriptionList[0].SubscriptionId);
        }

        //[TestMethod]
        //[Ignore]
        //public async Task deploy_4x4iotsolution_controller()
        //{
        //    var servicePrinRepo = new Mock<IServicePrincipalRepository>();
        //    var resourceManagerRepo = new Mock<IResourceManagerRepository>();

        //    DeploymentRequest deployReq = new DeploymentRequest()
        //    {
        //        ApplicationName = "msiotunittest",
        //        ClientId = "1234",
        //        ClientSecret = "12345",
        //        SubscriptionId = "123456subid",
        //        TenantId = "12345",
        //        Location = "eastus",
        //        DataPacketDesignerPackageWebZipUri = "url123"
        //    };

        //    resourceManagerRepo.Setup(ur => ur.Deploy4x4MSIoTSolutionUsingAzureRMTemplate(deployReq, AuthConfig.SessionItems.ManagementAuthToken))
        //        .Returns(MockedDeploy4x4IoTSolution());

        //    var resCtrl = new ResourceManagerApiController(servicePrinRepo.Object, resourceManagerRepo.Object);

        //    var depModel = new DeploymentModel()
        //    {
        //        ApplicationName= "msiotunittest",
        //        ClientId = "1234",
        //        ClientSecret = "12345",
        //        SubscriptionId = "123456subid",
        //        TenantId = "12345",
        //        Location = "eastus"
        //    };

        //    var deploymentOutput = await resCtrl.Deploy4x4IoTSolution(depModel);
            
        //    var contentResult = deploymentOutput as OkNegotiatedContentResult<DeploymentTemplateOutputModel>;

        //    Assert.AreEqual(contentResult.Content.IotHubName, "testiothubname");
            
        //}


        private Task<IReadOnlyList<Subscription>> MockedGetSubscriptions()
        {

            var s = new List<Subscription>()
            {
                new Subscription("1234", displayName: "Subscription MSDN", state: SubscriptionState.Enabled, subscriptionId: "1234")
            };


            return Task.FromResult<IReadOnlyList<Subscription>>(s);
        }

        //private Task<DeploymentExtended> MockedDeploy4x4IoTSolution()
        //{
        //    var ret = new DeploymentExtended()
        //    {
        //        Id="deploymentidTest",
        //        Name="deploymentnametest",
        //        Properties=new DeploymentPropertiesExtended()
        //        {
        //            Outputs= "{{'dataPacketDesignerUrl': {'type': 'String','value': 'testurl'},'iotHubName': {'type': 'String','value': 'testiothub'},'cosmosDBAccountName': {'type': 'String','value': 'testcosmosdbname'},'cosmosDBAccountEndPoint': {'type': 'String','value': 'testcosmosdbendpoint'},'cosmosDBAccountKey': {'type': 'String','value': 'testcosmosdbkey'}}}"
        //        }          
        //    };

        //    return Task.FromResult(ret);
        //}
    }
    static class TestExtensions
    {
        public static T OkayContent<T>(this IActionResult actionResult)
        {
            return (T)((actionResult as OkObjectResult)?.Value);
        }

        public static void AssertOkValueType<T>(this IActionResult actionResult)
        {
            Assert.IsAssignableFrom<T>((actionResult as OkObjectResult)?.Value);
        }
    }
}
