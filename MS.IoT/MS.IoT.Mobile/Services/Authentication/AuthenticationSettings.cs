using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile.Services.Authentication
{
    public static class AuthenticationSettings
    {
        public static string Tenant = "msiotdevicemanagementdev.onmicrosoft.com";
        public static string ClientID = "33363ec4-875c-4118-95cc-257b48b8a0d2";
        public static string PolicySignUpSignIn = "B2C_1_signin_signup";
        //public static string PolicyEditProfile = "B2C_1_edit_profile";
        //public static string PolicyResetPassword = "B2C_1_reset";

        public static string AuthorityPolicy = PolicySignUpSignIn;

        public static string ReadScope = $"https://msiotdevicemanagementdev.onmicrosoft.com/mobiledev/read";
        public static string WriteScope = $"https://msiotdevicemanagementdev.onmicrosoft.com/mobiledev/write";




        public static List<string> Scopes = new List<string>{ ReadScope,
                                                              WriteScope };

        // { "" };//{ "https://fabrikamb2c.onmicrosoft.com/demoapi/demo.read" };


        public static string ApiEndpointRoot = $"https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/";

        public static string GetRegisteredDevicesEndpoint = $"{ApiEndpointRoot}api/devices/users/";

    }
}
