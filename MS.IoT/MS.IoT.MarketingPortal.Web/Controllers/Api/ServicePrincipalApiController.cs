using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MS.IoT.Domain.Model;
using MS.IoT.Common;
using System.Net.Http;
using System.Net;
using MS.IoT.MarketingPortal.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Route("api/serviceprincipal")]
    [Authorize]
    public class ServicePrincipalApiController : BaseApiController
    {
        private readonly ITokenServices tokenServices;
        private readonly ILogger<ServicePrincipalApiController> logger;

        public ServicePrincipalApiController(IServicePrincipalRepository servicePrincipalRepo, 
                                             IResourceManagerRepository resourceManagerRepo,
                                             ITokenServices tokenServices,
                                             ILogger<ServicePrincipalApiController> logger) 
            : base(servicePrincipalRepo, resourceManagerRepo)
        {
            this.tokenServices = tokenServices;
            this.logger = logger;
        }    

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateAzureADApplication([FromBody]DeploymentModel deployApp)
        {
            try
            {
                logger.LogInformation("Create Azure AD Application: {application}", deployApp.ApplicationName);
                var appUri = String.Format("https://{0}/{1}", tokenServices.TenantId, deployApp.ApplicationName);

                var app = await _servicePrincipalRepo.CreateAppAndServicePrincipal(deployApp.ApplicationName, appUri, "msiot123",
                                                                                   tokenServices.TenantId);

                ServicePrincipalResponseModel spm = new ServicePrincipalResponseModel()
                {
                    ApplicationName = app.App.DisplayName,
                    AppObjectId = app.App.ObjectId,
                    AppIdUrl = app.App.IdentifierUris.First(),
                    ClientId = app.App.AppId,
                    ClientSecret = app.AppClientSecret,
                    TenantId = tokenServices.TenantId
                };

                logger.LogInformation("Create Azure AD Application Completed: App clientid{application}", spm.ClientId);
                return Ok(spm);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Create Azure AD Application: - Exception {message}", e.Message);
                throw;
            }
        }

        [Route("application/update")]
        [HttpPut]
        public async Task UpdateAzureADApplication([FromBody]UpdateApplicationModel updateApp)
        {
            try
            {
                logger.LogInformation("Update Azure AD Application: {application}", updateApp.HomePage);
                var updateModel = new UpdateApplicationRequest()
                {
                    Homepage = updateApp.HomePage,
                    ReplyUrls = updateApp.ReplyUrls
                };
                await _servicePrincipalRepo.UpdateAzureADApplication(updateApp.AppObjectId, updateModel, tokenServices.TenantId);
                logger.LogInformation("Update Azure AD Application Reply url updated: {application}", updateApp.HomePage);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Update Azure AD Application: - Exception {message}", e.Message);
            }
        }
    }
}