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
	public partial class StandardNotificationViewCell : ViewCell
	{
		public StandardNotificationViewCell ()
		{
			InitializeComponent ();
		}
	}
}