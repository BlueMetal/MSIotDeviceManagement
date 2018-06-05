using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.DeviceTwinSimulator.Models
{
    public class RetailerState
    {
        public Item State { get; }
        public int ActivationChance { get; set; }

        public RetailerState(string name, int percentage, int activationChance)
        {
            State = new Item(name, percentage);
            ActivationChance = activationChance;
        }
    }
}
