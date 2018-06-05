using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class used for the routing of the MVC part of the application.
    /// Since the app is a single-page angular application, the MVC routing is deactivated exception for Sign Out.
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Angular Routes
            /*routes.MapRoute(
                name: "AngularDefault",
                url: "Angular/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );*/

            //MVC Routes
            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{param1}/{param2}",
                defaults: new { controller = "Home", action = "Index", param1 = UrlParameter.Optional, param2 = UrlParameter.Optional }
            );*/

            routes.MapRoute(
                name: "SignOut",
                url: "Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            //SPA Routes for Angular
            routes.MapRoute(
                name: "Default",
                url: "{*entrypoint}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
