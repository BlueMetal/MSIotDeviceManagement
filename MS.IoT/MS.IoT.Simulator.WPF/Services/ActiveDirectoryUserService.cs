using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.Common;
using MS.IoT.Domain.Model;
using MS.IoT.Simulator.WPF.Helpers;
using MS.IoT.Simulator.WPF.Models;
using MS.IoT.Simulator.WPF.Services.Interfaces;
using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;

namespace MS.IoT.Simulator.WPF.Services
{
    public class ActiveDirectoryUserService : IUserService, IDisposable
    {
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        Uri redirectUri = new Uri(ConfigurationManager.AppSettings["ida:RedirectUri"]);
        private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static string graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];            
        private AuthenticationContext authContext = null;

        private User currentUser;

        public delegate void ConnectionStatusEventHandler(Object sender, RelayItemEventArgs<AzureConnectionObject> e);
        public event ConnectionStatusEventHandler ConnectionStatusChanged;

        public ActiveDirectoryUserService()
        {
            authContext = new AuthenticationContext(authority, new FileCache());
        }

        public async Task<AzureConnectionStatus> SignIn()
        {
            // Get an Access Token for the Graph API
            AuthenticationResult result = null;
            try
            {
                OnConnectionStatusChanged(AzureConnectionStatus.NotConnected, string.Empty, null);
                result = await authContext.AcquireTokenAsync(graphResourceId, clientId, redirectUri, new PlatformParameters(PromptBehavior.Auto));
                currentUser = new User()
                {
                    Id = result.UserInfo.DisplayableId,
                    Name = string.Format("{0} {1}", result.UserInfo.GivenName, result.UserInfo.FamilyName)
                };
                OnConnectionStatusChanged(AzureConnectionStatus.Authenticated, string.Empty, currentUser);
                return AzureConnectionStatus.Authenticated;
            }
            catch (AdalException ex)
            {
                // An unexpected error occurred, or user canceled the sign in.
                if (ex.ErrorCode == "access_denied")
                {
                    Log.Information(ex, "Access denied.");
                    OnConnectionStatusChanged(AzureConnectionStatus.AccessDenied, ex.Message, null);
                    return AzureConnectionStatus.AccessDenied;
                }
                OnConnectionStatusChanged(AzureConnectionStatus.UnknownError, ex.Message, null);
                return AzureConnectionStatus.UnknownError;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown error.");
                OnConnectionStatusChanged(AzureConnectionStatus.UnknownError, ex.Message, null);
                return AzureConnectionStatus.UnknownError;
            }
        }

        public AzureConnectionStatus SignOut()
        {
            authContext.TokenCache.Clear();
            OnConnectionStatusChanged(AzureConnectionStatus.NotConnected, string.Empty, null);
            return AzureConnectionStatus.NotConnected;
        }

        /*private async void CheckForCachedToken()
        {
            // As the application starts, try to get an access token without prompting the user.  If one exists, show the user as signed in.
            AuthenticationResult result = null;
            try
            {
                result = await authContext.AcquireTokenAsync(graphResourceId, clientId, redirectUri, new PlatformParameters(PromptBehavior.Never));
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode != "user_interaction_required")
                    OnConnectionStatusChanged(ConnectionStatus.UserInteractionRequired, ex.Message, null);
                else
                    OnConnectionStatusChanged(ConnectionStatus.UnknownError, ex.Message, null);
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(ConnectionStatus.UnknownError, ex.Message, null);
            }
        }*/

        public User GetCurrentUser()
        {
            return currentUser;
        }

        public void OnConnectionStatusChanged(AzureConnectionStatus connectionStatus, string connectionErrorMessage, User user)
        {
            App.Current.Dispatcher.Invoke(new Action(() => {
                ConnectionStatusChanged?.Invoke(this, new RelayItemEventArgs<AzureConnectionObject>(new AzureConnectionObject()
                {
                    ConnectionStatus = connectionStatus,
                    User = user,
                    ErrorMessage = connectionErrorMessage
                }));
            }));
            
        }

        /// <summary>
        /// Method to be used when the application exits
        /// </summary>
        public void Dispose()
        {
        }
    }
}
