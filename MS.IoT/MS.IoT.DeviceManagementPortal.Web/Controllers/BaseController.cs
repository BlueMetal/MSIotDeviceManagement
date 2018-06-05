using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.Common;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers
{
    /// <summary>
    /// Base Controller
    /// Provide an access to the current user for all MVC controllers
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        //Services members
        private UserProfileService _userProfile;
        private Domain.Model.User _currentUser;

        //Properties
        public Domain.Model.User CurrentUser { get { return _currentUser; } }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="userProfile">User Profile Service</param>
        public BaseController(UserProfileService userProfile)
        {
            _userProfile = userProfile;
        }

        /// <summary>
        /// Retrieve the user
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EnsureCurrentUser().ConfigureAwait(false);
        }

        /// <summary>
        /// Call the UserService to retrieve the current user
        /// </summary>
        /// <returns>User object</returns>
        private async Task<Domain.Model.User> EnsureCurrentUser()
        {
            if (_currentUser != null)
                return _currentUser;

            IUser currentClaimUser = await _userProfile.GetCurrentUser();
            _currentUser = new Domain.Model.User() { Id = currentClaimUser.UserPrincipalName, Name = currentClaimUser.DisplayName };

            if (_currentUser == null)
            {
                Log.Error("User not found.");
                throw new Exception("User not found.");
            }
            return _currentUser;
        }

        public void RefreshSession(string path)
        {
            Log.Information("Application Refresh session called");
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = path },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}