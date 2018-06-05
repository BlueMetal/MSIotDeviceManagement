using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;

namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StandardActionViewCell : ViewCell
	{

		public StandardActionViewCell ()
		{
			InitializeComponent ();
            // Note: Will replace this with a Button when replacing with fully supported button  - to get ripple effects etc.
            // ActionButton.Source = FFImageLoading.Svg.Forms.SvgImageSource.FromResource("MS.IoT.Mobile.Assets.Icons.btn-start.svg");

        }
    }
}