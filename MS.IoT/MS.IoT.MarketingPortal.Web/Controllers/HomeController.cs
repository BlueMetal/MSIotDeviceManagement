using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
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
using MS.IoT.Common;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
   
    public class HomeController : Controller
    {      
        public readonly IServicePrincipalRepository _servicePrincipalRepo;
        public readonly IResourceManagerRepository _resourceManagerRepo;

        public HomeController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo)
        {
            this._servicePrincipalRepo = servicePrincipalRepo;
            this._resourceManagerRepo = resourceManagerRepo;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            
            ViewBag.Message = "Your application description page.";          

            return View(new WatchVideoModel() { VideoUrl = AppConfig.ConfigurationItems.WatchVideoUrl });                  
        }

        [Authorize]
        public async Task<ActionResult> GetStarted()
        {                      
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public async Task<ActionResult> Deploy()
        {
            Log.Information("Application Loaded- auth value {AuthCode}", AuthConfig.SessionItems.AuthCode);
            Log.Information("Application Loaded- graph token value {GraphToken}", AuthConfig.SessionItems.GraphAuthToken);
            Log.Information("Application Loaded- managament token value {ManagemenToken}", AuthConfig.SessionItems.ManagementAuthToken);

            var authcode = AuthConfig.SessionItems.AuthCode;
            if (authcode == null)
            {
                RefreshSession("/Home/Deploy");
                Log.Information("Application Refresh session- auth value {AuthCode}", AuthConfig.SessionItems.AuthCode);
                Log.Information("Application Refresh session- graph token value {GraphToken}", AuthConfig.SessionItems.GraphAuthToken);
                Log.Information("Application Refresh session- managament token value {ManagemenToken}", AuthConfig.SessionItems.ManagementAuthToken);
            }
            return View();
        }

        public void RefreshSession(string path)
        {
            Log.Information("Application Refresh session called");
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = path },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult Faqs()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }    
    }
}