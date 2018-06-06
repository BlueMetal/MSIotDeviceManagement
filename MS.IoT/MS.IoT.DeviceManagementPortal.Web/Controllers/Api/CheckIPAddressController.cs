using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.IoT.DeviceManagementPortal.Web.Controllers.API
{
    [Route("api/ip")]
    public class CheckIPAddressController : Controller
    {
        public CheckIPAddressController()
        {
        }

        [Route("check")]
        [HttpGet]
        public IActionResult CheckIp()
        {
            return Ok(HttpContext.Connection.RemoteIpAddress);
        }
    }
}