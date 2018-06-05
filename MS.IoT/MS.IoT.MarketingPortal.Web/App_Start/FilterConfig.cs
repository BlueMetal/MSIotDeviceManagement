using System.Web;
using System.Web.Mvc;

namespace MS.IoT.MarketingPortal.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
