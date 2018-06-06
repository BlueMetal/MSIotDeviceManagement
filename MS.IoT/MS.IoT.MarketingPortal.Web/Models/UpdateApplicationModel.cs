using System.Collections.Generic;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class UpdateApplicationModel
    {
        public string AppObjectId { get; set; }
        public string HomePage { get; set; }
        public List<string> ReplyUrls { get; set; }
         
    }
}
