using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using MS.IoT.DeviceManagementPortal.Web.Helpers;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.API
{
    /// <summary>
    /// User Controller API
    /// </summary>
    [Authorize]
    [Route("api/user")]
    public class UserApiController : BaseApiController
    {
        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="userProfile">User Profile Service</param>
        public UserApiController(IUserProfileService userProfile)
             : base(userProfile)
        {
        }

        /// <summary>
        /// Endpoint to retrieve an object of the current user
        /// </summary>
        /// <returns>User Object</returns>
        [Route("current")]
        [Produces(typeof(IUser))]
        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            //Domain.Model.User currentUser = await EnsureCurrentUser();

            //if (currentUser == null)
            //{
            //    Log.Error("API GetCurrentUser error: User not found.");
            //    return StatusCode(HttpStatusCode.InternalServerError);
            //}
            //return Ok(currentUser);
            return Ok(true);
        }
    }
}