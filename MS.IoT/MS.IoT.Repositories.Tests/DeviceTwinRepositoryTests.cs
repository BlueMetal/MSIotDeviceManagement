using System;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;
using MS.IoT.Repositories;

public class DeviceTwinRepositoryTests
    {
        public static readonly string iotHubConnectionString = "HostName=msiot-iothub-dev.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yrvwTx3onmJW48e3iOy//JlfQzf79ZYVWqeAJVsWo6s=";       

        [Fact]
        public async Task create_iotHub_device()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device=await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);
        }

        [Fact]
        public async Task get_iotHub_device()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            var deviceFound = await repo.GetDevice("unittestdevice");
            Assert.Equal("unittestdevice", deviceFound.Id);
        }

        [Fact]
        public async Task get_iotHub_device_twin()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
            var deviceTwin = await repo.GetDeviceTwinAsync(device.Id);
            Assert.NotNull(deviceTwin);
        }

        [Fact]
        public async Task get_iotHub_device_twin_list()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
            var devicesTwin = await repo.GetDevices();
           // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public async Task get_iotHub_device_twin_list_groupby()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
           // var devicesTwin = await repo.GetDevicesTwinGroupByAsync("retailerName");
            // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public async Task get_iotHub_device_twin_aggregations()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
           // var aggResult= repo.GetDevicesSummaryAggregationsAsync();
            // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public async Task get_iotHub_device_twin_retailer_region_aggregations()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
           // var aggResult = await repo.GetRetailerRegionAggregations();
            // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public void update_iotHub_devices_twin_locations()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
           // await repo.UpdateDevicesTwinLocation();
           // Assert.Equal("coffeemaker1", device.Id);

            // get device twin
            //var aggResult = await repo.GetDevicesSummaryAggregationsAsync();
            // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public void update_iotHub_devices_twin_status()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            //await repo.UpdateDevicesStatus();
            // Assert.Equal("coffeemaker1", device.Id);

            // get device twin
            //var aggResult = await repo.GetDevicesSummaryAggregationsAsync();
            // Assert.NotNull(deviceTwin);
        }

        [Fact]
        public async Task delete_iotHub_device()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            await repo.DeleteDeviceAsync(device.Id);

            var getDevice = await repo.GetDevice(device.Id);
            Assert.Null(getDevice);
        }

        [Fact]
        public async Task delete_multiple_iotHub_devices()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var device1 = await repo.CreateDevice("coffeemaker1");
            var device2 = await repo.CreateDevice("coffeemaker2");
            var device3 = await repo.CreateDevice("coffeemaker3");
            List<string> devices = new List<string> { device1.Id, device2.Id, device3.Id };
            await repo.DeleteMultipleDevicesAsync(devices);

            var getDevice1 = await repo.GetDevice(device1.Id);
            Assert.Null(getDevice1);
            var getDevice2 = await repo.GetDevice(device2.Id);
            Assert.Null(getDevice2);
            var getDevice3 = await repo.GetDevice(device3.Id);
            Assert.Null(getDevice3);
        }

        //[Fact]
        //public async Task direct_method_iotHub_test()
        //{
        //    DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
        //    repo.up
        //    //await repo.UpdateDevicesStatus();
        //    // Assert.Equal("coffeemaker1", device.Id);

        //    // get device twin
        //    //var aggResult = await repo.GetDevicesSummaryAggregationsAsync();
        //    // Assert.NotNull(deviceTwin);
        //}

        [Fact(Skip = "Skipped")]
        public async Task get_iotHub_update_device_twin()
        {
            DeviceTwinRepository repo = new DeviceTwinRepository(iotHubConnectionString);
            var tags = new DeviceTwinTagsModel()
            {
                ProductFamily = "ProductFamilyTest",
                ProductName = "ProductNameTest",
                RetailerName = "RetailerTest",
                ManufacturedDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                RetailerRegion = "Chicago"
            };
            var device = await repo.CreateAndInitializeDeviceTwin("unittestdevice", tags);
            Assert.Equal("unittestdevice", device.Id);

            // get device twin
            var deviceTwin = await repo.GetDeviceTwinAsync(device.Id);
            Assert.NotNull(deviceTwin);

            var deviceTwinModel = new DeviceTwinModel()
            {
                DeviceId = "unittestdevice",
                /*Location = new DeviceLocation()
                {
                    AddressLine1 = "1100, S King Drive",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "50630",
                    Country = "US",
                    AdditionalNotes="Test Notes"
                },
                Features = new List<DeviceFeature>()
                {
                    new DeviceFeature(){ Name = "Just Test Feature",
                    Value = true }
                    
                },
                ActivationDate = DateTime.Now,
                ShipDate = DateTime.Now.AddDays(2)*/
            };

            //await repo.UpdateDeviceTwinTagsAsync(deviceTwinModel);
            await repo.InitializeDeviceTwinDesiredFeaturesAsync(new DeviceTwinDesiredFeaturesModel() { DeviceId = device.Id /*, ActivatedFeatures = deviceTwinModel.Features*/ });

            // get device twin
            deviceTwin = await repo.GetDeviceTwinAsync(device.Id);
         
            //Assert.Equal(1,deviceTwin.Tags.Count);
            //Assert.True(deviceTwin.Tags.Contains("location"));

            //Assert.Equal(1, deviceTwin.Properties.Desired.Count);
            //Assert.True(deviceTwin.Properties.Desired.Contains("features"));
        }
    }

