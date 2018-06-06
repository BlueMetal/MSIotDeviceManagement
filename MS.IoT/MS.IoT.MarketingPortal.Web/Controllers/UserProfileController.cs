using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ITokenServices tokenServices;

        public UserProfileController(ITokenServices tokenServices)
        {
            this.tokenServices = tokenServices;
        }

        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            var serviceRoot = new Uri(tokenServices.DirectoryInstance);
            var activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () => await tokenServices.GetGraphTokenAsync());

            // use the token for querying the graph to get the user details

            IUser user = await activeDirectoryClient.Me.ExecuteAsync();

            return View(user);
        }

        public ActionResult GrantAccessToNonAdminUsers()
        {
            return Challenge(
                new AuthenticationProperties { Items = { new KeyValuePair<string, string>("IsAdmin", "true")}},
                OpenIdConnectDefaults.AuthenticationScheme);
        }        
    }
}
