using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using System;
using System.Threading.Tasks;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// Base Controller for the API
    /// </summary>
    public class BaseApiController : Controller
    {
        //Service members
        public readonly IUserProfileService _userProfile;

        //Members
        private Domain.Model.User _currentUser;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="userProfile">User Profile Service</param>
        public BaseApiController(IUserProfileService userProfile)
        {
            _userProfile = userProfile;
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
                throw new Exception("User not found.");
            }
            return _currentUser;
        }
    }
}