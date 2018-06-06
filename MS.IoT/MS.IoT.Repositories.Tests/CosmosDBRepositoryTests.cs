using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MS.IoT.Repositories.Tests
{
    //[TestClass]
    //public class CosmosDBRepositoryTests
    //{
    //    public static readonly string iotHubConnectionString = "HostName=iothubnhebubkulhjwe.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3HmlNLDKiLBZC26VoandWTpVhPx2YEcLMHYQjJbS3FI=";
    //    public static readonly string endpoint = "https://cosmosdbnhebubkulhjwe.documents.azure.com:443/";
    //    public static readonly string authkey = "AN87gpLR9MZ2YlX5JqUYBAikwMj1vc6kCcEMNFtRfNI2RHPVNZ0gYfX5KhE4PX5p9Qwu9fdWMDlKlMI1WrVgXQ==";
    //    public static readonly string database = "MSIoT";
    //    public static readonly string colDevices = "Devices";

    //    [TestMethod]
    //    [Ignore]
    //    public async Task get_cosmosdb_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        // first delete the item
    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        DeviceModel deviceModelGet = new DeviceModel();

    //        if (isDeviceExists)
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //            await repo.DeleteItemAsync(deviceModelGet.Id);
    //        }

    //        // now create item
    //        var deviceModel = new DeviceModel()
    //        {
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "Chicago",
    //                State = "IL",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = true,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "BestBy"
    //        };

    //        isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        if (!isDeviceExists)
    //        {
    //            var deviceDocId = await repo.CreateItemAsync(deviceModel);
    //            deviceModelGet = repo.GetItemAsync(deviceDocId).Result;
    //        }
    //        else
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //        }

    //        Assert.AreEqual(deviceModel.DeviceId, deviceModelGet.DeviceId);

    //        Assert.AreEqual(deviceModel.Location.AddressLine1, deviceModelGet.Location.AddressLine1);
    //        Assert.AreEqual(deviceModel.Location.AddressLine2, deviceModelGet.Location.AddressLine2);
    //        Assert.AreEqual(deviceModel.Location.City, deviceModelGet.Location.City);
    //        Assert.AreEqual(deviceModel.Location.State, deviceModelGet.Location.State);
    //        Assert.AreEqual(deviceModel.Location.Country, deviceModelGet.Location.Country);
    //        Assert.AreEqual(deviceModel.Location.ZipCode, deviceModelGet.Location.ZipCode);
    //        Assert.AreEqual(deviceModel.Location.AdditionalNotes, deviceModelGet.Location.AdditionalNotes);

    //        //Assert.AreEqual(deviceModel.Features[0].Name, deviceModelGet.Features[0].Name);
    //        //Assert.AreEqual(deviceModel.Features[0].Value, deviceModelGet.Features[0].Value);

    //        Assert.AreEqual(deviceModel.CreatedBy, deviceModelGet.CreatedBy);
    //        Assert.AreEqual(deviceModel.Retailer, deviceModelGet.Retailer);
    //        //Assert.AreEqual(deviceModel.IsConnected, deviceModelGet.IsConnected);
    //    }

    //    [TestMethod]
    //    [Ignore]
    //    public async Task create_cosmosdb_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        // first delete the item
    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        DeviceModel deviceModelGet = new DeviceModel();

    //        if (isDeviceExists)
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //            await repo.DeleteItemAsync(deviceModelGet.Id);
    //        }

    //        // now create item
    //        var deviceModel = new DeviceModel()
    //        {
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "Chicago",
    //                State = "IL",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = true,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "BestBy"
    //        };

    //        isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
            
    //        if (!isDeviceExists)
    //        {
    //            var deviceDocId = await repo.CreateItemAsync(deviceModel);
    //            deviceModelGet = repo.GetItemAsync(deviceDocId).Result;              
    //        }
    //        else
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p=>p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //        }

    //        Assert.AreEqual(deviceModel.DeviceId, deviceModelGet.DeviceId);

    //        Assert.AreEqual(deviceModel.Location.AddressLine1, deviceModelGet.Location.AddressLine1);
    //        Assert.AreEqual(deviceModel.Location.AddressLine2, deviceModelGet.Location.AddressLine2);
    //        Assert.AreEqual(deviceModel.Location.City, deviceModelGet.Location.City);
    //        Assert.AreEqual(deviceModel.Location.State, deviceModelGet.Location.State);
    //        Assert.AreEqual(deviceModel.Location.Country, deviceModelGet.Location.Country);
    //        Assert.AreEqual(deviceModel.Location.ZipCode, deviceModelGet.Location.ZipCode);
    //        Assert.AreEqual(deviceModel.Location.AdditionalNotes, deviceModelGet.Location.AdditionalNotes);

    //        //Assert.AreEqual(deviceModel.Features[0].Name, deviceModelGet.Features[0].Name);
    //        //Assert.AreEqual(deviceModel.Features[0].Value, deviceModelGet.Features[0].Value);

    //        Assert.AreEqual(deviceModel.CreatedBy, deviceModelGet.CreatedBy);
    //        Assert.AreEqual(deviceModel.Retailer, deviceModelGet.Retailer);
    //        //Assert.AreEqual(deviceModel.IsConnected, deviceModelGet.IsConnected);
    //    }

    //    [TestMethod]
    //    [Ignore]
    //    public async Task cosmosdb_device_already_exists_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        var deviceModel = new DeviceModel()
    //        {
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "Chicago",
    //                State = "IL",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = true,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "BestBy"
    //        };

    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        if (!isDeviceExists)
    //        {
    //            var deviceDocId = await repo.CreateItemAsync(deviceModel);
    //            isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            Assert.IsTrue(isDeviceExists);
    //        }
    //        else
    //        {
    //            Assert.IsTrue(isDeviceExists);
    //        }          
    //    }

    //    [TestMethod]
    //    [Ignore]
    //    public async Task cosmosdb_device_doesnot_exists_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        var deviceModel = new DeviceModel()
    //        {
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "Chicago",
    //                State = "IL",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = true,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "BestBy"
    //        };

    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice1")).Result;
    //        Assert.IsFalse(isDeviceExists);
    //    }

    //    [TestMethod]
    //    [Ignore]
    //    public async Task update_cosmosdb_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        // first delete the item
    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        DeviceModel deviceModelGet = new DeviceModel();

    //        if (isDeviceExists)
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //            await repo.DeleteItemAsync(deviceModelGet.Id);
    //        }
           
    //        // now create again
    //        var deviceModel = new DeviceModel()
    //        {
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "Chicago",
    //                State = "IL",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }
                    
    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = true,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "BestBy"
    //        };

    //        isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        if (!isDeviceExists)
    //        {
    //            var deviceDocId = await repo.CreateItemAsync(deviceModel);
    //            deviceModelGet = repo.GetItemAsync(deviceDocId).Result;
    //        }
    //        else
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //        }

    //        Assert.AreEqual(deviceModel.DeviceId, deviceModelGet.DeviceId);

    //        Assert.AreEqual(deviceModel.Location.AddressLine1, deviceModelGet.Location.AddressLine1);
    //        Assert.AreEqual(deviceModel.Location.AddressLine2, deviceModelGet.Location.AddressLine2);
    //        Assert.AreEqual(deviceModel.Location.City, deviceModelGet.Location.City);
    //        Assert.AreEqual(deviceModel.Location.State, deviceModelGet.Location.State);
    //        Assert.AreEqual(deviceModel.Location.Country, deviceModelGet.Location.Country);
    //        Assert.AreEqual(deviceModel.Location.ZipCode, deviceModelGet.Location.ZipCode);
    //        Assert.AreEqual(deviceModel.Location.AdditionalNotes, deviceModelGet.Location.AdditionalNotes);

    //        //Assert.AreEqual(deviceModel.Features[0].Name, deviceModelGet.Features[0].Name);
    //        //Assert.AreEqual(deviceModel.Features[0].Value, deviceModelGet.Features[0].Value);

    //        Assert.AreEqual(deviceModel.CreatedBy, deviceModelGet.CreatedBy);
    //        Assert.AreEqual(deviceModel.Retailer, deviceModelGet.Retailer);
    //        //Assert.AreEqual(deviceModel.IsConnected, deviceModelGet.IsConnected);

    //        // now update
    //        var deviceUpdateModel = new DeviceModel()
    //        {
    //            Id= deviceModelGet.Id,
    //            DeviceId = "unittestdevice",
    //            Location = new DeviceLocation()
    //            {
    //                AddressLine1 = "1100, S King Drive",
    //                City = "New York",
    //                State = "NY",
    //                ZipCode = "50630",
    //                Country = "US",
    //                AdditionalNotes = "Test Notes"
    //            },
    //            /*Features = new List<DeviceFeature>()
    //            {
    //                new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //            },*/
    //            ActivationDate = DateTime.Now,
    //            ShipDate = DateTime.Now.AddDays(2),
    //            //IsConnected = false,
    //            CreatedBy = "testuser@gmail.com",
    //            Retailer = "Amazon"
    //        };

    //        await repo.UpdateItemAsync(deviceModelGet.Id, deviceUpdateModel);

    //        var deviceupdateModelGet = repo.GetItemAsync(deviceModelGet.Id).Result;

    //        Assert.AreEqual(deviceModel.DeviceId, deviceupdateModelGet.DeviceId);

    //        Assert.AreEqual(deviceUpdateModel.Location.AddressLine1, deviceupdateModelGet.Location.AddressLine1);
    //        Assert.AreEqual(deviceUpdateModel.Location.AddressLine2, deviceupdateModelGet.Location.AddressLine2);
    //        Assert.AreEqual(deviceUpdateModel.Location.City, deviceupdateModelGet.Location.City);
    //        Assert.AreEqual(deviceUpdateModel.Location.State, deviceupdateModelGet.Location.State);
    //        Assert.AreEqual(deviceUpdateModel.Location.Country, deviceupdateModelGet.Location.Country);
    //        Assert.AreEqual(deviceUpdateModel.Location.ZipCode, deviceupdateModelGet.Location.ZipCode);
    //        Assert.AreEqual(deviceUpdateModel.Location.AdditionalNotes, deviceupdateModelGet.Location.AdditionalNotes);

    //        //Assert.AreEqual(deviceUpdateModel.Features[0].Name, deviceupdateModelGet.Features[0].Name);
    //        //Assert.AreEqual(deviceUpdateModel.Features[0].Value, deviceupdateModelGet.Features[0].Value);

    //        Assert.AreEqual(deviceUpdateModel.ActivationDate, deviceupdateModelGet.ActivationDate);
    //        Assert.AreEqual(deviceUpdateModel.ShipDate, deviceupdateModelGet.ShipDate);
    //        Assert.AreEqual(deviceUpdateModel.CreatedBy, deviceupdateModelGet.CreatedBy);
    //        Assert.AreEqual(deviceUpdateModel.Retailer, deviceupdateModelGet.Retailer);
    //        //Assert.AreEqual(deviceUpdateModel.IsConnected, deviceupdateModelGet.IsConnected);
    //    }

    //    [TestMethod]
    //    [Ignore]
    //    public async Task delete_cosmosdb_item()
    //    {
    //        CosmosDBRepository<DeviceModel> repo = new CosmosDBRepository<DeviceModel>(
    //            endpoint, authkey, database, colDevices);
    //        await repo.Initialize();

    //        var isDeviceExists = repo.IsItemExistsByNonIdAsync(p => p.DeviceId.Equals("unittestdevice")).Result;

    //        DeviceModel deviceModelGet = new DeviceModel();

    //        if (isDeviceExists)
    //        {
    //            var devicesGetList = repo.GetItemsAsync(p => p.DeviceId.Equals("unittestdevice")).Result;
    //            var dev = devicesGetList.GetEnumerator();
    //            while (dev.MoveNext())
    //            {
    //                deviceModelGet = dev.Current;
    //            }
    //            await repo.DeleteItemAsync(deviceModelGet.Id);
    //            var deviceAfterDelete = repo.GetItemAsync(deviceModelGet.Id).Result;
    //            Assert.IsNull(deviceAfterDelete);
    //        }
    //        else
    //        {
    //            var deviceModel = new DeviceModel()
    //            {
    //                DeviceId = "unittestdevice",
    //                Location = new DeviceLocation()
    //                {
    //                    AddressLine1 = "1100, S King Drive",
    //                    City = "Chicago",
    //                    State = "IL",
    //                    ZipCode = "50630",
    //                    Country = "US",
    //                    AdditionalNotes = "Test Notes"
    //                },
    //                /*Features = new List<DeviceFeature>()
    //                {
    //                    new DeviceFeature(){
    //                    Name = "Just Test Feature",
    //                    Value = true }

    //                },*/
    //                ActivationDate = DateTime.Now,
    //                ShipDate = DateTime.Now.AddDays(2),
    //                //IsConnected = true,
    //                CreatedBy = "testuser@gmail.com",
    //                Retailer = "BestBy"
    //            };
    //            var deviceDocId = await repo.CreateItemAsync(deviceModel);

    //            deviceModelGet = repo.GetItemAsync(deviceDocId).Result;

    //            Assert.AreEqual(deviceModel.DeviceId, deviceModelGet.DeviceId);

    //            // delete item
    //            await repo.DeleteItemAsync(deviceDocId);
    //            var deviceAfterDelete = repo.GetItemAsync(deviceDocId).Result;
    //            Assert.IsNull(deviceAfterDelete);
    //        }           
    //    }
    //}
}

