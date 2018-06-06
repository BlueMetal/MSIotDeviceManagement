using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Domain.Interface;
using System.Threading.Tasks;
using MS.IoT.Domain.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;
using MS.IoT.Repositories;

public class UserDeviceTwinRepositoryTests
    {
        public static readonly string iotHubConnectionString = "HostName=msiot-iothub-dev.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yrvwTx3onmJW48e3iOy//JlfQzf79ZYVWqeAJVsWo6s=";       
      
        [Fact]
        public Task direct_method_iotHub_test()
        {
            UserDeviceTwinRepository repo = new UserDeviceTwinRepository(iotHubConnectionString);
        //    string deviceId = "01633f79-a393-4f16-b000-ac160b2a40c4";

            return Task.CompletedTask;
            //var response = await repo.UpdateDeviceFeatureDirectMethod(deviceId, new DirectMethodChangeBrewStrength(DirectMethodChangeBrewStrength.BrewStrength.Bold));
            //Console.WriteLine(response.Status);
            //Console.WriteLine(response.NewValue);

            //response = await repo.UpdateDeviceFeatureDirectMethod(deviceId, new DirectMethodLaunchBrew());
            //Console.WriteLine(response.Status);
            //Console.WriteLine(response.TimeTillCompletion);
            //await repo.UpdateDevicesStatus();
            // Assert.Equal("coffeemaker1", device.Id);

            // get device twin
            //var aggResult = await repo.GetDevicesSummaryAggregationsAsync();
            // Assert.NotNull(deviceTwin);
        }
    }


