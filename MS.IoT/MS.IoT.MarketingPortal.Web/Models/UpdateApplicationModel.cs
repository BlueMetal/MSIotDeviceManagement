using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class UpdateApplicationModel
    {
        public string AppObjectId { get; set; }
        public string HomePage { get; set; }
        public List<string> ReplyUrls { get; set; }
         
    }
}
