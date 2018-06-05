using MS.IoT.Common;
using MS.IoT.DeviceManagementMobile.Web.Helpers;
using Owin;

namespace MS.IoT.DeviceManagementMobile.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Init(AppConfig.ConfigurationItems.AppInsightsInstrumentationKey);
            ConfigureAuth(app);
        }
    }
}
