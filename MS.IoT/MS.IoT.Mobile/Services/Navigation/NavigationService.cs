using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace QXFUtilities.Navigation
{
    /// <summary>
    /// Helper navigation service to use so we don't push multiple pages at the same time.
    /// Provide Push abstractions that monitor whether the app is already in the process of navigating.
    /// This prevents the reported possibility that in some circumstances, he user or a process can trigger
    /// additional navigations before the first is completed.
    /// </summary>
    public class NavigationService
    {
        private static bool navigating;

        /// <summary>
        /// PUsh a page async
        /// </summary>
        /// <returns>awaitable task.</returns>
        /// <param name="navigation">Navigation.</param>
        /// <param name="page">Page.</param>
        /// <param name="animate">If set to <c>true</c> animate.</param>
        public static async Task PushAsync(INavigation navigation, Page page, bool animate = true)
        {
            if (navigating)
                return;

            navigating = true;
            await navigation.PushAsync(page, animate);
            navigating = false;
        }

        /// <summary>
        /// Push a page modal async
        /// </summary>
        /// <returns>awaitable task.</returns>
        /// <param name="navigation">Navigation.</param>
        /// <param name="page">Page.</param>
        /// <param name="animate">If set to <c>true</c> animate.</param>
        public static async Task PushModalAsync(INavigation navigation, Page page, bool animate = true)
        {
            if (navigating)
                return;

            navigating = true;
            await navigation.PushModalAsync(page, animate);
            navigating = false;
        }


        public static void NavigateToNewRootPage(NavigationPage page)
        {
            // Wrap in Navigation page to implement navigation/action tool bar
            Application.Current.MainPage = page;
        }

        public static void NavigateToNewRootPage(Page page, Color toolbarColor, Color toolbarTextColor)
        {
            // Wrap in Navigation page to implement navigation/action tool bar
            Application.Current.MainPage = new NavigationPage(page)
            {
                BarBackgroundColor = toolbarColor, //GlobalResources.Colors.NavBarColor,
                BarTextColor = toolbarTextColor //Color.White,
            };
        }


    }

    public enum NavigationSource
    {
        Normal,
        FromNotification
    }
}
