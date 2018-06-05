using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FFImageLoading.Svg;

namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TwoButtonNotificationViewCell : ViewCell
	{
		public TwoButtonNotificationViewCell ()
		{
			InitializeComponent ();
		}

        /*
        void OnButtonTapped(object sender, EventArgs args)
        {
            var image = sender as FFImageLoading.Svg.Forms.SvgCachedImage;
            var viewCell = image.Parent.Parent as ViewCell;
            viewCell.ForceUpdateSize();
        }
        */

    }
}