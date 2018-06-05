using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MS.IoT.Mobile
{
    public class DeviceFeatureTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate standardSettingDataTemplate;
        private readonly DataTemplate standardActionDataTemplate;
        private readonly DataTemplate brewStrengthDataTemplate;

        public DeviceFeatureTemplateSelector()
        {
            standardSettingDataTemplate = new DataTemplate(typeof(StandardSettingViewCell));
            standardActionDataTemplate = new DataTemplate(typeof(StandardActionViewCell));
            brewStrengthDataTemplate = new DataTemplate(typeof(BrewStrengthViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var vm = item as FeaturesViewModel;
            if (vm == null)
                return null;

            switch (vm.FeatureType)
            {
                case (FeatureType.Selection):
                    return standardSettingDataTemplate;

                case (FeatureType.Action):
                    return standardActionDataTemplate;

                case (FeatureType.SubType):
                    switch (vm.SubType)
                    {
                        case (SubType.BrewStrength):
                            return brewStrengthDataTemplate;

                        default:
                            return null;

                    }

                default:
                    return null;

            }
        }


    }

}
