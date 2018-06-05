using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DirectMethodLaunchBrewGrind : DirectMethodBase
    {
        public DirectMethodLaunchBrewGrind()
            : base("launchGrindAndBrew")
        {
        }
    }
}
