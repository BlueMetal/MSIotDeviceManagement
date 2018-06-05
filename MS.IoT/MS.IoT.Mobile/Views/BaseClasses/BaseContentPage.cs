using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QXFUtilities
{
    public class BaseContentPage : ContentPage
    {
        /// <summary>
        /// Gets or Sets the Back button click overridden custom action
        /// </summary>
        public Action BackButtonAction { get; set; }

        /// <summary>
        /// Gets or Sets Custom Back button overriding state
        /// </summary>
        public static readonly BindableProperty EnableBackButtonOverrideProperty = BindableProperty.Create(nameof(EnableBackButtonOverride), typeof(bool), typeof(BaseContentPage), false);
        public bool EnableBackButtonOverride
        {
            get { return (bool)GetValue(EnableBackButtonOverrideProperty); }
            set { SetValue(EnableBackButtonOverrideProperty, value); }
        }

        public BaseContentPage() : base()
        {
            // Set up the Navigation Bar features associated with this page - has to be done programatically - not available in XAML
            /*
                // Remove the back button
                NavigationPage.SetHasBackButton (this, false);
			*/
            // For Android, set the icon to be a blank icon (transparent 1px). Note: on ioS setting this replaces the page title
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetTitleIcon(this, new FileImageSource() { File = "blank.png" });
            }
        }
        public BaseContentPage(bool removeBackButton) : base()
        {
            // Set up the Navigation Bar features associated with this page - has to be done programatically - not available in XAML
            if (removeBackButton)
            {
                // Remove the back button
                NavigationPage.SetHasBackButton(this, false);
            }

            // For Android, set the icon to be a blank icon (transparent 1px). Note: on ioS setting this replaces the page title
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetTitleIcon(this, new FileImageSource() { File = "blank.png" });
            }
        }


        protected virtual void OnRowSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // if an item was selected, pass to the view model
            if (e.SelectedItem != null)
            {
                // Pass to the View Model
                ((BaseViewModel)this.BindingContext).OnRowSelected(sender, e.SelectedItem);
                if (sender is ListView)
                {
                    ((ListView)sender).SelectedItem = null;   // This resets the selected Item
                }
            }
        }

    }
}
