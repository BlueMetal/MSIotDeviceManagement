using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FFImageLoading.Svg;
using QXFUtilities.Navigation;

namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashScreen : ContentPage
	{
		public SplashScreen ()
		{
			InitializeComponent ();

            this.BindingContext = new SplashScreenViewModel();
		}

		public SplashScreen (NavigationSource navigationSource)
		{
			InitializeComponent ();

            this.BindingContext = new SplashScreenViewModel(navigationSource);
		}


	}
}