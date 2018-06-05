using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.DeviceTwinSimulator.Models
{
    public class RetailerItem
    {
        static Random random = new Random();
        public Item Retailer { get; }
        public List<ProductFamilyItem> Families { get; set; }
        public List<ProductFamilyItem> FamiliesChances { get; set; }
        public List<DateTime> ShipmentDates { get; set; }
        public List<RetailerState> States { get; set; }
        public List<RetailerState> StateChances { get; set; }

        public RetailerItem(string name, int percentage)
        {
            Retailer = new Item(name, percentage);
            States = new List<RetailerState>();
            Families = new List<ProductFamilyItem>();
            ShipmentDates = new List<DateTime>();
            StateChances = new List<RetailerState>();
            FamiliesChances = new List<ProductFamilyItem>();

            int dateChance = random.Next(3, 10);
            for (int i = 0; i < dateChance; i++)
            {
                DateTime randomDate = new DateTime(2017, random.Next(1, 12), random.Next(1, 28));
                int chance = random.Next(1, 10);
                for (int j = 0; j < chance; j++)
                    ShipmentDates.Add(randomDate);
            }
        }

        public void InitValues()
        {
            foreach (RetailerState State in States)
            {
                for (int i = 0; i < State.State.Percentage; i++)
                    StateChances.Add(State);
            }

            foreach (ProductFamilyItem Family in Families)
            {
                foreach (Item ProductName in Family.ProductNames)
                {
                    for (int i = 0; i < ProductName.Percentage; i++)
                        Family.ProductNamesChances.Add(ProductName.Name);
                }
                for (int i = 0; i < Family.ProductFamily.Percentage; i++)
                    FamiliesChances.Add(Family);
            }
        }
    }
}
