using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using MS.IoT.Common;
using MS.IoT.DeviceManagementPortal.Web.Helpers;

namespace MS.IoT.DeviceManagementPortal.Web
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
