using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ActiveDirectoryClient activeDirectoryClient;

        public UserProfileController(ActiveDirectoryClient activeDirectoryClient)
        {
            this.activeDirectoryClient = activeDirectoryClient;
        }

        // GET: UserProfile
        public async Task<IActionResult> Index()
        {
            var user = await activeDirectoryClient.Me.ExecuteAsync();

            return View(user);
        }

    }
}
