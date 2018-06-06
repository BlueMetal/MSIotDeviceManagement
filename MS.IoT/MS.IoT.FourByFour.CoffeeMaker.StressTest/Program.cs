using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.FourByFour.CoffeeMaker.StressTest
{
    class Program
    {
        private static readonly HttpClient _Client = new HttpClient();
        private static readonly HttpClient _Client2 = new HttpClient();
        private static Random rand = new Random();
        private static string deviceId = string.Empty;
        private static AuthenticationResult token = null;
        private static string[] apiCallsDirectMethods = {"https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/api/devices/{0}/features/changeBrewStrength/1",
                "https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/api/devices/{0}/features/launchBrew/0",
                "https://msiot-devicemanagement-mobile-api-dev.azurewebsites.net/api/devices/{0}/features/launchGrindAndBrew/0" };
        private static string[] features = { "brewStrengthFeature", "brewFeature", "grindAndBrewFeature", "wifiFeature", "debugFeature" };

        static async Task Main(string[] args)
        {
            Console.WriteLine("Please enter Device ID:");
            deviceId = Console.ReadLine();

            //directmethod
            await CallDirectMethod();

            //desired
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += CallUpdateDesired;
            aTimer.Interval = 3000;
            aTimer.Enabled = true;


            Console.WriteLine("Please enter to stop testing.");
            Console.Read();
        }

        private async static void CallUpdateDesired(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (token == null)
                token = await GetAccessToken();

            string apiCall = "https://msiot-devicemanagementportal-dev.azurewebsites.net/api/devicetwin/properties/feature";

            try
            {
                DeviceTwinDesiredSingleFeatureModel obj = new DeviceTwinDesiredSingleFeatureModel()
                {
                    DeviceId = deviceId,
                    Feature = new DeviceFeature() { Name = features[rand.Next(0, features.Length)], IsActivated = rand.Next(0, 2) == 1 }
                };
                Console.WriteLine(string.Format("CallUpdateDesired: Feature {0}, Change to status: {1}...", obj.Feature.Name, obj.Feature.IsActivated));

                AuthenticationResult auth = await GetAccessToken();
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, apiCall))
                {
                    message.Headers.Add("Authorization", "Bearer " + auth.AccessToken);// = new AuthenticationHeaderValue(auth.AccessTokenType, auth.AccessToken);
                    var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                    message.Content = content;

                    HttpResponseMessage response = await _Client2.SendAsync(message);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2.Message);
            }
        }

        public async static Task CallDirectMethod()
        {
            try
            {
                for (int i = 0; i < 200; i++)
                {
                    string call = string.Format(apiCallsDirectMethods[rand.Next(0, apiCallsDirectMethods.Length)], deviceId);
                    Console.WriteLine("CallDirectMethod: " + call);
                    //AuthenticationResult auth = await GetAccessToken();
                    using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, call))
                    {
                        // Console.WriteLine(auth.AccessToken);
                        //message.Headers.Add("Authorization", "Bearer " + auth.AccessToken);// = new AuthenticationHeaderValue(auth.AccessTokenType, auth.AccessToken);

                        HttpResponseMessage response = await _Client.SendAsync(message);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("CallDirectMethod: " + responseBody);
                    }
                }
                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static readonly string tenantId = "72f43d57-b980-4152-b703-e2d8666a3ea9";
        public static readonly string clientId = "e8b29c1b-5f3f-4fe7-93da-e147ae3ab60d";
        public static readonly string clientSecret = "aK2C3ss/9inQFjw9FjCKm0VPKb73bm2vA5D9OVgsGaM=";
        public static readonly string resource = "https://miteshpatekarbluemetal.onmicrosoft.com/MSIoT.DeviceManagementPortal.Web";
        private async static Task<AuthenticationResult> GetAccessToken()
        {
            string authority = "https://login.microsoftonline.com/" + tenantId;
            var authenticationContext = new AuthenticationContext(authority);
            var credential = new ClientCredential(clientId, clientSecret);
            var result = await authenticationContext.AcquireTokenAsync(resource, credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result;
        }
    }

    public class DeviceTwinDesiredSingleFeatureModel
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "feature")]
        public DeviceFeature Feature { get; set; }
    }

    public class DeviceFeature
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isActivated")]
        public bool IsActivated { get; set; }
    }
}
