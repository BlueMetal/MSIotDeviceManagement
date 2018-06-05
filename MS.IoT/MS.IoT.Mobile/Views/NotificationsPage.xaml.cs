using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using QXFUtilities;


namespace MS.IoT.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationsPage : BaseContentPage
    {
		public NotificationsPage ()
		{
			InitializeComponent ();

            // Set the back icon
/*            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetTitleIcon(this, new FileImageSource() { File = "blank.png" });
            }
*/

            this.EnableBackButtonOverride = false;  //true;
            // ToDo Implement custom action so either used default back or navigates to MainPage.
 //           this.BackButtonAction = CustomAction;


            // set the associated view model as the binding context, so as to bind the values in XAML
            this.BindingContext = new NotificationsPageViewModel(Navigation);

        }

        private void OnItemTapped(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null)
                lv.SelectedItem = null;
        }

	}
}