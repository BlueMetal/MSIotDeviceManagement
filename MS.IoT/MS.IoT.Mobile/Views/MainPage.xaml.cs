using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.ObjectModel;

using Microsoft.Identity.Client;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json.Linq;
using Syncfusion.SfCarousel;
using Syncfusion.SfCarousel.XForms;

using FFImageLoading;
using FFImageLoading.Svg;

using QXFUtilities;
using MS.IoT.Mobile.Services.Authentication;


namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : BaseContentPage
	{
        public MainPage ()
		{
			InitializeComponent ();

            this.EnableBackButtonOverride = false;  //true;
            this.BackButtonAction = CustomAction;


            // set the associated view model as the binding context, so as to bind the values in XAML
            this.BindingContext = new MainPageViewModel(Navigation);


            // NOTE: use for debugging to check that Embedded assets have loaded, not in released app code!
            /*
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
            */


        }

        // Use code behind to control expanding and collapsing items. Needs to be moved to ViewModel
        public void OnFeatureTapped(object sender, ItemTappedEventArgs e)
        {
            var feature = e.Item as FeaturesViewModel;
            var vm = this.BindingContext as MainPageViewModel;
            if (feature.FeatureType == FeatureType.Selection && feature.NumberSubTypeChildren > 0)
            {
                if (feature.Expanded)
                {
                    // Already expanded, so collapse - remove the sub feature entries from collection  
                    for (int indx = feature.Index + 1; indx <= feature.Index + feature.NumberSubTypeChildren; indx++)
                    {
                        vm.FullFeatureCollection[vm.CurrentDeviceSelection][indx].Expanded = false;
                        vm.FeatureCollection.RemoveAt(indx);
                    }
                }
                else
                {
                    // Not expanded so expand - add in the selection sub feature entries into the observable collection
                    for (int indx = feature.Index + 1; indx <= feature.Index + feature.NumberSubTypeChildren; indx++)
                    {
                        vm.FullFeatureCollection[vm.CurrentDeviceSelection][indx].Expanded = true;
                        vm.FeatureCollection.Insert(indx, vm.FullFeatureCollection[vm.CurrentDeviceSelection][indx]);
                    }
                }
                feature.Expanded = !feature.Expanded;
            }

        }


        private void CustomAction()
        {
            bool test = true;
        }

	}
}