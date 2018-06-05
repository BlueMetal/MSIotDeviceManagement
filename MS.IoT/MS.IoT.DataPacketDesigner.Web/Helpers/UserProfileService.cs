using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MS.IoT.DataPacketDesigner.Web.Models;
using MS.IoT.Common;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    public class UserProfileService : IUserProfileService
    {
        private string _clientId;
        private string _appKey;
        private string _aadInstance;
        private string _graphResourceID;
        private IUser _currentUser;

        public UserProfileService(string graphResourceID, string clientId, string appKey, string aadInstance)
        {
            _graphResourceID = graphResourceID;
            _clientId = clientId;
            _appKey = appKey;
            _aadInstance = aadInstance;
        }

        public async Task<IUser> GetCurrentUser()
        {
            if (_currentUser != null)
                return _currentUser;

            if (ClaimsPrincipal.Current != null)
            {
                Log.Information("Creating CurrentUser object.");
                _currentUser = new User() {
                    UserPrincipalName = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value,
                    DisplayName = ClaimsPrincipal.Current.FindFirst("name").Value,
                };
                return _currentUser;

                /*string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                try
                {
                    Uri servicePointUri = new Uri(_graphResourceID);
                    Uri serviceRoot = new Uri(servicePointUri, tenantID);
                    ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                          async () => await GetTokenForApplication());

                    // use the token for querying the graph to get the user details
                    var result = await activeDirectoryClient.Users
                        .Where(u => u.ObjectId.Equals(userObjectID))
                        .ExecuteAsync();
                    _currentUser = result.CurrentPage.ToList().First();
                    return _currentUser;
                }
                catch (AdalException e)
                {
                    //TODO: Log
                    return null;
                }
                // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
                catch (Exception e)
                {
                    //TODO: Log
                    return null;
                }*/
            }
            else
            {
                Log.Error("Current user Claim could not be found.");
                return null;
            }
        }

        private async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(_clientId, _appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            //AuthenticationContext authenticationContext = new AuthenticationContext(_aadInstance + tenantID, new ADALTokenCache(signedInUserID));
            // AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(_graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            //return authenticationResult.AccessToken;
            return null;
        }
    }
   
}