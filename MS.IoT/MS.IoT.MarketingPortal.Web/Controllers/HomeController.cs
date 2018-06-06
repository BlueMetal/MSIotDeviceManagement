using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MS.IoT.Domain.Interface;
using MS.IoT.MarketingPortal.Web.Models;

namespace MS.IoT.MarketingPortal.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public readonly IServicePrincipalRepository _servicePrincipalRepo;
        public readonly IResourceManagerRepository _resourceManagerRepo;
        private readonly IOptions<UrlOptions> urlOptions;
        private readonly ILogger<HomeController> logger;

        public HomeController(IServicePrincipalRepository servicePrincipalRepo, 
                              IResourceManagerRepository resourceManagerRepo, 
                              IOptions<UrlOptions> urlOptions,
                              ILogger<HomeController> logger)
        {
            _servicePrincipalRepo = servicePrincipalRepo;
            _resourceManagerRepo = resourceManagerRepo;
            this.urlOptions = urlOptions;
            this.logger = logger;
        }

        [Authorize]
        public ActionResult Index()
        {

            ViewData["Message"] = "Your application description page.";

            return View(new WatchVideoModel() { VideoUrl = urlOptions.Value.WatchVideo });
        }

        [Authorize]
        public ActionResult GetStarted()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult Deploy()
        {
            
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Faqs()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
