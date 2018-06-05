using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.DeviceTwinSimulator.Models
{
    public class Item
    {
        public string Name { get; set; }
        public int Percentage { get; set; }

        public Item(string name, int percentage)
        {
            Name = name;
            Percentage = percentage;
        }
    }
}
