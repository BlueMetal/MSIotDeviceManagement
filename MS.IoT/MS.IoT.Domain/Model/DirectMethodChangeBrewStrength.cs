using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public class DirectMethodChangeBrewStrength : DirectMethodBase
    {
        public enum BrewStrength
        {
            Regular = 0,
            Bold = 1,
            Strong = 2
        }

        public BrewStrength Strength { get; private set; }

        public DirectMethodChangeBrewStrength(BrewStrength strength)
            : base("changeBrewStrength")
        {
            Strength = strength;
        }

        public override string GetJson()
        {
            return "{strength:" + (int)Strength + "}";
        }
    }
}
