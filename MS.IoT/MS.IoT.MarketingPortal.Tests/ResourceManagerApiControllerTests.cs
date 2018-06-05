using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using System.Web.Http.Results;
using Microsoft.Azure.Management.ResourceManager.Models;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Tests
{
    [TestClass]
    public class ResourceManagerApiControllerTests
    {

        [TestMethod]
        public async Task get_subscription_controller()
        {
            var servicePrinRepo = new Mock<IServicePrincipalRepository>();
            var resourceManagerRepo = new Mock<IResourceManagerRepository>();
            resourceManagerRepo.Setup(ur => ur.GetSubscriptions(AuthConfig.SessionItems.ManagementAuthToken))
                .Returns(MockedGetSubscriptions());

            var resCtrl = new ResourceManagerApiController(servicePrinRepo.Object, resourceManagerRepo.Object);
           
            var subscriptions = await resCtrl.GetSubscriptions();
            var contentResult = subscriptions as OkNegotiatedContentResult<SubscriptionResponseModel>;

            Assert.AreEqual(contentResult.Content.SubscriptionList[0].DisplayName, "Subscription MSDN");
            Assert.AreEqual(contentResult.Content.SubscriptionList[0].Id, "1234");
            Assert.AreEqual(contentResult.Content.SubscriptionList[0].State.ToString(), "Enabled");
            Assert.AreEqual(contentResult.Content.SubscriptionList[0].SubscriptionId, "1234");
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
       

        private Task<SubscriptionResponse> MockedGetSubscriptions()
        {
            var ret = new SubscriptionResponse()
            {
                SubscriptionList = new List<Domain.Model.Subscription>()
                {
                    new Domain.Model.Subscription()
                    {
                        DisplayName="Subscription MSDN",
                        Id="1234",
                        State=SubState.Enabled,
                        SubscriptionId="1234",
                    }
                }.ToList(),
            };

            return Task.FromResult(ret);
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
}
