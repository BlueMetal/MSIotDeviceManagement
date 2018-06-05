using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using MS.IoT.MarketingPortal.Web.Controllers;
using Microsoft.Extensions.Logging;
using MS.IoT.Common;
using MS.IoT.MarketingPortal.Web.Helpers;

namespace MS.IoT.MarketingPortal.Web
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
