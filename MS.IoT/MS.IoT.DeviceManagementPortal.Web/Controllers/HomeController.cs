using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.Common;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers
{
    /// <summary>
    /// Home Controller
    /// Main controller of the application. The navigation is done through Angular navigation
    /// </summary>
    [Authorize]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Main Controller
        /// </summary>
        /// <param name="userProfile">User Profile Service</param>
        public HomeController(UserProfileService userProfile)
            : base(userProfile)
        {
        }

        /// <summary>
        /// Index
        /// Angular single page endpoint
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}