using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

using QXFUtilities;

using UWPFrame = Windows.UI.Xaml.Controls.Frame;


using UWPFram = Windows.UI.Xaml.Controls.Frame;

[assembly: ExportRenderer(typeof(QXFUtilities.BaseContentPage), typeof(QXFUtilities.UWP.BaseContentPageRenderer))]
namespace QXFUtilities.UWP
{
    public class BaseContentPageRenderer : PageRenderer
    {
        private SystemNavigationManager _navigationManager = null;
        private SystemNavigationManager NavigationManager
        {
            get
            {
                if (_navigationManager == null)
                {
                    _navigationManager = SystemNavigationManager.GetForCurrentView();
                }
                return _navigationManager;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);

            if (((BaseContentPage)this.Element).EnableBackButtonOverride)
            {
                SetCustomBackButton();
            }

        }

        private void SetCustomBackButton()
        {
            NavigationManager.BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var currentPage = (BaseContentPage)this.Element;
            if (currentPage.BackButtonAction != null)
            {
                e.Handled = true;
                // Invoke the back button action
                currentPage.BackButtonAction.Invoke();
            }
            else
            {
                // Invoke the back button action
                UWPFrame rootFrame = Window.Current.Content as UWPFrame;
                if (rootFrame.CanGoBack)
                {
                    e.Handled = true;
                    rootFrame.GoBack();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            NavigationManager.BackRequested -= OnBackRequested;
            _navigationManager = null;
            base.Dispose(disposing);
        }
    }

}
