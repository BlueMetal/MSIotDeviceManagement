using System.Web;
using System.Web.Optimization;

namespace MS.IoT.DeviceManagementPortal.Web
{
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
                      "~/Scripts/angular-sanitize.js",
                      "~/Scripts/angular-ui-router.js",
                      "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                      "~/Scripts/select.js",
                      "~/Scripts/angular-aside.js",
                      "~/Scripts/ng-sortable.js",
                      "~/Scripts/ng-file-upload.js",
                      "~/Scripts/ng-tags-input.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/App/app.js",
                      "~/App/models/enums.js",
                      "~/App/models/models.js",
                      "~/App/directives/directives.js",
                      "~/App/services/alertService.js",
                      "~/App/services/deviceDBService.js",
                      "~/App/services/groupsService.js",
                      "~/App/services/userService.js",
                      "~/App/controllers/modals/alertModalCtrl.js",
                      "~/App/controllers/modals/confirmModalCtrl.js",
                      "~/App/controllers/modals/importModalCtrl.js",
                      "~/App/controllers/modals/jsonModalCtrl.js",
                      "~/App/controllers/modals/publishFeatureModalCtrl.js",
                      "~/App/controllers/modals/updatePropertiesModalCtrl.js",
                      "~/App/controllers/side/sideMenuCtrl.js",
                      "~/App/controllers/side/sidePanelCtrl.js",
                      "~/App/controllers/side/sidePanelCustomGroupCtrl.js",
                      "~/App/controllers/groupsCtrl.js",
                      "~/App/controllers/listDashboardCtrl.js",
                      "~/App/controllers/mapDashboardCtrl.js",
                      "~/App/controllers/menuCtrl.js",
                      "~/App/controllers/mainCtrl.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/select.css",
                      "~/Content/angular-aside.css",
                      "~/Content/ng-sortable.css",
                      "~/Content/ng-tags-input.css",
                      "~/Content/ng-tags-input.bootstrap.css",
                      "~/Content/Site.css"));
        }        
    }
}
