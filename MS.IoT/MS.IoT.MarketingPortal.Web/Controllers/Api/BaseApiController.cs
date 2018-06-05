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

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    public class BaseApiController : ApiController
    {
        public readonly IServicePrincipalRepository _servicePrincipalRepo;
        public readonly IResourceManagerRepository _resourceManagerRepo;
        
        public BaseApiController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo)
        {
            this._servicePrincipalRepo = servicePrincipalRepo;
            this._resourceManagerRepo = resourceManagerRepo;
        }
    }
}