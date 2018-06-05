using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.DeviceTwinSimulator.Models
{
    public class ProductFamilyItem
    {
        public Item ProductFamily { get; set; }
        public List<Item> ProductNames { get; set; }
        public List<string> ProductNamesChances { get; set; }

        public ProductFamilyItem(string name, int percentage)
        {
            ProductFamily = new Item(name, percentage);
            ProductNamesChances = new List<string>();
        }

        public void InitProductNames()
        {
            foreach (Item ProductName in ProductNames)
            {
                for (int i = 0; i < ProductName.Percentage; i++)
                    ProductNamesChances.Add(ProductName.Name);
            }
        }
    }
}
