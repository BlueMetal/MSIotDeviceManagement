using Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class ServicePrincipalResponse
    {
        public IApplication App { get; set; }
        public IServicePrincipal Principal { get; set; }
        public string AppClientSecret { get; set; }
        public bool IsNewApp { get; set; }
        public bool IsNewPrincipal { get; set; }
    }
}
