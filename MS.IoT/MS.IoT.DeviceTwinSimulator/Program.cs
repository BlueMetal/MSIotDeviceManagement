using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using MS.IoT.DeviceTwinSimulator.Models;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MS.IoT.DeviceTwinSimulator
{
    class Program
    {
        static Random random = new Random();
        public static readonly string iotHubConnectionString = "HostName=iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=XXXXXXXXXXXXXXXX";

        static string[] states = new string[] { "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois",
            "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey",
            "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah",
            "Vermont", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming" };
        static string[] names = new string[] { "Smart {0}", "Super {0} Ultra", "New {0}", "{0} 2", "{0} MX", "{0} TX", "{0} GX" };
        static RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        public static string deviceId = "";

        static async Task Main(string[] args)
        {
            await SwitchChoice();
        }

        public static List<RetailerItem> InitConfig()
        {
            List<RetailerItem> retailers = new List<RetailerItem>() //used https://www.shopify.com to come up with ideas
            {
                new RetailerItem("Best Value Market", 20),
                new RetailerItem("Web Digital Store", 35),
                new RetailerItem("Artspace", 5),
                new RetailerItem("Dial Distributing", 13),
                new RetailerItem("Life Services Supplements", 10),
                new RetailerItem("Shop N Save", 17)
            };

            //Best Value Market - Operates West mainly
            RetailerItem retailer = retailers[0];
            retailer.States.Add(new RetailerState("Washington", 100, 85));
            retailer.States.Add(new RetailerState("Oregon", 100, 70));
            retailer.States.Add(new RetailerState("Nevada", 10, 20));
            retailer.States.Add(new RetailerState("California", 100, 80));
            retailer.States.Add(new RetailerState("Arizona", 5, 70));
            retailer.States.Add(new RetailerState("Utah", 15, 60));
            retailer.States.Add(new RetailerState("Wyoming", 5, 40));
            retailer.States.Add(new RetailerState("Montana", 20, 50));
            retailer.States.Add(new RetailerState("Idaho", 30, 70));
            retailer.States.Add(new RetailerState("New Mexico", 40, 35));
            retailer.States.Add(new RetailerState("Colorado", 30, 66));
            retailer.Families.Add(new ProductFamilyItem("Kitchen", 3) { ProductNames = new List<Item>() { new Item("Coffee Maker", 30), new Item("Stove", 20), new Item("Oven", 15), new Item("Brewing Machine", 5), new Item("Toaster", 21) } });
            retailer.Families.Add(new ProductFamilyItem("Laundry", 2) { ProductNames = new List<Item>() { new Item("Dryer", 30), new Item("Washing Machine", 30), new Item("Washer-Dryer", 40) } });
            retailer.Families.Add(new ProductFamilyItem("Lights", 1) { ProductNames = new List<Item>() { new Item("RGB", 10), new Item("Ultraviolet", 15), new Item("Spotlight", 16), new Item("Disco Ball", 7), new Item("LED", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 3) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 1) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 20), new Item("Shower", 30), new Item("Jacuzzi", 5), new Item("Swimming Pool", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Refrigeration", 3) { ProductNames = new List<Item>() { new Item("Fridge", 35), new Item("Freezer", 30), new Item("Watercooler", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Security", 1) { ProductNames = new List<Item>() { new Item("Secure Doors", 30), new Item("Safe", 20), new Item("Camera", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 1) { ProductNames = new List<Item>() { new Item("Trash Can", 20), new Item("Shredder", 40) } });
            retailer.InitValues();

            //Web Digital Store, operates everywhere
            retailer = retailers[1];
            retailer.States.Add(new RetailerState("Alabama", 19, 84));
            retailer.States.Add(new RetailerState("Alaska", 3, 61));
            retailer.States.Add(new RetailerState("Arizona", 12, 66));
            retailer.States.Add(new RetailerState("Arkansas", 2, 94));
            retailer.States.Add(new RetailerState("California", 13, 90));
            retailer.States.Add(new RetailerState("Colorado", 6, 66));
            retailer.States.Add(new RetailerState("Connecticut", 12, 88));
            retailer.States.Add(new RetailerState("Delaware", 17, 65));
            retailer.States.Add(new RetailerState("Florida", 5, 67));
            retailer.States.Add(new RetailerState("Georgia", 5, 61));
            retailer.States.Add(new RetailerState("Hawaii", 4, 73));
            retailer.States.Add(new RetailerState("Idaho", 10, 47));
            retailer.States.Add(new RetailerState("Illinois", 11, 46));
            retailer.States.Add(new RetailerState("Indiana", 5, 90));
            retailer.States.Add(new RetailerState("Iowa", 4, 57));
            retailer.States.Add(new RetailerState("Kansas", 3, 75));
            retailer.States.Add(new RetailerState("Kentucky", 6, 41));
            retailer.States.Add(new RetailerState("Louisiana", 2, 78));
            retailer.States.Add(new RetailerState("Maine", 4, 93));
            retailer.States.Add(new RetailerState("Maryland", 18, 79));
            retailer.States.Add(new RetailerState("Massachusetts", 3, 32));
            retailer.States.Add(new RetailerState("Michigan", 7, 61));
            retailer.States.Add(new RetailerState("Minnesota", 8, 91));
            retailer.States.Add(new RetailerState("Mississippi", 7, 43));
            retailer.States.Add(new RetailerState("Missouri", 2, 67));
            retailer.States.Add(new RetailerState("Montana", 1, 20));
            retailer.States.Add(new RetailerState("Nebraska", 9, 69));
            retailer.States.Add(new RetailerState("Nevada", 8, 41));
            retailer.States.Add(new RetailerState("New Hampshire", 3, 58));
            retailer.States.Add(new RetailerState("New Jersey", 17, 51));
            retailer.States.Add(new RetailerState("New Mexico", 18, 44));
            retailer.States.Add(new RetailerState("New York", 16, 53));
            retailer.States.Add(new RetailerState("North Carolina", 6, 59));
            retailer.States.Add(new RetailerState("North Dakota", 14, 71));
            retailer.States.Add(new RetailerState("Ohio", 9, 67));
            retailer.States.Add(new RetailerState("Oklahoma", 1, 56));
            retailer.States.Add(new RetailerState("Oregon", 8, 82));
            retailer.States.Add(new RetailerState("Pennsylvania", 15, 81));
            retailer.States.Add(new RetailerState("Rhode Island", 3, 69));
            retailer.States.Add(new RetailerState("South Carolina", 7, 70));
            retailer.States.Add(new RetailerState("South Dakota", 9, 77));
            retailer.States.Add(new RetailerState("Tennessee", 18, 78));
            retailer.States.Add(new RetailerState("Texas", 4, 67));
            retailer.States.Add(new RetailerState("Utah", 18, 86));
            retailer.States.Add(new RetailerState("Vermont", 2, 33));
            retailer.States.Add(new RetailerState("Virginia", 10, 53));
            retailer.States.Add(new RetailerState("Washington", 11, 69));
            retailer.States.Add(new RetailerState("West Virginia", 5, 74));
            retailer.States.Add(new RetailerState("Wisconsin", 15, 75));
            retailer.States.Add(new RetailerState("Wyoming", 6, 70));
            retailer.Families.Add(new ProductFamilyItem("Kitchen", 3) { ProductNames = new List<Item>() { new Item("Coffee Maker", 20), new Item("Stove", 30), new Item("Oven", 15), new Item("Brewing Machine", 5), new Item("Toaster", 21) } });
            retailer.Families.Add(new ProductFamilyItem("Laundry", 1) { ProductNames = new List<Item>() { new Item("Dryer", 50), new Item("Washing Machine", 30), new Item("Washer-Dryer", 20) } });
            retailer.Families.Add(new ProductFamilyItem("Lights", 1) { ProductNames = new List<Item>() { new Item("RGB", 10), new Item("Ultraviolet", 35), new Item("Spotlight", 16), new Item("Disco Ball", 17), new Item("LED", 20) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 3) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 1) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 30), new Item("Shower", 10), new Item("Jacuzzi", 5), new Item("Swimming Pool", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Refrigeration", 2) { ProductNames = new List<Item>() { new Item("Fridge", 35), new Item("Freezer", 30), new Item("Watercooler", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Security", 5) { ProductNames = new List<Item>() { new Item("Secure Doors", 20), new Item("Safe", 20), new Item("Camera", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 1) { ProductNames = new List<Item>() { new Item("Trash Can", 20), new Item("Shredder", 40) } });
            retailer.InitValues();

            //Artspace - operates north east coast
            retailer = retailers[2];
            retailer.States.Add(new RetailerState("Maine", 20, 75));
            retailer.States.Add(new RetailerState("Vermont", 10, 50));
            retailer.States.Add(new RetailerState("New Hampshire", 20, 70));
            retailer.States.Add(new RetailerState("New York", 100, 85));
            retailer.States.Add(new RetailerState("Massachusetts", 40, 53));
            retailer.States.Add(new RetailerState("Connecticut", 60, 50));
            retailer.States.Add(new RetailerState("Rhode Island", 50, 60));
            retailer.States.Add(new RetailerState("New Jersey", 55, 90));
            retailer.States.Add(new RetailerState("Pennsylvania", 15, 45));
            retailer.States.Add(new RetailerState("Delaware", 5, 70));
            retailer.States.Add(new RetailerState("Maryland", 30, 45));
            retailer.Families.Add(new ProductFamilyItem("Kitchen", 2) { ProductNames = new List<Item>() { new Item("Coffee Maker", 30), new Item("Stove", 20), new Item("Oven", 15), new Item("Brewing Machine", 5), new Item("Toaster", 21) } });
            retailer.Families.Add(new ProductFamilyItem("Laundry", 1) { ProductNames = new List<Item>() { new Item("Dryer", 30), new Item("Washing Machine", 30), new Item("Washer-Dryer", 40) } });
            retailer.Families.Add(new ProductFamilyItem("Lights", 3) { ProductNames = new List<Item>() { new Item("Ultraviolet", 15), new Item("Spotlight", 16), new Item("Disco Ball", 7), new Item("LED", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 2) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 1) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 20), new Item("Shower", 30), new Item("Jacuzzi", 5), new Item("Swimming Pool", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 3) { ProductNames = new List<Item>() { new Item("Trash Can", 20), new Item("Shredder", 40) } });
            retailer.InitValues();

            //Dial Distributing - operates everywhere, but with low activation chance
            retailer = retailers[3];
            retailer.States.Add(new RetailerState("Alabama", 1, 56));
            retailer.States.Add(new RetailerState("Alaska", 4, 48));
            retailer.States.Add(new RetailerState("Arizona", 13, 66));
            retailer.States.Add(new RetailerState("Arkansas", 7, 43));
            retailer.States.Add(new RetailerState("California", 9, 53));
            retailer.States.Add(new RetailerState("Colorado", 7, 43));
            retailer.States.Add(new RetailerState("Connecticut", 7, 68));
            retailer.States.Add(new RetailerState("Delaware", 4, 53));
            retailer.States.Add(new RetailerState("Florida", 9, 79));
            retailer.States.Add(new RetailerState("Georgia", 17, 45));
            retailer.States.Add(new RetailerState("Hawaii", 4, 15));
            retailer.States.Add(new RetailerState("Idaho", 11, 47));
            retailer.States.Add(new RetailerState("Illinois", 14, 30));
            retailer.States.Add(new RetailerState("Indiana", 13, 49));
            retailer.States.Add(new RetailerState("Iowa", 3, 35));
            retailer.States.Add(new RetailerState("Kansas", 1, 48));
            retailer.States.Add(new RetailerState("Kentucky", 14, 37));
            retailer.States.Add(new RetailerState("Louisiana", 6, 45));
            retailer.States.Add(new RetailerState("Maine", 5, 41));
            retailer.States.Add(new RetailerState("Maryland", 9, 48));
            retailer.States.Add(new RetailerState("Massachusetts", 6, 63));
            retailer.States.Add(new RetailerState("Michigan", 15, 29));
            retailer.States.Add(new RetailerState("Minnesota", 17, 45));
            retailer.States.Add(new RetailerState("Mississippi", 7, 77));
            retailer.States.Add(new RetailerState("Missouri", 7, 45));
            retailer.States.Add(new RetailerState("Montana", 6, 50));
            retailer.States.Add(new RetailerState("Nebraska", 15, 57));
            retailer.States.Add(new RetailerState("Nevada", 8, 18));
            retailer.States.Add(new RetailerState("New Hampshire", 2, 68));
            retailer.States.Add(new RetailerState("New Jersey", 19, 80));
            retailer.States.Add(new RetailerState("New Mexico", 9, 55));
            retailer.States.Add(new RetailerState("New York", 17, 25));
            retailer.States.Add(new RetailerState("North Carolina", 9, 75));
            retailer.States.Add(new RetailerState("North Dakota", 5, 63));
            retailer.States.Add(new RetailerState("Ohio", 16, 40));
            retailer.States.Add(new RetailerState("Oklahoma", 9, 49));
            retailer.States.Add(new RetailerState("Oregon", 10, 57));
            retailer.States.Add(new RetailerState("Pennsylvania", 5, 59));
            retailer.States.Add(new RetailerState("Rhode Island", 7, 54));
            retailer.States.Add(new RetailerState("South Carolina", 4, 78));
            retailer.States.Add(new RetailerState("South Dakota", 5, 74));
            retailer.States.Add(new RetailerState("Tennessee", 10, 73));
            retailer.States.Add(new RetailerState("Texas", 3, 42));
            retailer.States.Add(new RetailerState("Utah", 4, 40));
            retailer.States.Add(new RetailerState("Vermont", 15, 69));
            retailer.States.Add(new RetailerState("Virginia", 13, 49));
            retailer.States.Add(new RetailerState("Washington", 11, 53));
            retailer.States.Add(new RetailerState("West Virginia", 19, 76));
            retailer.States.Add(new RetailerState("Wisconsin", 16, 63));
            retailer.States.Add(new RetailerState("Wyoming", 18, 49));
            retailer.Families.Add(new ProductFamilyItem("Laundry", 1) { ProductNames = new List<Item>() { new Item("Dryer", 30), new Item("Washing Machine", 30), new Item("Washer-Dryer", 40) } });
            retailer.Families.Add(new ProductFamilyItem("Lights", 5) { ProductNames = new List<Item>() { new Item("RGB", 10), new Item("Ultraviolet", 15), new Item("Spotlight", 16), new Item("Disco Ball", 7), new Item("LED", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 2) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 1) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 20), new Item("Shower", 30), new Item("Jacuzzi", 5), new Item("Swimming Pool", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Refrigeration", 2) { ProductNames = new List<Item>() { new Item("Fridge", 35), new Item("Freezer", 30), new Item("Watercooler", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Security", 3) { ProductNames = new List<Item>() { new Item("Secure Doors", 30), new Item("Safe", 20), new Item("Camera", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 1) { ProductNames = new List<Item>() { new Item("Trash Can", 20), new Item("Shredder", 40) } });
            retailer.InitValues();

            //Life Services Supplements - south east all the way to texas
            retailer = retailers[4];
            retailer.States.Add(new RetailerState("Texas", 50, 40));
            retailer.States.Add(new RetailerState("Louisiana", 10, 60));
            retailer.States.Add(new RetailerState("Oklahoma", 5, 30));
            retailer.States.Add(new RetailerState("Arkansas", 7, 70));
            retailer.States.Add(new RetailerState("Mississippi", 20, 50));
            retailer.States.Add(new RetailerState("Alabama", 30, 50));
            retailer.States.Add(new RetailerState("Georgia", 35, 55));
            retailer.States.Add(new RetailerState("Florida", 80, 75));
            retailer.States.Add(new RetailerState("South Carolina", 40, 75));
            retailer.States.Add(new RetailerState("North Carolina", 50, 65));
            retailer.Families.Add(new ProductFamilyItem("Kitchen", 2) { ProductNames = new List<Item>() { new Item("Coffee Maker", 30), new Item("Stove", 20), new Item("Oven", 15), new Item("Brewing Machine", 5), new Item("Toaster", 21) } });
            retailer.Families.Add(new ProductFamilyItem("Lights", 1) { ProductNames = new List<Item>() { new Item("RGB", 10), new Item("Ultraviolet", 15), new Item("Spotlight", 16), new Item("Disco Ball", 7), new Item("LED", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 2) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 5) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 20), new Item("Shower", 30), new Item("Jacuzzi", 5), new Item("Swimming Pool", 10) } });
            retailer.Families.Add(new ProductFamilyItem("Refrigeration", 3) { ProductNames = new List<Item>() { new Item("Fridge", 35), new Item("Freezer", 30), new Item("Watercooler", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Security", 1) { ProductNames = new List<Item>() { new Item("Secure Doors", 30), new Item("Safe", 20), new Item("Camera", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 1) { ProductNames = new List<Item>() { new Item("Trash Can", 20), new Item("Shredder", 40) } });
            retailer.InitValues();

            //Shop N Save - Midwest
            retailer = retailers[5];
            retailer.States.Add(new RetailerState("Illinois", 80, 89));
            retailer.States.Add(new RetailerState("Michigan", 40, 60));
            retailer.States.Add(new RetailerState("Ohio", 40, 70));
            retailer.States.Add(new RetailerState("Indiana", 50, 75));
            retailer.States.Add(new RetailerState("Iowa", 20, 50));
            retailer.States.Add(new RetailerState("Wisconsin", 30, 45));
            retailer.States.Add(new RetailerState("Missouri", 35, 55));
            retailer.States.Add(new RetailerState("Kentucky", 30, 75));
            retailer.Families.Add(new ProductFamilyItem("Kitchen", 2) { ProductNames = new List<Item>() { new Item("Coffee Maker", 30), new Item("Stove", 20), new Item("Oven", 15), new Item("Brewing Machine", 5), new Item("Toaster", 21) } });
            retailer.Families.Add(new ProductFamilyItem("Laundry", 1) { ProductNames = new List<Item>() { new Item("Dryer", 30), new Item("Washing Machine", 30), new Item("Washer-Dryer", 40) } });
            retailer.Families.Add(new ProductFamilyItem("Heating", 2) { ProductNames = new List<Item>() { new Item("AC", 30), new Item("Heating", 30), new Item("Fan", 20), new Item("Mattress", 15), new Item("Fireplace", 15) } });
            retailer.Families.Add(new ProductFamilyItem("Water", 1) { ProductNames = new List<Item>() { new Item("Water fountain", 35), new Item("Water Pump", 20), new Item("Shower", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Refrigeration", 2) { ProductNames = new List<Item>() { new Item("Fridge", 35), new Item("Freezer", 30), new Item("Watercooler", 30) } });
            retailer.Families.Add(new ProductFamilyItem("Security", 1) { ProductNames = new List<Item>() { new Item("Secure Doors", 30), new Item("Safe", 20), new Item("Camera", 50) } });
            retailer.Families.Add(new ProductFamilyItem("Recycling", 1) { ProductNames = new List<Item>() { new Item("Trash Can", 60), new Item("Shredder", 20) } });
            retailer.InitValues();

            return retailers;
        }

        public static async Task GenerateRandomDevices(int nbrDevices)
        {
            List<RetailerItem> retailersDB = InitConfig();
            List<RetailerItem> retailers = new List<RetailerItem>();
            foreach (var retailerDB in retailersDB)
            {
                for (var i = 0; i < retailerDB.Retailer.Percentage; i++)
                    retailers.Add(retailerDB);
            }

            for (int i = 0; i < nbrDevices; i++)
            {
                var deviceIdN = Guid.NewGuid().ToString();
                try
                {
                    AddDeviceAsync(deviceIdN).GetAwaiter().GetResult();

                    RetailerItem retailer = retailers[random.Next(0, retailers.Count)];
                    RetailerState state = retailer.StateChances[random.Next(0, retailer.StateChances.Count)];
                    ProductFamilyItem retailerFamily = retailer.FamiliesChances[random.Next(0, retailer.FamiliesChances.Count)];
                    string productName = retailerFamily.ProductNamesChances[random.Next(0, retailerFamily.ProductNamesChances.Count)];
                    DateTime shippedDate = retailer.ShipmentDates[random.Next(0, retailer.ShipmentDates.Count)];
                    DateTime manufacturedDate = shippedDate.AddMonths(-1);

                    var tagsN = new DeviceTwinTagsModel()
                    {
                        ProductFamily = retailerFamily.ProductFamily.Name,
                        ManufacturedDate = manufacturedDate,
                        ProductName = GetRandomModel(productName),
                        RetailerName = retailer.Retailer.Name,
                        RetailerRegion = state.State.Name,
                        ShippedDate = shippedDate,
                        UserId = Guid.NewGuid().ToString(),
                        DemoTag = true
                    };

                    UpdateTagsProperties(deviceIdN, tagsN).GetAwaiter().GetResult();

                    UpdateDesiredProperties(deviceIdN).GetAwaiter().GetResult();

                    bool activated = random.Next(0, 100) <= state.ActivationChance ? true : false;
                    DateTime activationDate = DateTime.MinValue;
                    if (activated)
                    {
                        do
                        {
                            activationDate = new DateTime(2017, random.Next(1, 12), random.Next(1, 28));
                        } while (DateTime.UtcNow > activationDate);
                    }

                    var reportedN = new DeviceTwinReportedModel()
                    {
                        StatusCode = 0,
                        FirmwareVersion = "1.2",
                        Heartbeat = activated ? (DateTime?)activationDate : null,
                        ActivationDate = activated ? (DateTime?)(activationDate) : null,
                        IpAddress = activated ? await GetRandomUSIP(state.State.Name) : null,
                        Features = new Dictionary<string, DeviceTwinReportedFeaturesModel>()
                            {
                                {"feature1", new DeviceTwinReportedFeaturesModel
                                { Name="feature1",DisplayName="Start Method 1",
                                    MethodName ="launchMethod1",IsActivated=random.Next(0,1) == 1 ? true : false} },

                                {"feature2", new DeviceTwinReportedFeaturesModel
                                { Name="feature2",DisplayName="Start Method 2",
                                    MethodName ="launchMethod2",IsActivated=random.Next(0,1) == 1 ? true : false} },

                                {"feature3", new DeviceTwinReportedFeaturesModel
                                { Name="feature3",DisplayName="Start Method 3",MethodName="launchMethod3",
                                    IsActivated = random.Next(0,1) == 1 ? true : false} }
                            }
                    };
                    UpdateReportedProperties(deviceIdN, reportedN).GetAwaiter().GetResult();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nDevice created and initialized number - {0} deviceId - {1}", i, deviceIdN);
                    Console.ResetColor();
                } catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n Error initializing device number - {0} deviceId - {1} - error", i, deviceIdN, e.Message);
                    Console.ResetColor();
                }
            }
        }

        private static async Task SwitchChoice()
        {
            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("\n\nPlease Enter your choice:");
                Console.WriteLine("1. Create Device or use an existing device");
                Console.WriteLine("2. Add Tags to Device");
                Console.WriteLine("3. Add Desired Properties to Device");
                Console.WriteLine("4. Add Reported Properties to Device");
                Console.WriteLine("5. Add and initialize multiple devices");
                Console.WriteLine("6. Delete a Device");
                Console.WriteLine("7. Initialize Demo Devices");
                Console.WriteLine("8. Delete Demo Devices");
                Console.WriteLine("9. Run Simulation");
                Console.WriteLine("0. Exit");
                key = Console.ReadKey();

                switch (key.Key)
                {
                    #region Manual settings
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        // create device Id
                        Console.WriteLine("\nEnter Device Id :");
                        deviceId = Console.ReadLine();
                        var devicekey = AddDeviceAsync(deviceId).GetAwaiter().GetResult();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDevice Created -Device Id-{0} , key-{1}", deviceId, devicekey);
                        Console.ResetColor();

                        break;

                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        if (deviceId.Equals(""))
                        {
                            Console.WriteLine("\nPlease select option 1 first");
                            break;
                        }

                        Console.WriteLine("\nEnter Product Family :");
                        var productFamily = Console.ReadLine();
                        Console.WriteLine("\nEnter Product Name :");
                        var productName = Console.ReadLine();
                        Console.WriteLine("\nEnter Retailer :");
                        var retailerName = Console.ReadLine();

                        var tags = new DeviceTwinTagsModel()
                        {
                            ProductFamily = productFamily ?? "CoffeeMaker",
                            ManufacturedDate = DateTime.UtcNow,
                            ProductName = productName ?? "Smart Coffee Maker",
                            RetailerName = retailerName ?? "Amazon",
                            ShippedDate = DateTime.UtcNow,

                        };
                        UpdateTagsProperties(deviceId, tags).GetAwaiter().GetResult();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDevice tags Updated");
                        Console.ResetColor();

                        break;

                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        if (deviceId.Equals(""))
                        {
                            Console.WriteLine("\nPlease select option 1 first");
                            break;
                        }

                        UpdateDesiredProperties(deviceId).GetAwaiter().GetResult();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDevice Desired Properties Updated");
                        Console.ResetColor();

                        break;

                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        if (deviceId.Equals(""))
                        {
                            Console.WriteLine("\nPlease select option 1 first");
                            break;
                        }

                        Console.WriteLine("\nEnter the IP address :");
                        var ipAddress = Console.ReadLine();

                        //var dateCondition = DateTime.Now;
                        //var dateString = dateCondition.ToString("yyyy-MM-ddThh:mm:ss:ffZ");

                        var reported = new DeviceTwinReportedModel()
                        {
                            StatusCode = 0,
                            FirmwareVersion = "1.2",
                            Heartbeat = DateTime.UtcNow,
                            IpAddress = ipAddress ?? "208.59.148.106",
                            Features = new Dictionary<string, DeviceTwinReportedFeaturesModel>()
                            {
                                {"brewStrengthFeature", new DeviceTwinReportedFeaturesModel
                                { DisplayName="Brew Strength",MethodName ="changeBrewStrength",
                                    IsActivated =true,InternalUseOnly=false} },

                                {"brewFeature", new DeviceTwinReportedFeaturesModel
                                { DisplayName="Brew",MethodName ="changeBrewStrength,launchBrew",
                                    IsActivated =true,InternalUseOnly=false} },

                                {"grindAndBrewFeature", new DeviceTwinReportedFeaturesModel
                                { DisplayName="Grind and Brew",MethodName ="changeBrewStrength,launchBrew,launchGrindAndBrew",
                                    IsActivated =false,InternalUseOnly=false} },

                                {"wifiFeature", new DeviceTwinReportedFeaturesModel
                                { DisplayName="Wifi Button",MethodName ="setWifi",
                                    IsActivated =false,InternalUseOnly=true} },

                                {"debugFeature", new DeviceTwinReportedFeaturesModel
                                { DisplayName="Debug",MethodName ="",
                                    IsActivated =true,InternalUseOnly=true} },
                            }
                        };

                        UpdateReportedProperties(deviceId, reported).GetAwaiter().GetResult();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDevice Reported Properties Updated");
                        Console.ResetColor();

                        break;

                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:

                        Console.WriteLine("\nEnter the number of devices to initialize:");
                        var numberOfDevices = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("\nEnter the Product name:");
                        var productNameN = Console.ReadLine();

                        Console.WriteLine("\nEnter the Retailer name:");
                        var retailerNameN = Console.ReadLine();

                        Console.WriteLine("\nEnter the Ip address:");
                        var ipN = Console.ReadLine();

                        for (int i = 0; i < numberOfDevices; i++)
                        {
                            var deviceIdN = Guid.NewGuid().ToString();
                            AddDeviceAsync(deviceIdN).GetAwaiter().GetResult();

                            var tagsN = new DeviceTwinTagsModel()
                            {
                                ProductFamily = productNameN + i,
                                ManufacturedDate = DateTime.UtcNow,
                                ProductName = "Smart " + productNameN + i,
                                RetailerName = retailerNameN,
                                ShippedDate = DateTime.UtcNow,
                            };
                            UpdateTagsProperties(deviceIdN, tagsN).GetAwaiter().GetResult();

                            UpdateDesiredProperties(deviceIdN).GetAwaiter().GetResult();

                            var reportedN = new DeviceTwinReportedModel()
                            {
                                StatusCode = 0,
                                FirmwareVersion = "1.2",
                                Heartbeat = DateTime.Now,
                                IpAddress = ipN,
                                Features = new Dictionary<string, DeviceTwinReportedFeaturesModel>()
                            {
                                {"brewStrength", new DeviceTwinReportedFeaturesModel
                                { Name="brewStrength",DisplayName="Change Brew Strength",
                                    MethodName ="changeBrewStrength",IsActivated=true} },

                                {"brew", new DeviceTwinReportedFeaturesModel
                                { Name="brew",DisplayName="Start Breweing",
                                    MethodName ="changeBrewStrength,launchBrew",IsActivated=false} },

                                {"grindAndBrew", new DeviceTwinReportedFeaturesModel
                                { Name="grindAndBrew",DisplayName="Start Grinding and Brewing",MethodName="changeBrewStrength,launchBrew,launchGrindAndBrew",
                                    IsActivated =true} }
                            }
                            };
                            UpdateReportedProperties(deviceIdN, reportedN).GetAwaiter().GetResult();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nDevice created and initailaized number - {0} deviceId - {1}", i, deviceIdN);
                            Console.ResetColor();
                        }

                        break;

                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        // create device Id
                        Console.WriteLine("\nEnter Device Id to delete :");
                        var deviceIdDelete = Console.ReadLine();
                        await DeleteDeviceAsync(deviceIdDelete);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nDevice Deleted -Device Id-{0} ", deviceIdDelete);
                        Console.ResetColor();

                        break;
                    #endregion
                    #region Demo related features
                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:

                        Console.WriteLine("\nEnter the number of devices to initialize:");
                        var numberOfDevicesDemo = Convert.ToInt32(Console.ReadLine());

                        await GenerateRandomDevices(numberOfDevicesDemo);

                        break;

                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Console.WriteLine("\nPress Enter to confirm. This will delete all demo devices.");
                        ConsoleKeyInfo keyDeletePress = Console.ReadKey();
                        if (keyDeletePress.Key == ConsoleKey.Enter)
                        {
                            List<DeviceTwinFlatModel> devices = await GetAllDemoItems();
                            List<DeviceTwinFlatModel> devicesProcessing = new List<DeviceTwinFlatModel>();
                            foreach (var device in devices)
                            {
                                devicesProcessing.Add(device);
                                if (devicesProcessing.Count >= 100)
                                {
                                    await DeleteMultipleDevicesAsync(devicesProcessing);
                                    devicesProcessing.Clear();
                                    Console.WriteLine("\nDeleting 100 devices. Pausing 10 seconds to avoid throttling");
                                    Thread.Sleep(10000);
                                    
                                }
                            }
                            if (devicesProcessing.Count >= 0)
                            {
                                await DeleteMultipleDevicesAsync(devicesProcessing);
                                devicesProcessing.Clear();
                            }
                            Console.WriteLine("\nDemo devices deleted");
                        }
                        break;

                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        List<DeviceTwinFlatModel> devicesRun = await GetAllActivatedDemoItems();

                        Timer t = new Timer(RunSimulation, devicesRun, 0, 5000);

                        Console.WriteLine("\nPress Enter to end the simulation");
                        ConsoleKeyInfo keyPress;
                        do {
                            keyPress = Console.ReadKey();
                        }
                        while (keyPress.Key != ConsoleKey.Enter);

                        t.Dispose();
                        
                        break;
                        #endregion
                }
            }
            while (!key.Key.Equals(ConsoleKey.D0));
        }

        private async static Task<List<DeviceTwinFlatModel>> GetAllDemoItems()
        {
            string queryString = @"SELECT deviceId FROM devices where tags.demo = true";
            List<DeviceTwinFlatModel> devices = new List<DeviceTwinFlatModel>();
            IQuery query = registryManager.CreateQuery(queryString);
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsJsonAsync();
                foreach (var twin in page)
                    devices.Add(JsonConvert.DeserializeObject<DeviceTwinFlatModel>(twin));
            }
            return devices;
        }

        private async static Task<List<DeviceTwinFlatModel>> GetAllActivatedDemoItems()
        {
            string queryString = @"SELECT deviceId FROM devices where tags.demo = true AND is_defined(properties.reported.activationDate)";
            List<DeviceTwinFlatModel> devices = new List<DeviceTwinFlatModel>();
            IQuery query = registryManager.CreateQuery(queryString);
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsJsonAsync();
                foreach (var twin in page)
                    devices.Add(JsonConvert.DeserializeObject<DeviceTwinFlatModel>(twin));
            }
            return devices;
        }

        private async static Task<List<DeviceTwinFlatModel>> GetAllActivatedDemoItemsWithStatusCode()
        {
            string queryString = @"SELECT deviceId FROM devices where tags.demo = true AND is_defined(properties.reported.activationDate) AND properties.reported.statusCode != 0";
            List<DeviceTwinFlatModel> devices = new List<DeviceTwinFlatModel>();
            IQuery query = registryManager.CreateQuery(queryString);
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsJsonAsync();
                foreach (var twin in page)
                    devices.Add(JsonConvert.DeserializeObject<DeviceTwinFlatModel>(twin));
            }
            return devices;
        }

        static bool isSimulating = false;
        static int[] statusCodeChances = {400,400,400,401,401,402,403,403,403,403,403,403,404,404,404,404,404,404,404,405,405,406,407,408,408,408,408,409,409,409,409,409,409};
        private async static void RunSimulation(object o)
        {
            List<DeviceTwinFlatModel> devices = o as List<DeviceTwinFlatModel>;
            if (isSimulating || devices == null)
                return;

            isSimulating = true;

            //Randomly pick a device
            DeviceTwinFlatModel device = null;
            DeviceTwinModel deviceTwin = null;
            int probability = random.Next(0, 1000);

            if (probability <= 600)
            {
                //Update heartbeat
                device = devices[random.Next(0, devices.Count)];
                deviceTwin = GetDeviceTwinAsync(device.DeviceId).Result;
                await UpdateReportedHeartbeat(device.DeviceId);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Heartbeat added for deviceId- {0}", device.DeviceId);
                Console.ResetColor();
            }
            else
            {
                List<DeviceTwinFlatModel> devicesStatus = await GetAllActivatedDemoItemsWithStatusCode();
                probability += devicesStatus.Count;
                if (probability > 600 && probability <= 950)
                {
                    //Add alert code
                    device = devices[random.Next(0, devices.Count)];
                    deviceTwin = GetDeviceTwinAsync(device.DeviceId).Result;
                    await UpdateReportedStatusCode(device.DeviceId, statusCodeChances[random.Next(0, statusCodeChances.Length)]);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("StatusCode added for deviceId- {0}", device.DeviceId);
                    Console.ResetColor();
                }
                else if (probability > 950 && devicesStatus.Count > 0)
                {
                    //Reset error code
                    device = devicesStatus[random.Next(0, devicesStatus.Count)];
                    deviceTwin = GetDeviceTwinAsync(device.DeviceId).Result;
                    await UpdateReportedStatusCode(device.DeviceId, 0);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("StatusCode removed for deviceId- {0}", device.DeviceId);
                    Console.ResetColor();
                }
            }

            isSimulating = false;
        }

        public static async Task DeleteMultipleDevicesAsync(List<DeviceTwinFlatModel> deviceIds)
        {
            try
            {
                List<Microsoft.Azure.Devices.Device> devices = new List<Microsoft.Azure.Devices.Device>();
                foreach (var deviceId in deviceIds)
                {
                    Microsoft.Azure.Devices.Device device = new Device(deviceId.DeviceId);
                    //Device device2 = await registryManager.GetDevicesAsync().;
                    if (device != null)
                    {
                        devices.Add(device);
                    }
                }

                if (devices.Count > 0)
                {
                    var result = await registryManager.RemoveDevices2Async(devices, true, new System.Threading.CancellationToken());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device = await registryManager.GetDeviceAsync(deviceId);
            if (device == null)
            {
                try
                {
                    device = await registryManager.AddDeviceAsync(new Device(deviceId));
                }
                catch (DeviceAlreadyExistsException)
                {
                    device = await registryManager.GetDeviceAsync(deviceId);
                }
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public static async Task DeleteDeviceAsync(string deviceId)
        {
            Device device = await registryManager.GetDeviceAsync(deviceId);
            if (device != null)
            {
                try
                {
                    await registryManager.RemoveDeviceAsync(device);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Device Delete exception" + e.Message);
                }
            }
        }

        public static async Task DeleteMultipleDevicesAsync(int number)
        {
            var devices = await registryManager.GetDevicesAsync(number);
            await registryManager.RemoveDevices2Async(devices);
        }

        public static async Task UpdateTagsProperties(string deviceId, DeviceTwinTagsModel tags)
        {
            var twin = await registryManager.GetTwinAsync(deviceId);

            dynamic patch = new ExpandoObject();

            patch = new
            {
                tags = tags
            };

            await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
        }

        public static async Task UpdateDesiredProperties(string deviceId)
        {
            var twin = await registryManager.GetTwinAsync(deviceId);

            dynamic patch = new ExpandoObject();

            patch.properties = new
            {
                desired = new DeviceTwinDesiredModel()
                {
                    Features = new Dictionary<string, bool>()
                            {
                                {"feature1", true },

                                {"feature2", false},

                                {"feature3", true}
                            }
                }
            };

            await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
        }

        public static async Task EmptyFeatures(string deviceId)
        {
            var twin = await registryManager.GetTwinAsync(deviceId);

            dynamic patch = new ExpandoObject();

            patch.properties = new
            {
                desired = new
                {
                    Features = new Dictionary<string, object>()
                    {
                        {"feature1", null },
                        {"feature2", null },
                        {"feature3", null }
                    }
                }
            };

            await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);

            dynamic patch2 = new ExpandoObject();

            patch2.properties = new
            {
                reported = new Dictionary<string, object>()
                {
                { "featuresDefinitions", null }
                }

            };

            var jsonString = JsonConvert.SerializeObject(patch2.properties.reported);

            var patch1 = JsonConvert.DeserializeObject<TwinCollection>(jsonString);
            var deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, deviceId);
            await deviceClient.UpdateReportedPropertiesAsync(patch1);
        }

        public static async Task UpdateReportedProperties(string deviceId, DeviceTwinReportedModel reported)
        {
            dynamic patch = new ExpandoObject();

            patch.properties = new
            {
                reported = new DeviceTwinReportedModel()
                {
                    StatusCode = reported.StatusCode,
                    FirmwareVersion = reported.FirmwareVersion,
                    Heartbeat = reported.Heartbeat,
                    ActivationDate = reported.ActivationDate,
                    IpAddress = reported.IpAddress,
                    Features = reported.Features
                }
            };

            var jsonString = JsonConvert.SerializeObject(patch.properties.reported);

            var patch1 = JsonConvert.DeserializeObject<TwinCollection>(jsonString);
            var deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, deviceId);
            await deviceClient.UpdateReportedPropertiesAsync(patch1);
        }

        public static async Task UpdateReportedHeartbeat(string deviceId)
        {
            dynamic patch = new ExpandoObject();

            patch.properties = new
            {
                reported = new
                {
                    heartbeat = DateTime.UtcNow
                }
            };

            var jsonString = JsonConvert.SerializeObject(patch.properties.reported);

            var patch1 = JsonConvert.DeserializeObject<TwinCollection>(jsonString);
            var deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, deviceId);
            await deviceClient.UpdateReportedPropertiesAsync(patch1);
        }

        public static async Task UpdateReportedStatusCode(string deviceId, int statusCode)
        {
            dynamic patch = new ExpandoObject();

            patch.properties = new
            {
                reported = new
                {
                    heartbeat = DateTime.UtcNow,
                    statusCode = statusCode
                }
            };

            var jsonString = JsonConvert.SerializeObject(patch.properties.reported);

            var patch1 = JsonConvert.DeserializeObject<TwinCollection>(jsonString);
            var deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, deviceId);
            await deviceClient.UpdateReportedPropertiesAsync(patch1);
        }

        public static async Task<DeviceTwinModel> GetDeviceTwinAsync(string deviceId)
        {
            var twin = await registryManager.GetTwinAsync(deviceId);

            var deviceTwinModel = new DeviceTwinModel()
            {
                DeviceId = twin.DeviceId,
                Tags = JsonConvert.DeserializeObject<DeviceTwinTagsModel>(twin.Tags.ToString()),
                Desired = JsonConvert.DeserializeObject<DeviceTwinDesiredModel>(twin.Properties.Desired.ToString()),
                Reported = JsonConvert.DeserializeObject<DeviceTwinReportedModel>(twin.Properties.Reported.ToString()),
            };

            return deviceTwinModel;
        }

        private static List<LocationAddress> _CacheAddresses = new List<LocationAddress>();
        private static int api_calls = 0;
        private static async Task<string> GetRandomUSIP(string retailerRegion)
        {
            string country = string.Empty;
            string region = string.Empty;
            string ipAddress = string.Empty;
            Console.WriteLine(retailerRegion);

            try
            {
                LocationAddress cachedAddress = _CacheAddresses.Find(p => p.RegionName == retailerRegion);
                if (cachedAddress != null)
                {
                    //Cached address found
                    _CacheAddresses.Remove(cachedAddress);
                    return cachedAddress.IpAddress;
                }

                LocationAddress location = null;
                while (country != "US" || region != retailerRegion)
                {
                    if(api_calls > 1250)
                    {
                        //Cool down API - Wait 5 minutes
                        Console.WriteLine("Cooldown API 5 minutes...");
                        System.Threading.Thread.Sleep(300000); // wait 5 minutes every 1250 calls
                        
                        api_calls = 0;
                    }

                    ipAddress = string.Format("{0}.{1}.{2}.{3}", random.Next(24, 242), random.Next(1, 254), random.Next(1, 254), random.Next(1, 254));
                    location = await GetLocationByIPAddress(ipAddress);
                    if (location != null && !string.IsNullOrEmpty(location.RegionCode) && !string.IsNullOrEmpty(location.City))
                    {
                        country = location.CountryCode;
                        region = location.RegionName;
                        if (region != retailerRegion && country == "US" && _CacheAddresses.Count < 50000)
                        {
                            _CacheAddresses.Add(location); //putting in cache to be reused later if needed
                        }
                        if(_CacheAddresses.Count == 50000)
                        {
                            try
                            {
                                var groupsToDelete = _CacheAddresses.GroupBy(p => p.RegionCode).Where(p => p.Count() > 1000);
                                foreach (var groupToDelete in groupsToDelete)
                                    _CacheAddresses.RemoveAll(p => groupToDelete.Contains(p));
                            }
                            catch(Exception) { }
                        }
                    }
                    api_calls++;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("getRandomUSIP crashed with error {0}. Trying again...", e.Message);
                return await GetRandomUSIP(retailerRegion);
            }

            return ipAddress;
        }

        private static async Task<LocationAddress> GetLocationByIPAddress(string ipAddress)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var resp = await client.GetStringAsync("http://freegeoip.net/json");

                    var data = JsonConvert.DeserializeObject<LocationAddress>(resp);
                    data.IpAddress = ipAddress;
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetRandomModel(string familyName)
        {
            return string.Format(names[random.Next(0, names.Length)], familyName);
        }
    }
}
