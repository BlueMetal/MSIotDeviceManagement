using System.Configuration;

namespace MS.IoT.DeviceManagementMobile.Web.Helpers
{
    public static class AuthConfig
    {
        public static class ConfigurationItems
        {
            public static string ClientId
            {
                get
                {
                    return ConfigurationManager.AppSettings["ida:ClientId"];
                }
            }

            public static string AadInstance
            {
                get
                {
                    return ConfigurationManager.AppSettings["ida:AadInstance"];
                }
            }
            
            public static string Tenant
            {
                get
                {
                    return ConfigurationManager.AppSettings["ida:Tenant"];
                }
            }

            public static string SignUpSignInPolicy
            {
                get
                {
                    return ConfigurationManager.AppSettings["ida:SignUpSignInPolicyId"];
                }
            }           
        }
    }
}