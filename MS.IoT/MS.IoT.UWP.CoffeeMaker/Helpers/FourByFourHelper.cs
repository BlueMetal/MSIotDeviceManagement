using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    public static class FourByFourHelper
    {
        private static ushort _IndexId = short.MaxValue + 1;

        public static ushort GetNewMessageId()
        {
            ushort newId = _IndexId;

            if (_IndexId == ushort.MaxValue)
                _IndexId = short.MaxValue + 1;
            else
                _IndexId++;

            return newId;
        }
    }
}
