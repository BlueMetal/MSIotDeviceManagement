using MS.IoT.DataPacketDesigner.Web.Helpers;
using MS.IoT.DataPacketDesigner.Web.Models;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using MS.IoT.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.DataPacketDesigner.Web.Controllers
{
    [Authorize]
    public class TemplateController : BaseController
    {
        public TemplateController(IRepository<Template> templateRepo, UserProfileProvider userProfile)
            : base(templateRepo, userProfile)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageTemplate()
        {
            //Get TemplateId 
            //this.RouteData.Values["id"].ToString()

            return PartialView();
        }
    }
}