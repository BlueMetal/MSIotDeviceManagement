using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StandardSettingViewCell : ViewCell
	{
		public StandardSettingViewCell ()
		{
			InitializeComponent ();

            //Indicator.Source = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-chevron-right.svg");
        }
	}
}