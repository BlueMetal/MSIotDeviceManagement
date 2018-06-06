using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DeploymentRequest
    {
        public string ApplicationName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string Location { get; set; }
        public string DataPacketDesignerPackageWebZipUri { get; set; }     
        public string DeviceManagementPortalPackageWebZipUri { get; set; }
    }
}
