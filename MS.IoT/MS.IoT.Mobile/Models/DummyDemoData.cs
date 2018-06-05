using MS.IoT.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public static class DummyDemoData
    {





#if DEMO
        // Dummy Device Definitions for Demo
        public static readonly Device DummyDevice1 = new Device
        {
            DeviceId = "1",
            DeviceName = "Connected Stove",
            DeviceType = "stove",
            Status = 0
        };

        public static readonly Device DummyDevice2 = new Device
        {
            DeviceId = "01633f79-a393-4f16-b000-ac160b2a40c4",
            DeviceName = "My Coffee Maker",
            DeviceType = "coffeemaker",
            Status = 0
        };

        public static readonly Device DummyDevice3 = new Device
        {
            DeviceId = "3",
            DeviceName = "My Washer",
            DeviceType = "washingmachine",
            Status = 0
        };

        public static readonly Feature DummyFeature1_1 = new Feature
        {
            FeatureType = FeatureType.Selection,
            SubType = SubType.OvenTemp,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature1_1",
            Title = "Oven Temperature",
            MethodName = null,
            CustomIcon = null
        };
        public static readonly Feature DummyFeature1_2 = new Feature
        {
            FeatureType = FeatureType.Selection,
            SubType = SubType.Duration,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature1_2",
            Title = "Cook Time",
            MethodName = null,
            CustomIcon = null
        };
        public static readonly Feature DummyFeature1_3 = new Feature
        {
            FeatureType = FeatureType.Action,
            SubType = SubType.Undefined,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature1_3",
            Title = "Start Cooking",
            MethodName = null,
            CustomIcon = null
        };


        public static readonly Feature DummyFeature2_1 = new Feature
        {
            FeatureType = FeatureType.Selection,
            SubType = SubType.BrewStrength,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "brewStrengthFeature",
            Title = "Brew Strength",
            MethodName = "changeBrewStrength",
            CustomIcon = null
        };
        public static readonly Feature DummyFeature2_2 = new Feature
        {
            FeatureType = FeatureType.Action,
            SubType = SubType.Undefined,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "brewFeature",
            Title = "Brew",
            MethodName = "launchBrew",
            CustomIcon = null
        };
        public static readonly Feature DummyFeature2_3 = new Feature
        {
            FeatureType = FeatureType.Action,
            SubType = SubType.Undefined,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "grindAndBrewFeature",
            Title = "Grind and Brew",
            MethodName = "launchGrindAndBrew",
            CustomIcon = null
        };

        public static readonly Feature DummyFeature3_1 = new Feature
        {
            FeatureType = FeatureType.Selection,
            SubType = SubType.WashCycle,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature3_1",
            Title = "Wash Cycle",
            MethodName = null,
            CustomIcon = null
        };
        public static readonly Feature DummyFeature3_2 = new Feature
        {
            FeatureType = FeatureType.Selection,
            SubType = SubType.WashTemp,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature3_2",
            Title = "Wash Temperature",
            MethodName = null,
            CustomIcon = null
        };
        public static readonly Feature DummyFeature3_3 = new Feature
        {
            FeatureType = FeatureType.Action,
            SubType = SubType.Undefined,
            FeatureId = Guid.NewGuid().ToString(),
            FeatureName = "DummyFeature3_3",
            Title = "Start Wash",
            MethodName = null,
            CustomIcon = null
        };


        public static readonly List<Notification> DummyNotificationList = new List<Notification> {
            new Notification {NotificationType = NotificationType.Offer, Title = "Save 25% on Coffee Maker warranty", ReceivedDTS = DateTime.Now.Subtract(new TimeSpan(3,0,0,0)), Read = false, Id = Guid.NewGuid().ToString() },
            new Notification {NotificationType = NotificationType.Maintenance, Title = "Your connected Coffee Maker is due for grinder cleaning", ReceivedDTS = DateTime.Now.Subtract(new TimeSpan(3,0,0)), Read = false, Id = Guid.NewGuid().ToString()  },

            new Notification {NotificationType = NotificationType.Feature, Title = "New Feature Available. Grind and Brew. Freshly grind your coffee before brewing for a better cup", ReceivedDTS = DateTime.Now, DeviceId = "01633f79-a393-4f16-b000-ac160b2a40c4",
                FeatureId = "newFeature", FeatureName = "New Feature" //grindAndBrewFeature", FeatureName = "Grind and Brew"
                , MethodName="launchGrindAndBrew", Id = Guid.NewGuid().ToString(), Read = false, FeatureType=FeatureType.Action  }
        };




        public static void AddDemoDevices(List<Device> devices)
        {
            Settings.DemoDevicesAdded = true;
            if (devices.Count == 0)
            {
                Device dummyDevice = DummyDevice1;
                List<Feature> dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature1_1);
                dummyFeatures.Add(DummyFeature1_2);
                dummyFeatures.Add(DummyFeature1_3);
                dummyDevice.Features = dummyFeatures;
                devices.Add(dummyDevice);
                dummyDevice = DummyDevice2;
                dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature2_1);
                dummyFeatures.Add(DummyFeature2_2);
                dummyFeatures.Add(DummyFeature2_3);
                dummyDevice.Features = dummyFeatures;
                devices.Add(dummyDevice);
                dummyDevice = DummyDevice3;
                dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature3_1);
                dummyFeatures.Add(DummyFeature3_1);
                dummyFeatures.Add(DummyFeature3_3);
                dummyDevice.Features = dummyFeatures;
                devices.Add(dummyDevice);
            }
            else if (devices.Count == 1)
            {
                Device dummyDevice = DummyDevice1;
                List<Feature> dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature1_1);
                dummyFeatures.Add(DummyFeature1_2);
                dummyFeatures.Add(DummyFeature1_3);
                dummyDevice.Features = dummyFeatures;
                devices.Insert(0, dummyDevice);
                dummyDevice = DummyDevice3;
                dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature3_1);
                dummyFeatures.Add(DummyFeature3_2);
                dummyFeatures.Add(DummyFeature3_3);
                dummyDevice.Features = dummyFeatures;
                devices.Add(dummyDevice);
            }
            else if (devices.Count == 2)
            {
                Device dummyDevice = DummyDevice1;
                List<Feature> dummyFeatures = new List<Feature>();
                dummyFeatures.Add(DummyFeature1_1);
                dummyFeatures.Add(DummyFeature1_2);
                dummyFeatures.Add(DummyFeature1_3);
                dummyDevice.Features = dummyFeatures;
                devices.Insert(0, dummyDevice);
            }
        }


        public static List<Notification> GetDemoNotifications()
        {
            // Add demo notifications
            Settings.DemoNotificationsAdded = true;
            return DummyNotificationList;
        }


#endif




    }
}
