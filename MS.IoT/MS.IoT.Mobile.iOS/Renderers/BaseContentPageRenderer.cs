using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using QXFUtilities;
using QXFUtilities.iOS;

[assembly: ExportRenderer(typeof(BaseContentPage), typeof(BaseContentPageRenderer))]
namespace QXFUtilities.iOS
{
    public class BaseContentPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (((BaseContentPage)Element).EnableBackButtonOverride)
            {
                SetCustomBackButton();
            }
        }


        private void SetCustomBackButton()
        {

            // Lets try this first -  may get called in addition to back, so test
            NavigationController.TopViewController.NavigationItem.LeftBarButtonItem.Clicked += (sender, e) =>
            {
                if (((BaseContentPage)Element)?.BackButtonAction != null)
                {
                    ((BaseContentPage)Element)?.BackButtonAction.Invoke();
                }
            };


/*
            // Load the Back arrow Image
            var backBtnImage = UIImage.FromBundle("iosbackchevron.png");
            backBtnImage = backBtnImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            // Create our Button and set Edge Insets for Title and Image
            var backBtn = new UIButton(UIButtonType.Custom)
            {
                HorizontalAlignment = UIControlContentHorizontalAlignment.Left,
                TitleEdgeInsets = new UIEdgeInsets(11.5f, 15f, 10f, 0f),
                ImageEdgeInsets = new UIEdgeInsets(1f, 8f, 0f, 0f)
            };

            // Set the styling for Title - can set any Text as you wish here
            backBtn.SetTitle("Back", UIControlState.Normal);
            // use the default blue color in iOS back button text
            backBtn.SetTitleColor(UIColor.White, UIControlState.Normal);
            backBtn.SetTitleColor(UIColor.LightGray, UIControlState.Highlighted);
            backBtn.Font = UIFont.FromName("HelveticaNeue", (nfloat)17);

            // Set the Image to the button
            backBtn.SetImage(backBtnImage, UIControlState.Normal);

            // Allow the button to Size itself
            backBtn.SizeToFit();

            // Add the Custom Click event you would like to execute upon the Back button click
            backBtn.TouchDown += (sender, e) =>
            {
                if (((BaseContentPage)Element)?.BackButtonAction != null)
                {
                    ((BaseContentPage)Element)?.BackButtonAction.Invoke();
                }
            };

            //Set the frame of the button
            backBtn.Frame = new CGRect(
                0,
                0,
                UIScreen.MainScreen.Bounds.Width / 4,
                NavigationController.NavigationBar.Frame.Height);

            // Add our button to a container
            var btnContainer = new UIView(new CGRect(0, 0, backBtn.Frame.Width, backBtn.Frame.Height));
            btnContainer.AddSubview(backBtn);

            // A dummy button item to push our custom  back button to
            // the edge of screen (sort of a hack)
            var fixedSpace = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace)
            {
                Width = -16f
            };
            // wrap our custom back button with a UIBarButtonItem
            var backButtonItem = new UIBarButtonItem("", UIBarButtonItemStyle.Plain, null)
            {
                CustomView = backBtn
            };

            // Add it to the ViewController
            NavigationController.TopViewController.NavigationItem.LeftBarButtonItems = new[] { fixedSpace, backButtonItem };
*/
        }
  


    }
}