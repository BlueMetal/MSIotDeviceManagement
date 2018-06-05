using System.Web;
using System.Web.Optimization;

namespace MS.IoT.DataPacketDesigner.Web
{
    /// <summary>
    /// Class to compile the CSS/JS into minimized versions
    /// </summary>
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-ui-router.js",
                      "~/Scripts/angular-ui-tree.js",
                      "~/Scripts/angular-ui/ui-bootstrap-tpls.js"));

            #if DEBUG
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/App/app.js",
                      "~/App/directives/directives.js",
                      "~/App/services/models/models.js",
                      "~/App/services/templateService.js",
                      "~/App/services/mockTemplateService.js",
                      "~/App/services/userService.js",
                      "~/App/services/mockUserService.js",
                      "~/App/filters/filters.js",
                      "~/App/controllers/mainCtrl.js",
                      "~/App/controllers/stepCtrl.js",
                      "~/App/controllers/menuCtrl.js",
                      "~/App/controllers/homeCtrl.js",
                      "~/App/controllers/chooseTemplateCtrl.js",
                      "~/App/controllers/manageTemplateCtrl.js",
                      "~/App/controllers/simulateTemplateCtrl.js",
                      "~/App/controllers/modalCtrl.js"));
#else
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/App/app.js",
                      "~/App/directives/directives.js",
                      "~/App/services/models/models.js",
                      "~/App/services/templateService.js",
                      "~/App/services/userService.js",
                      "~/App/filters/filters.js",
                      "~/App/controllers/mainCtrl.js",
                      "~/App/controllers/stepCtrl.js",
                      "~/App/controllers/menuCtrl.js",
                      "~/App/controllers/homeCtrl.js",
                      "~/App/controllers/chooseTemplateCtrl.js",
                      "~/App/controllers/manageTemplateCtrl.js",
                      "~/App/controllers/simulateTemplateCtrl.js",
                      "~/App/controllers/modalCtrl.js"));
#endif

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/angular-ui-tree.css",
                      "~/Content/site.min.css"));
        }
    }
}
