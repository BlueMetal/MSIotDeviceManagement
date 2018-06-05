using Owin;
using MS.IoT.Common;

namespace MS.IoT.DataPacketDesigner.Web
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