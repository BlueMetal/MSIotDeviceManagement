using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS.IoT.DeviceManagementPortal.Web.Helpers;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserProfileService userProfile;

        public HomeController(IUserProfileService userProfile)
        {
            this.userProfile = userProfile;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous	]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
