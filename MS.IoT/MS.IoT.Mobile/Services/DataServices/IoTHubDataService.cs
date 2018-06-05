using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using MS.IoT.Mobile.Helpers;
using QXFUtilities.Communication;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Microsoft.Identity.Client;

namespace MS.IoT.Mobile.Services.DataServices
{
    public class IoTHubDataService : HttpCommunicationService
    {

        private static string ServerBaseAddress = $"https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/";
        public static string RegisteredDevicesApiUrl = $"api/devices/users/";
        public static string RegisteredDevicesBaseApiUrl = $"api/devices/";
        public static string ActivateFeatureUrl = $"api/devices/feature";
        public static string FeaturesUrl = $"/features/";
        public static string UrlSeperator = $"/";


        private static IoTHubDataService instance;
        public static IoTHubDataService Instance => instance ?? (instance = new IoTHubDataService());

        private IoTHubDataService(): base()
        {
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            mediaType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
            Client.DefaultRequestHeaders.Accept.Add(mediaType);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AuthenticationService.GetAccessToken("ReadAndWrite"));
        }


        public Task<List<ReturnedDevice>> GetRegisteredDevicesAsync()
        {
            // Refresh the Access Token
            // silent Authentication not working on iPhone - needs investigation
        /*    if (Device.RuntimePlatform != Device.iOS)
            {
                AuthenticationResult authenticationResult = await App.AuthenticationService.AcquireTokenSilentlyAsync(AuthenticationSettings.Scopes);
                App.AuthenticationService.AddAccessToken("ReadAndWrite", authenticationResult.AccessToken);
            }
        */
            return GetAsync<List<ReturnedDevice>>(ServerBaseAddress, RegisteredDevicesApiUrl + Settings.UserId);
        }

        public Task<bool> ActivateFeature(string deviceId, string featureName)
        {
            var parameter = new FeatureActivation
            {
                DeviceId = deviceId,
                Feature = new DtoFeature { Name = featureName, IsActivated = true }
            };
            return PutAsync<bool, FeatureActivation>(ServerBaseAddress, ActivateFeatureUrl, parameter);
        }
        public Task<object> DeactivateFeature(string deviceId, string featureName)
        {
            var parameter = new FeatureActivation
            {
                DeviceId = deviceId,
                Feature = new DtoFeature { Name = featureName, IsActivated = false }
            };
            return PutAsync<object, FeatureActivation>(ServerBaseAddress, ActivateFeatureUrl, parameter);
        }

        public Task<InvokeFeatureResponse> InvokeFeature(string deviceId, string featureName, string featureParameters)
        {
            string url = RegisteredDevicesBaseApiUrl + deviceId + FeaturesUrl + featureName + UrlSeperator + featureParameters;
            return PutAsync<InvokeFeatureResponse>(ServerBaseAddress, url);
        }



    }
}
