using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using QXFUtilities.Navigation;
using Xamarin.Forms;

namespace MS.IoT.Mobile
{

    /// <summary>
    /// Manages navigation decisions on behalf of view models (and even views).
    /// Allows view models to delegate navigation, removing the need to have any knowledge
    /// of navigation.
    /// Collects all navigation decisions/models into one place to assist maintenance.
    /// Expresses navigation as a state machine
    /// 
    /// </summary>


    public class NavigationManager : NavigationService
    {
        // Pages
        private const string SplashScreen = "SplashScreen";
        private const string SplashScreenViewModel = "SplashScreenViewModel";
        private const string MainPage = "MainPage";
        private const string MainPageViewModel = "MainPageViewModel";
        private const string NotificationsPage = "NotificationsPage";
        private const string NotificationsPageViewModel = "NotificationsPageViewModel";

        private const string NotificationsPageInsertMainPage = "NotificationsPageInserMainPage";

        private const string SystemNotifications = "SystemNotifications";

        // Events
        public const string AuthenticationSuccess = "AuthenticationSuccess";
        public const string AuthenticationFailure = "AuthenticationFailure";
        public const string ClickNotification = "ClickNotification";

        // Buttons
        public const string BackButton = "BackButton";
        public const string NotificationsButton = "NotificationsButton";
        public const string SelectFeature = "SelectFeature";
        public const string SelectNotification = "SelectNotification";

        // actions
        public const string Back = "Back";


        /// <summary>
        /// Uses states machine to determine how to navigate
        /// </summary>
        /// <param name="currentPageName"></param>
        /// <param name="navigationTrigger"></param>
        /// <param name="navigationSource"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>

        public static async Task Navigate(INavigation navigation, string currentPageOrViewModelName, string navigationTrigger, NavigationSource navigationSource = NavigationSource.Normal, Dictionary<string, string> parameters = null)
        {
            if (navigationSource == NavigationSource.FromNotification)
                // Navigation from a Notification
                await Navigate(navigation, NotificationNavigationStateMachine, currentPageOrViewModelName, navigationTrigger, parameters);
            else
                // Normal in app navigation
                await Navigate(navigation, StandardNavigationStateMachine, currentPageOrViewModelName, navigationTrigger, parameters);
        }


        private static async Task Navigate(INavigation navigation, Dictionary<string, Dictionary<string, string>> stateMachine, string currentPageOrViewModelName, string navigationTrigger, Dictionary<string, string> parameters = null)
        {
            Dictionary<string, string> navigationOptions = null;
            if (stateMachine.TryGetValue(currentPageOrViewModelName, out navigationOptions))
            {
                // Found the page
                string targetPage = null;
                if (navigationOptions.TryGetValue(navigationTrigger, out targetPage))
                    //Found trigger - navigate to page
                    if (navigationTrigger == AuthenticationSuccess)
                        await NavigateToPage(navigation, targetPage, true);
                    else
                        await NavigateToPage(navigation, targetPage);
                //Didn't find the trigger - Do nothing               
            }
            // Didn't find the page - Do nothing
        }


        private static async Task NavigateToPage(INavigation navigation, string page, bool newNavigation = false)
        {
            switch (page)
            {
                case (Back):
                    // go back in the back stack
                    await navigation.PopAsync();
                    break;
                case (MainPage):
                    // got to Main Page
                    if (newNavigation)
                        NavigateToNewRootPage(new MainPage(), GlobalResources.Colors.NavBarColor, Color.White);
                    else
                        await PushAsync(navigation, new MainPage());
                    break;
                case (NotificationsPage):
                    // got to Notifications Page
                    await PushAsync(navigation, new NotificationsPage());
                    break;
                case (NotificationsPageInsertMainPage):
                    // got to Notifications Page
                    await PushAsync(navigation, new NotificationsPage());
                    // insert main page into Navigation stack before the notification page just pushed
                    navigation.InsertPageBefore(new MainPage(), navigation.NavigationStack[navigation.NavigationStack.Count - 1]);
                    break;
            }
            // Invalid navigation request - don't do anything
        }





        // Notification normal Source
        // Navigation comes as a result of the user reacting to a notification

        private static Dictionary<string, Dictionary<string, string>> StandardNavigationStateMachine
        {
            get
            {
                return new Dictionary<string, Dictionary<string, string>>
                                        {
                                            // From SplashScreen
                                            {SplashScreen, FromSplashScreen },
                                            {SplashScreenViewModel, FromSplashScreen },
                                            // From MainPage
                                            {MainPage, FromMainPage },
                                            {MainPageViewModel, FromMainPage },
                                            // From Notification Page
                                            {NotificationsPage, FromNotificationsPage },
                                            {NotificationsPageViewModel, FromNotificationsPage }
                                        };
            }
        }



        private static Dictionary<string, string> FromSplashScreen{
            get
            {
                return new Dictionary<string, string>
                                        {
                                            {AuthenticationSuccess, MainPage},
                                            {AuthenticationFailure, SplashScreen}

                                        };
            }
        }

        private static Dictionary<string, string> FromMainPage{
            get
            {
                return new Dictionary<string, string>
                                        {
                                            {SelectFeature, MainPage},
                                            {BackButton, Back},
                                            { NotificationsButton, NotificationsPage}

                                        };
            }
        }

        private static Dictionary<string, string> FromNotificationsPage{
            get
            {
                return new Dictionary<string, string>
                                        {
                                            {SelectNotification, NotificationsPage},
                                            {BackButton, Back}
                                        };
            }
        }




        // Notification navigation Source
        // Navigation comes as a result of the user reacting to a notification
        private static Dictionary<string, Dictionary<string, string>> NotificationNavigationStateMachine = new Dictionary<string, Dictionary<string, string>>
        {
            // From SystemNotification
            {SystemNotifications, FromSystemNotification },
            // From SplashScreen
            {SplashScreen, FromSplashScreen_Notification },
            {SplashScreenViewModel, FromSplashScreen_Notification }
        };


        private static readonly Dictionary<string, string> FromSplashScreen_Notification = new Dictionary<string, string>
        {
            {AuthenticationSuccess, NotificationsPageInsertMainPage},
            {AuthenticationFailure, SplashScreen}
        }; 

        private static readonly Dictionary<string, string> FromSystemNotification = new Dictionary<string, string>
        {
            {ClickNotification, NotificationsPageInsertMainPage}
        };


    }
}
