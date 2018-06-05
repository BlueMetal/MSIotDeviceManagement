using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using Xamarin.Forms;

using QXFUtilities;



namespace MS.IoT.Mobile
{
    public class DeviceViewModel : BaseViewModel
    {

        public ImageSource Icon { get; set; }

        public string Caption { get; set; }

        public string DeviceType { get; set; }

        public string DeviceId { get; set; }

        private double _iconScale  = 0.7;
        public double IconScale
        {
            get { return _iconScale; }
            set
            {
                _iconScale = value;
                OnPropertyChanged<double>();
            }
        }


        public List<Feature> _deviceFeatures = new List<Feature>();
        // Features
        public List<Feature> DeviceFeatures
        {
            get { return _deviceFeatures; }
            set
            {
                _deviceFeatures = value;
                OnPropertyChanged<List<Feature>>();
            }
        }

        public int IndexOfFirstActionFeature
        {
            get
            {
                var firstActionFeature = _deviceFeatures.Where(f => f.FeatureType == FeatureType.Action).FirstOrDefault();
                return firstActionFeature == null ? _deviceFeatures.Count : _deviceFeatures.IndexOf(firstActionFeature); 
            }
        }
        public void AddFeature(Feature feature)
        {
            _deviceFeatures.Add(feature);
            OnPropertyChanged<List<Feature>>(nameof(DeviceFeatures));
        }
        public void InsertFeatureAt(int index, Feature feature)
        {
            if (index < _deviceFeatures.Count - 1)
                _deviceFeatures.Insert(index, feature);
            else
                _deviceFeatures.Add(feature);
            OnPropertyChanged<List<Feature>>(nameof(DeviceFeatures));
        }
        public void AddFeatureByType(Feature feature)
        {
            // Selection features should come at top and require an additional feature to be added
            if (feature.FeatureType == FeatureType.Selection)
            {
                // insert the selection feature above the first action
                int index = IndexOfFirstActionFeature;
                _deviceFeatures.Insert(index, feature);
                // TODO:
                // NOTE: THE CURRENT DATA MODEL FROM THE SERVER DOES NOT SUPPORT ADDING THE SELECTION
                // CRITERIA IN AN ABSTRACT FASHION. IT WOULD NEED TO PASS THE PARAMETER TYPE(S), THE RANGE OR DESCRETE VALUES; 
                // AND THE SELECTION CONTROL TYPE.
                // AS SUCH CURRENTLY ONLT HARD CODED TYPES AND HARD CODED SELECTION CRITERIA CAN BE SUPPORTED. NEW TYPES WILL
                // REQUIRE AN UPDATE TO THE APP
            }
            else
            {
                _deviceFeatures.Add(feature);
            }
            OnPropertyChanged<List<Feature>>(nameof(DeviceFeatures));
        }
        private void addFeaturesByType(List<Feature> features)
        {
            if (features != null)
            {
                foreach (Feature feature in features)
                {
                    if (_deviceFeatures.Count == 0)
                    {
                        _deviceFeatures.Add(feature);
                    }
                    else
                    {
                        // Selection features should come at top and require an additional feature to be added
                        if (feature.FeatureType == FeatureType.Selection)
                        {
                            _deviceFeatures.Insert(IndexOfFirstActionFeature, feature);
                            // TODO:
                            // NOTE: THE CURRENT DATA MODEL FROM THE SERVER DOES NOT SUPPORT ADDING THE SELECTION
                            // CRITERIA IN AN ABSTRACT FASHION. IT WOULD NEED TO PASS THE PARAMETER TYPE(S), THE RANGE OR DESCRETE VALUES; 
                            // AND THE SELECTION CONTROL TYPE.
                            // AS SUCH CURRENTLY ONLT HARD CODED TYPES AND HARD CODED SELECTION CRITERIA CAN BE SUPPORTED. NEW TYPES WILL
                            // REQUIRE AN UPDATE TO THE APP
                        }
                        else
                        {
                            _deviceFeatures.Add(feature);
                        }
                    }
                }
            }
        }
        public void AddFeaturesByType(List<Feature> features)
        {
            addFeaturesByType(features);
            OnPropertyChanged<List<Feature>>(nameof(DeviceFeatures));
        }


        public DeviceViewModel() { }

        public DeviceViewModel(Device device)
        {
            DeviceType = GetDeviceType(device.DeviceType, device.DeviceName);
            Caption = device.DeviceName; // GetCaption(DeviceType);
            DeviceId = device.DeviceId;
            Icon = GetIcon(DeviceType);
            addFeaturesByType(device.Features);
        }






        public static List<DeviceViewModel> GetViewModels (List<Device> devices)
        {
            List<DeviceViewModel> viewModels = new List<DeviceViewModel>();
            foreach (Device device in devices)
            {
                var vm = new DeviceViewModel(device);
                viewModels.Add(vm);
            }
            return viewModels;
        }

        private string GetDeviceType(string devType, string devName)
        {
            switch (devType)
            {
                case ("coffeemaker") :
                    return "CoffeeMaker";
                case ("washingmachine"):
                    return "WashingMachine";
                case ("dishwasher"):
                    return "Dishwasher";  
                case ("stove"):
                    return "Stove";                  
                case ("refrigerator"):
                    return "Refrigerator";
                case ("microwave"):
                    return "Microwave";  
                case ("furnace"):
                    return "Furnace";
                default:
                    break;
            }

            if (devName.Contains("Coffee Maker"))
                return "CoffeeMaker";
            if (devName.Contains("Washing Machine"))
                return "WashingMachine";   
            if (devName.Contains("Dishwasher"))
                return "Dishwasher";              
            if (devName.Contains("Stove"))
                return "Stove";
            if (devName.Contains("Refrigerator"))
                return "Refrigerator";
            if (devName.Contains("Microwave"))
                return "Microwave";
            if (devName.Contains("Furnace"))
                return "Furnace";
            return "CoffeeMaker";  // for demo
        }
        private string GetCaption(string deviceType)
        {
            switch (deviceType)
            {
                case "CoffeeMaker":
                    return "My Coffee Maker";

                case "WashingMachine":
                    return "My Washer"; 

                case "Dishwasher":
                    return "My Dishwasher"; 

                case "Stove":
                    return "My Stove";

                case "Refrigerator":
                    return "My Refrigerator";

                case "Microwave":
                    return "My Microwave";

                case "Furnace":
                    return "My Furnace";

                default:
                    return deviceType;
            }
        }
        private ImageSource GetIcon(string deviceType)
        {
            switch (deviceType)
            {
                case "CoffeeMaker":
                    return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.apps-coffeemaker.svg");

                case "WashingMachine":
                    return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.apps-washer.svg");

                case "Stove":
                    return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.apps-stove.svg");

                case "Dishwasher":
                    return ImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.dishwasher.png");

                case "Refrigerator":
                    return ImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.toaster.png");
/*
                case "Microwave":
                    return

                case "Furnace":
                    return
*/
                default:
                    return ImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon.png");
            }
        }


    }
}
