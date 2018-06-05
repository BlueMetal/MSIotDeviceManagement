using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MS.IoT.MarketingPortal.Web.Models
{
    public class DeploymentStatusModel
    {
        public string ApplicationName { get; set; }
        public string SubscriptionId { get; set; }       
    }
}
