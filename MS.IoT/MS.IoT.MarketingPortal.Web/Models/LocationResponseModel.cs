using System.Collections.Generic;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class LocationResponseModel
    {
        public List<LocationModel> LocationList { get; set; }      
    }
    public class LocationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }     
    }
}
