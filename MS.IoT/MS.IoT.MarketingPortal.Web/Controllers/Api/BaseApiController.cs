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
using Microsoft.AspNetCore.Mvc;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    public class BaseApiController : Controller
    {
        public readonly IServicePrincipalRepository _servicePrincipalRepo;
        public readonly IResourceManagerRepository _resourceManagerRepo;
        
        public BaseApiController(IServicePrincipalRepository servicePrincipalRepo, IResourceManagerRepository resourceManagerRepo)
        {
            _servicePrincipalRepo = servicePrincipalRepo;
            _resourceManagerRepo = resourceManagerRepo;
        }
    }
}