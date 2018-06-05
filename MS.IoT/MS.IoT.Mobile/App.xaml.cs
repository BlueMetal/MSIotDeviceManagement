
using Xamarin.Forms;
using Microsoft.Identity.Client;

using MS.IoT.Mobile.Services.Authentication;
using QXFUtilities.Navigation;

namespace MS.IoT.Mobile
{


    public partial class App : Application
    {




        public static App Current;

        public static AuthenticationService AuthenticationService = null;


        public static NavigationPage CurrentNavigationPage { get; set; }

        public static bool CurrentPageIsModal { get; set; } = false;


        public static INavigation CurrentNavigator { get; set; } = null;




        public App(NavigationSource navigationSource = NavigationSource.Normal)
        {
            Current = this;
            InitializeComponent();

            // Create Authentication Service
            AuthenticationService = new AuthenticationService(AuthenticationSettings.Tenant, AuthenticationSettings.ClientID, AuthenticationSettings.AuthorityPolicy);
            // Add Policies to Authentication Service
            AuthenticationService.AddNamedPolicy(nameof(AuthenticationSettings.PolicySignUpSignIn), AuthenticationSettings.PolicySignUpSignIn);
            //AuthenticationService.AddNamedPolicy(nameof(AuthenticationSettings.PolicyEditProfile), AuthenticationSettings.PolicyEditProfile);
            //AuthenticationService.AddNamedPolicy(nameof(AuthenticationSettings.PolicyResetPassword), AuthenticationSettings.PolicyResetPassword);
            // Add scopes
            AuthenticationService.AddNamedScope(nameof(AuthenticationSettings.ReadScope), AuthenticationSettings.ReadScope);
            AuthenticationService.AddNamedScope(nameof(AuthenticationSettings.WriteScope), AuthenticationSettings.WriteScope);


            MainPage = new SplashScreen(navigationSource);


        }


        protected override void OnStart()
        {
            // Handle when your App starts

        }

        protected override void OnSleep()
        {
            // Handle when your App sleeps
        }

        protected override void OnResume()
        {
            // Handle when your App resumes
        }




    }
}