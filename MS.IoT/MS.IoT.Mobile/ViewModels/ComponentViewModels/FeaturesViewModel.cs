using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text;
using System.Threading;
using Xamarin.Forms;

using QXFUtilities.Communication;
using QXFUtilities;

namespace MS.IoT.Mobile
{
    public class FeaturesViewModel : BaseViewModel
    {
        public int Index { get; set; }
        public FeatureType FeatureType { get; set; }
        public SubType SubType { get; set; }
        public int ParentIndex { get; set; } = -1;
        public int NumberSubTypeChildren {get; set;} = 0;
        public ImageSource Icon { get; set; }
        public string Label { get; set; }

        public object SelectedValue { get; set; }
        public Type SelectedValueType { get; set; } = typeof(int);


        public string FeatureId  { get; set; }
        public string FeatureName  { get; set; }
        public string FeatureMethodName  { get; set; }
        public string FeatureMethodParameter { get; set; } = "null";
        public HttpRequestType ActionType { get; set; } = HttpRequestType.Put;
        public Dictionary<string, object> Parameters { get; set; }


        private bool _expanded = false;
        public bool Expanded
        {
            get { return _expanded; }
            set {
                _expanded= value;
                OnPropertyChanged<bool>();
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy= value;
                OnPropertyChanged<bool>();
                if(value)
                    ButtonColorTransform = StopButtonTransform;
                else
                    ButtonColorTransform = StartButtonTransform;
            }
        }




        private static readonly List<FFImageLoading.Work.ITransformation> StopButtonTransform
            = new List<FFImageLoading.Work.ITransformation> { new FFImageLoading.Transformations.TintTransformation { HexColor = GlobalResources.Colors.StopButtonColorHex, EnableSolidColor = true } };
        private static readonly List<FFImageLoading.Work.ITransformation> StartButtonTransform
            = new List<FFImageLoading.Work.ITransformation> { new FFImageLoading.Transformations.TintTransformation { HexColor = GlobalResources.Colors.StartButtonColorHex, EnableSolidColor = true } };


        List<FFImageLoading.Work.ITransformation> _buttonColorTransform = StartButtonTransform;
        public List<FFImageLoading.Work.ITransformation> ButtonColorTransform
        {
            get { return _buttonColorTransform; }
            set
            {
                _buttonColorTransform = value;
                OnPropertyChanged<List<FFImageLoading.Work.ITransformation>>();
            }
        }


        public Timer Timer { get; set; } = null;


        public ICommand SelectionOptionTapped { get; set; }

        public FeaturesViewModel()
        {
            SelectionOptionTapped = new Command(OnSelectionOptionTapped);
        }

        public FeaturesViewModel(Feature feature, int index)
        {
            SelectionOptionTapped = new Command(OnSelectionOptionTapped);

            Index = index;
            FeatureType = feature.FeatureType;
            SubType = feature.SubType;
            Label = feature.Title;
            FeatureId = feature.FeatureId;
            FeatureName = feature.FeatureName;
            FeatureMethodName = feature.FeatureMethodName;
            FeatureMethodParameter = feature.FeatureMethodParameter;
            Icon = GetIcon(feature.FeatureName);
        }

        public FeaturesViewModel(Feature feature, int index, BaseViewModel parentViewModel)
        {
            SelectionOptionTapped = new Command(OnSelectionOptionTapped);

            Index = index;
            FeatureType = feature.FeatureType;
            SubType = feature.SubType;
            Label = feature.Title;
            FeatureId = feature.FeatureId;
            FeatureName = feature.FeatureName;
            FeatureMethodName = feature.FeatureMethodName;
            FeatureMethodParameter = feature.FeatureMethodParameter;
            Icon = GetIcon(feature.FeatureName);
            ParentViewModel = parentViewModel;
        }


        private ImageSource GetIcon(string name)
        {
            // Currently hard coded
            if (name.Contains("brewStrength"))
                return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-coffee-strength.svg");
            if (name.Contains("grindAndBrew"))
                return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-grind-and-brew.svg");
            if (name.Contains("brew"))
                return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-brew.svg");
            if (name.Contains("grind"))
                return FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.icon-grind.svg");
            //Add others here for dummy feature

            return null;
        }


        public static List<FeaturesViewModel> GetFeaturesViewModels(List<Feature> features, BaseViewModel parentViewModel = null)
        {
            List<FeaturesViewModel> list = new List<FeaturesViewModel>(features?.Count ?? 0);
            int index = 0;
            foreach (Feature feature in features ?? new List<Feature>(0) )
            {
                var fvm = new FeaturesViewModel(feature, index, parentViewModel);

                // Add in the vm for the selection option entry
                if (feature.FeatureType == FeatureType.Selection)
                {

                    // TODO:
                    // NOTE: THE CURRENT DATA MODEL FROM THE SERVER DOES NOT SUPPORT ADDING THE SELECTION
                    // CRITERIA IN AN ABSTRACT FASHION. IT WOULD NEED TO PASS THE PARAMETER TYPE(S), THE RANGE OR DESCRETE VALUES, 
                    // HOW MANY SUB-SELECTIONS, AND THE SELECTION CONTROL TYPE.
                    // AS SUCH CURRENTLY ONLT HARD CODED TYPES AND HARD CODED SELECTION CRITERIA CAN BE SUPPORTED. NEW TYPES WILL
                    // REQUIRE AN UPDATE TO THE APP

                    // Check if a DUMMY feature in which case don't add anything - could add in but not at moment
                    if (feature.FeatureName.Contains("Dummy"))
                        // Add to list but not any sub features
                        list.Add(fvm);
                    else
                    {
                        //set the number of sub features
                        fvm.NumberSubTypeChildren = 1;
                        // Add to list
                        list.Add(fvm);

                        index++;
                        //create new view model and add to list - hard coded (See above)
                        // only brew strength currently defined
                        if (feature.SubType == SubType.BrewStrength)
                        {
                            var vm = new FeaturesViewModel { Index = index, FeatureType = FeatureType.SubType, SubType = SubType.BrewStrength, ParentIndex = index - 1, FeatureName = "brewStrengthFeature", FeatureMethodName = "changeBrewStrength" };
                            list.Add(vm);
                        }
                    }
                }
                else
                    // Add to list
                    list.Add(fvm);
                index++;
            }
            return list;
        }


        public static List<List<FeaturesViewModel>> GetFeaturesViewModelsList(List<Device> devices, BaseViewModel parentViewModel = null)
        {
            List<List<FeaturesViewModel>> list = new List<List<FeaturesViewModel>>(devices?.Count ?? 0);
            foreach (Device device in devices ?? new List<Device>(0))
            {
                list.Add(GetFeaturesViewModels(device.Features, parentViewModel));               
            }
            return list;
        }




        // Selection Options have to be passed to the parent view model explicitly due to a bug
        // in Xamarin.Forms that doesn't resolve referenced BindingContexts from within a DataTemplate
        // unless it is declared in the parent View.
        private void OnSelectionOptionTapped(object value)
        {
            SelectedValue = value;
            // Convert the object to the parameter type and store in parent view model
            //var val = Convert.ChangeType(value, SelectedValueType);
            //Tuple<object, Type> tupleValue = new Tuple<object, Type>(value, SelectedValueType);
            MainPageViewModel.AddOrUpdateSelection(ParentIndex, value, SelectedValueType);
        }

    }





}
