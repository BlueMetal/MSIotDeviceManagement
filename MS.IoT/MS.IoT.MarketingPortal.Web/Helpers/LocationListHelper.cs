using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Common;
using MS.IoT.MarketingPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MS.IoT.MarketingPortal.Web.Helpers
{
    // list of locations where IoT hub can be deployed
    public static class LocationListHelper
    {
        public static LocationResponseModel GetLocationList()
        {
            LocationResponseModel location = new LocationResponseModel();

            location.LocationList = new List<LocationModel>()
            {
                new LocationModel(){Name="australiaeast",DisplayName="Australia East",},
                new LocationModel(){Name="australiasoutheast",DisplayName="Australia Southeast",},
                new LocationModel(){Name="centralindia",DisplayName="Central India",},
                new LocationModel(){Name="centralus",DisplayName="Central US",},
                new LocationModel(){Name="eastasia",DisplayName="East Asia",},
                new LocationModel(){Name="eastus",DisplayName="East US",},
                new LocationModel(){Name="eastus2",DisplayName="East US 2",},
                new LocationModel(){Name="japaneast",DisplayName="Japan East",},
                new LocationModel(){Name="japanwest",DisplayName="Japan West",},
                new LocationModel(){Name="northeurope",DisplayName="North Europe",},
                new LocationModel(){Name="southindia",DisplayName="South India",},
                new LocationModel(){Name="southeastasia",DisplayName="Southeast Asia",},
                new LocationModel(){Name="uksouth",DisplayName="UK South",},
                new LocationModel(){Name="ukwest",DisplayName="UK West",},
                new LocationModel(){Name="westcentralus",DisplayName="West Central US",},
                new LocationModel(){Name="westeurope",DisplayName="West Europe",},
                new LocationModel(){Name="westus",DisplayName="West US",},
                new LocationModel(){Name="westus2",DisplayName="West US 2",},
            };

            return location;
        }
    }
}