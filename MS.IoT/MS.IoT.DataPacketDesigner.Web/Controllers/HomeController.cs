using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System.Web.Mvc;

namespace MS.IoT.DataPacketDesigner.Web.Controllers
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
        /// <param name="templateRepo">Repository Template Service</param>
        /// <param name="userProfile">User Profile Service</param>
        public HomeController(ICosmosDBRepository<Template> templateRepo, UserProfileService userProfile)
            : base(templateRepo, userProfile)
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