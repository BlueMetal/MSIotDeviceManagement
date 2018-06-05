using Microsoft.Azure.ActiveDirectory.GraphClient;
using MS.IoT.Common;
using MS.IoT.DeviceManagementPortal.Web.Helpers;
using MS.IoT.Domain.Interface;
using MS.IoT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.Api
{
    [RoutePrefix("api/ip")]
    public class CheckIPAddressController : ApiController
    {
        public CheckIPAddressController()
        {
        }

        [Route("check")]
        [HttpGet]
        public IHttpActionResult CheckIp()
        {
            return Ok(HttpContext.Current.Request.UserHostAddress);
        }
    }
}