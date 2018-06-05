using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class CosmosDBInitModel
    {
        public string CosmosDBAccountEndPoint { get; set; }
        public string CosmosDBAccountKey { get; set; }     
    }
}
