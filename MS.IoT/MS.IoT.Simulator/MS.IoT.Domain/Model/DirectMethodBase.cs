using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DirectMethodBase
    {
        public string MethodName { get; private set; }
        
        public DirectMethodBase(string methodName)
        {
            MethodName = methodName;
        }

        public virtual string GetJson()
        {
            return "{}";
        }
    }
}
