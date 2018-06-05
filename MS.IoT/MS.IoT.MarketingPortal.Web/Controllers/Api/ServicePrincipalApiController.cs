using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.MarketingPortal.Web.Models;
using MS.IoT.Domain.Interface;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Http;
using MS.IoT.Domain.Model;
using MS.IoT.Common;
using System.Net.Http;
using System.Net;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [RoutePrefix("api/serviceprincipal")]
    [Authorize]
    public class ServicePrincipalApiController : BaseApiController
    {     
        public ServicePrincipalApiController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo) : base(servicePrincipalRepo, resourceManagerRepo)
        {
        }    

        [Route("create")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateAzureADApplication(DeploymentModel deployApp)
        {
            try
            {
                Log.Information("Create Azure AD Application: {application}", deployApp.ApplicationName);
                var appUri = String.Format("https://{0}/{1}", AuthConfig.SessionItems.GraphTargetTenant, deployApp.ApplicationName);

                var app = await _servicePrincipalRepo.CreateAppAndServicePrincipal(deployApp.ApplicationName, appUri, "msiot123",
                    AuthConfig.SessionItems.GraphTargetTenant, AuthConfig.SessionItems.GraphAuthToken);

                ServicePrincipalResponseModel spm = new ServicePrincipalResponseModel()
                {
                    ApplicationName = app.App.DisplayName,
                    AppObjectId = app.App.ObjectId,
                    AppIdUrl = app.App.IdentifierUris.First(),
                    ClientId = app.App.AppId,
                    ClientSecret = app.AppClientSecret,
                    TenantId = AuthConfig.SessionItems.GraphTargetTenant
                };

                Log.Information("Create Azure AD Application Completed: App clientid{application}", spm.ClientId);
                return Ok(spm);
            }
            catch (Exception e)
            {
                Log.Error("Create Azure AD Application: - Exception {message}", e.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
        }

        [Route("application/update")]
        [HttpPut]
        public async Task UpdateAzureADApplication(UpdateApplicationModel updateApp)
        {
            try
            {
                Log.Information("Update Azure AD Application: {application}", updateApp.HomePage);
                var updateModel = new UpdateApplicationRequest()
                {
                    Homepage = updateApp.HomePage,
                    ReplyUrls = updateApp.ReplyUrls
                };
                await _servicePrincipalRepo.UpdateAzureADApplication(updateApp.AppObjectId, updateModel, AuthConfig.SessionItems.GraphTargetTenant, AuthConfig.SessionItems.GraphAuthToken);
                Log.Information("Update Azure AD Application Reply url updated: {application}", updateApp.HomePage);
            }
            catch (Exception e)
            {
                Log.Error("Update Azure AD Application: - Exception {message}", e.Message);
            }
        }
    }
}