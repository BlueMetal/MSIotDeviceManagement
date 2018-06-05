using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MS.IoT.Mobile
{
    public class Device
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int Status { get; set; }

        public List<Feature> Features { get; set; }

        public string DeviceType { get; set; } = "CoffeeMaker";


        public Device() { }

        public Device(ReturnedDevice dto)
        {
            if (dto != null)
            {
                DeviceId = dto.DeviceId;
                DeviceName = dto.Tags.ProductName;
                Status = dto.Reported.StatusCode;
                DeviceType = dto.Tags.ProductType;

                List<Feature> features = new List<Feature>();
                List<FeatureDefinition> featureDefs = dto.Reported?.Features?.Values?.ToList();
                if (featureDefs != null)
                {
                    foreach (FeatureDefinition featureDef in featureDefs)
                    {
                        // Only take activated features. Ignore debug and wifi features
                        if (featureDef.IsActivated && !featureDef.IsInternal && !featureDef.Name.Contains("wifi") && !featureDef.Name.Contains("debug"))
                        {
                            var feature = new Feature(featureDef);
                            features.Add(feature);
                        }
                    }
                }
                Features = features;
            }
        }


    }
}
