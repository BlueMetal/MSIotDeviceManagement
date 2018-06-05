using Microsoft.Azure.ActiveDirectory.GraphClient;
using MS.IoT.Common;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.Api
{
    [HostAuthentication("OAuth2Bearer")]
    public class BaseApiController : ApiController
    {
        //Service members
        public readonly IUserProfileService _userProfile;

        //Members
        private Domain.Model.User _currentUser;

        public BaseApiController(IUserProfileService userProfile)
        {
            this._userProfile = userProfile;
        }

        /// <summary>
        /// Call the UserService to retrieve the current user
        /// Throws an exception if the current user is not found.
        /// </summary>
        /// <returns>User object</returns>
        protected async Task<Domain.Model.User> EnsureCurrentUser()
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
    }
}