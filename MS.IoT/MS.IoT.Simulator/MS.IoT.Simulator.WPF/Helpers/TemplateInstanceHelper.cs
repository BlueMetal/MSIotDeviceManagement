using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MS.IoT.Simulator.WPF.Helpers
{
    /// <summary>
    /// TemplateInstanceHelper
    /// Static class used to generate randomized data based on a specific Template
    /// </summary>
    public static class TemplateInstanceHelper
    {
        private static Random random = new Random();
        private static string[] list = new string[] { "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming" };
        private static string[] brands = new string[] { "Subzero", "Siemens", "Omega", "KWC", "Bosch", "LG", "Samsung" };
        private static string[] addresses = new string[] { "10055 Demo Street", "45066 Simulation Avenue", "86210 4x4 Street" };
        private static string[] cities = new string[] { "Chicago", "New York", "Seattle", "Pittsburgh", "Los Angeles", "San Fransisco", "Dallas", "Denver", "Washington", "Newark", "Saint Louis" };
        private static string[] countries = new string[] { "U.S.A.", "Canada", "Mexico" };
        private static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Returns a TemplatePacket based on a specific template
        /// </summary>
        /// <param name="templateModel">Template</param>
        /// <returns></returns>
        public static TemplatePacket GetTemplatePacketFromTemplate(Template templateModel)
        {
            TemplatePacket template = new TemplatePacket();
            template.TemplateId = templateModel.Id;
            template.Properties = new Dictionary<string, dynamic>();
            foreach (CustomProperty property in templateModel.Properties)
                template.Properties.Add(property.Name, GetDynamicObjectFromCustomProperty(property));
            return template;
        }

        /// <summary>
        /// Creates a tree of dynamic values based on a customProperty
        /// Recursive method.
        /// </summary>
        /// <param name="property">CustomProperty</param>
        /// <returns></returns>
        private static dynamic GetDynamicObjectFromCustomProperty(CustomProperty property)
        {
            switch (property.Type)
            {
                case CustomPropertyType.Boolean:
                    return GetRandomBoolValue();
                case CustomPropertyType.Date:
                    return GetRandomDateTimeValue();
                case CustomPropertyType.List:
                    return GetRandomListValues();
                case CustomPropertyType.Number:
                    return GetRandomDoubleValue();
                case CustomPropertyType.Text:
                    return GetRandomTextValue(property.Name);
                case CustomPropertyType.Object:
                    Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                    foreach (CustomProperty subProperty in property.Properties)
                        values.Add(subProperty.Name, GetDynamicObjectFromCustomProperty(subProperty));
                    return values;
            }
            return null;
        }

        /// <summary>
        /// Randomize a boolean value
        /// </summary>
        /// <returns></returns>
        private static bool GetRandomBoolValue()
        {
            int value = random.Next(0, 2);
            if (value == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Randomize a text value. A property parameter is passed in order to provide some user-friendly values depending of the property value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string GetRandomTextValue(string propertyName)
        {
            //Detecting some specific demo property to show better values
            switch (propertyName)
            {
                case "message_id":
                    return Guid.NewGuid().ToString();
                case "device_id":
                    return AppConfig.IoTHub.SimulatorDeviceName;
                case "model":
                    return string.Format("Model_{0}", new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray()));
                case "brand":
                    return brands[random.Next(0, brands.Length - 1)];
                case "address":
                    return addresses[random.Next(0, addresses.Length - 1)];
                case "city":
                    return cities[random.Next(0, cities.Length - 1)];
                case "zipcode":
                    return random.Next(10000, 70000).ToString();
                case "country":
                    return countries[random.Next(0, countries.Length - 1)];
            }

            return new string(Enumerable.Repeat(chars, 12)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Provide a randomized list of states.
        /// </summary>
        /// <returns></returns>
        private static List<string> GetRandomListValues()
        {
            List<string> newList = new List<string>();

            int nbrItems = random.Next(1, 10);

            for (int i = 0; i < nbrItems; i++)
                newList.Add(list[random.Next(0, list.Length - 1)]);

            return newList;
        }

        /// <summary>
        /// Provide a randomized double value
        /// </summary>
        /// <returns></returns>
        private static double GetRandomDoubleValue()
        {
            return random.NextDouble() * 100;
        }

        /// <summary>
        /// Provide a randomized date value
        /// </summary>
        /// <returns></returns>
        private static DateTime GetRandomDateTimeValue()
        {
            return DateTime.UtcNow.AddDays(random.Next(0,90));
        }
    }
}
