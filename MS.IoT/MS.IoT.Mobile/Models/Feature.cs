using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MS.IoT.Mobile
{
    public class Feature
    {
        public FeatureType FeatureType { get; set; } = FeatureType.SubType;

        public SubType SubType { get; set; } = SubType.Undefined;

        public string FeatureId { get; set; }

        public string FeatureName { get; set; }

        public string MethodName { get; set; } = null;

        public string CustomIcon { get; set; }
        public string Title { get; set; }







        public string FeatureMethodName { get; set; }
        public string FeatureMethodParameter { get; set; } = "null";

        public Dictionary<string, object> Parameters { get; set; }


        public Feature() { }


        public Feature(FeatureDefinition dto)
        {
            FeatureName = dto.Name;
            Title = dto.DisplayName;
            // TODO:  The server should also send a feature ID wit each feature. In the meantime, use the FeatureName
            FeatureId = dto.Name;

            // TODO: This needs to be redone in a more generic fashion
            string[] methods = dto.MethodNames.Split(',');

            FeatureMethodName = methods[methods.Length - 1];
            MethodName = methods[methods.Length - 1];
            if (FeatureMethodName.Contains("launch"))
                this.FeatureType = FeatureType.Action;
            else if (FeatureMethodName.Contains("change"))
            {
                this.FeatureType = FeatureType.Selection;

                var index = FeatureMethodName.IndexOf("change");
                string type = FeatureMethodName.Remove(index, "change".Length);
                SubType st = SubType.BrewStrength;  //SubType.Undefined;
                Enum.TryParse(type, out st);
                this.SubType = st;
            }
        }

        public Feature(NotificationViewModel nvm)
        {
            FeatureType = nvm.FeatureType;
            FeatureId = nvm.FeatureId;
            FeatureName = nvm.FeatureId;
            MethodName = nvm.MethodName;
            FeatureMethodName = nvm.MethodName;
            CustomIcon = nvm.CustomIcon;
            Title = nvm.FeatureName;
        }


    }

    public enum FeatureType
    {
        Selection,
        Action,
        SubType,
        Undefined
    }

    public enum SubType
    {
        BrewStrength,
        Temp,
        Duration,
        WashCycle,
        WashTemp,
        OvenTemp,
        Undefined
    }
}
