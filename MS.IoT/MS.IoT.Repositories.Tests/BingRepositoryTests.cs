//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using MS.IoT.Domain.Interface;
//using System.Threading.Tasks;
//using MS.IoT.Domain.Model;
//using Newtonsoft.Json;

//namespace MS.IoT.Repositories.Tests
//{
//    [TestClass]
//    public class BingRepositoryTests
//    {
//        public static readonly string bingMapsApiKey = "Am-PemccWaPtGyGw36n1eGqtKJmjK7MAbuEQhemkTRvDyibkYPzGcw_g2NDl12aQ";

//        [Fact]
//        public void get_address_valid_coordinates()
//        {
//            //BingMapsRepository repo = new BingMapsRepository(bingMapsApiKey);

//            var location = repo.GetLocationCoordinates("2901 S King Drive, Chicago IL 60616");
//            var loc = location.Result.GetEnumerator();
//            while (loc.MoveNext())
//            {
//                var currentLocation = loc.Current;
//                Assert.Equal("2901 S King Dr, Chicago, IL 60616", currentLocation.FormattedAddress);
//                Assert.Equal("41.84165", currentLocation.Coordinates.Latitude.ToString());
//                Assert.Equal("-87.61658", currentLocation.Coordinates.Longitude.ToString());
//            }
//        }

//        [Fact]
//        public void get_address_not_valid()
//        {
//           // BingMapsRepository repo = new BingMapsRepository(bingMapsApiKey);

//            var location = repo.GetLocationCoordinates("ewfewgfegew sdefvsa ewfwef6");
//            var loc = location.Result.GetEnumerator();

//            Assert.False(loc.MoveNext());
//        }

//        [Fact]
//        public void get_address_by_ip_address()
//        {
//            BingMapsRepository repo = new BingMapsRepository(bingMapsApiKey);

//            var location = repo.GetLocationByIPAddress("208.59.148.106");
//            //var loc = location.Result.GetEnumerator();
//            //while (loc.MoveNext())
//            //{
//            //    var currentLocation = loc.Current;
//            //    Assert.Equal("2901 S King Dr, Chicago, IL 60616", currentLocation.FormattedAddress);
//            //    Assert.Equal("41.84165", currentLocation.Coordinates.Latitude.ToString());
//            //    Assert.Equal("-87.61658", currentLocation.Coordinates.Longitude.ToString());
//            //}
//        }
//    }
//}

