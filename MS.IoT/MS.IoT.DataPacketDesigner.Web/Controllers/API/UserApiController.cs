using Microsoft.Azure.ActiveDirectory.GraphClient;
using MS.IoT.Common;
using MS.IoT.DataPacketDesigner.Web.Helpers;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace MS.IoT.DataPacketDesigner.Web.Controllers.API
{
    /// <summary>
    /// User Controller API
    /// </summary>
    [Authorize]
    [RoutePrefix("api/user")]
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
        [ResponseType(typeof(IUser))]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            Domain.Model.User currentUser = await EnsureCurrentUser();

            if (currentUser == null)
            {
                Log.Error("API GetCurrentUser error: User not found.");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(currentUser);
        }
    }
}