using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Logging;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// User Controller API
    /// </summary>
    [Authorize]
    [Route("api/user")]
    public class UserApiController : BaseApiController
    {
        private readonly ILogger<UserApiController> logger;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="userProfile">User Profile Service</param>
        public UserApiController(IUserProfileService userProfile, ILogger<UserApiController> logger)
            : base(userProfile)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Endpoint to retrieve an object of the current user
        /// </summary>
        /// <returns>User Object</returns>
        [Route("current")]
        [Produces(typeof(IUser))]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();

            if (currentUser == null)
            {
                logger.LogError("API GetCurrentUser error: User not found.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(currentUser);
        }
    }
}