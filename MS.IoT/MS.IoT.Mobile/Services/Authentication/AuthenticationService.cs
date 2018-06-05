using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MS.IoT.Mobile.Services.Authentication
{





    // AD B2C Authentication Service using MSAL

    public class AuthenticationService : IAuthenticationService
    {


        public static UIParent UiParent { get; set; } = null;  // Needed by Android 

        public static string B2CTenant { get; set; } = string.Empty;
        public static string ClientId { get; set; } = string.Empty;

        private static string _authenticatePolicy = null;
        public string AuthenticatePolicy
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_authenticatePolicy))
                {
                    _authenticatePolicy = Policies?.First();
                }
                return _authenticatePolicy;
            }
            set
            {
                _authenticatePolicy = value;
            }
        }

        public static string BaseAuthority { get; set; } = $"https://login.microsoftonline.com/tfp/";


        public static string AuthorityBase
        {
            get
            {
                return $"{BaseAuthority}" + $"{B2CTenant}" + $"/";
            }
        }

        public static string Authority
        {
            get
            {
                return $"{AuthorityBase}" + _authenticatePolicy;
            }
        }

        public static string RedirectUri { get; set; } = string.Empty;
        public string ResourceUri { get; set; } = DefaultAuthenticationSettings.ResourceUri;





        private static PublicClientApplication _publicClientApplication = null;
        public PublicClientApplication PublicClientApplication
        {
            get
            {
                if (_publicClientApplication == null)
                {
                    // default redirectURI; each platform specific project will have to override it with its own
                    _publicClientApplication = new PublicClientApplication(ClientId, Authority);
                    _publicClientApplication.RedirectUri = "msal" + $"{ClientId}" + $"://auth";
                }
                return _publicClientApplication;
            }
            set
            {
                _publicClientApplication = value;
            }
        }



        // Policies - Don't have to use the Policy properties, can just use the policy directly. May replace with a Policy class later
        public static List<string> Policies { get; } = new List<string>();

        private static Dictionary<string, string> _namedPolicies = new Dictionary<string, string>();
        public Dictionary<string, string> NamedPolicies { get { return _namedPolicies; } }
        public void AddNamedPolicy(string policyName, string policy)
        {
            _namedPolicies[policyName] = policy;
            if (!Policies.Contains(policy))
                Policies.Add(policy);
        }
        public void AddPolicy(string policy)
        {
            _namedPolicies[policy] = policy;
            if (!Policies.Contains(policy))
                Policies.Add(policy);
        }
        public void AddPolicies(List<string> policies)
        {
            foreach (string policy in policies)
            {
                _namedPolicies[policy] = policy;
                if (!Policies.Contains(policy))
                    Policies.Add(policy);
            }
        }
        public void RemoveNamedPolicy(string policyName)
        {
            var policy = GetPolicy(policyName);
            if (!string.IsNullOrWhiteSpace(policy))
            {
                _namedPolicies.Remove(policyName);
                Policies.Remove(policy);
            }
        }
        public void RemovePolicy(string policy)
        {
            if (!string.IsNullOrWhiteSpace(policy))
            {
                foreach (var item in _namedPolicies.Where(kvp => kvp.Value == policy).ToList())
                {
                    _namedPolicies.Remove(item.Key);
                }
                Scopes.Remove(policy);
            }
        }
        public string GetPolicy(string policyName)
        {
            if (_namedPolicies.TryGetValue(policyName, out string policy))
                return policy;
            return null;
        }
        public List<string> GetAllPolicies()
        {
            return _namedPolicies.Values.ToList();
        }


        // Scopes - Don't have to use the Scope properties, can just use the Scopes directly. May replace with a Scope class later
        public static List<string> Scopes { get; } = new List<string>();

        private static Dictionary<string, string> _namedScopes = new Dictionary<string, string>();
        public Dictionary<string, string> NamedScopes { get { return _namedScopes; } }
        public void AddNamedScope(string scopeName, string scope)
        {
            _namedScopes[scopeName] = scope;
            if (!Scopes.Contains(scope))
                Scopes.Add(scope);
        }
        public void AddScope(string scope)
        {
            _namedScopes[scope] = scope;
            if (!Scopes.Contains(scope))
                Scopes.Add(scope);
        }
        public void AddScopes(List<string> scopes)
        {
            foreach (string scope in scopes)
            {
                _namedScopes[scope] = scope;
                if (!Scopes.Contains(scope))
                    Scopes.Add(scope);
            }
        }
        public void RemoveNamedScope(string scopeName)
        {
            var scope = GetScope(scopeName);
            if (!string.IsNullOrWhiteSpace(scope))
            {
                _namedScopes.Remove(scopeName);
                Scopes.Remove(scope);
            }
        }
        public void RemoveScope(string scope)
        {
            if (!string.IsNullOrWhiteSpace(scope))
            {
                foreach (var item in _namedScopes.Where(kvp => kvp.Value == scope).ToList())
                {
                    _namedScopes.Remove(item.Key);
                }
                Scopes.Remove(scope);
            }
        }
        public string GetScope(string scopeName)
        {
            if (_namedScopes.TryGetValue(scopeName, out string scope))
                return scope;
            return string.Empty;
        }
        public List<string> GetScopes(List<string> scopeNames)
        {
            List<string> scopes = new List<string>();
            if (scopeNames?.Count > 0)
            {
                scopes = new List<string>(scopeNames.Count);
                foreach (string name in scopeNames)
                {
                    string scope = GetScope(name);
                    if (!string.IsNullOrWhiteSpace(scope))
                        scopes.Add(scope);
                }
            }
            if (scopes.Count == 0)
                scopes.Add(string.Empty);
            return scopes;
        }
        public List<string> GetAllScopes()
        {
            return _namedScopes.Values.ToList();
        }



        // API Endpoints - Don't have to use the API endpoint properties, can just use the API endpoint directly. May replace with a ApiEndpoint class later
        public static List<string> ApiEndpoints { get; } = new List<string>();
        private static Dictionary<string, string> _namedApiEndpoints = new Dictionary<string, string>();
        public Dictionary<string, string> NamedApiEndpoints { get { return _namedApiEndpoints; } }
        public void AddNamedApiEndpoint(string apiEndpointName, string apiEndpoint)
        {
            _namedApiEndpoints[apiEndpointName] = apiEndpoint;
            if (!ApiEndpoints.Contains(apiEndpoint))
                ApiEndpoints.Add(apiEndpoint);
        }
        public void AddApiEndpoint(string apiEndpoint)
        {
            _namedApiEndpoints[apiEndpoint] = apiEndpoint;
            if (!ApiEndpoints.Contains(apiEndpoint))
                ApiEndpoints.Add(apiEndpoint);
        }
        public void AddApiEndpoints(List<string> apiEndpoints)
        {
            foreach (string apiEndpoint in apiEndpoints)
            {
                _namedApiEndpoints[apiEndpoint] = apiEndpoint;
                if (!ApiEndpoints.Contains(apiEndpoint))
                    ApiEndpoints.Add(apiEndpoint);
            }
        }
        public void RemoveNamedApiEndpoint(string apiEndpointName)
        {
            var apiEndpoint = GetApiEndpoint(apiEndpointName);
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                _namedApiEndpoints.Remove(apiEndpointName);
                ApiEndpoints.Remove(apiEndpoint);
            }
        }
        public void RemoveApiEndpoint(string apiEndpoint)
        {
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                foreach (var item in _namedApiEndpoints.Where(kvp => kvp.Value == apiEndpoint).ToList())
                {
                    _namedApiEndpoints.Remove(item.Key);
                }
                ApiEndpoints.Remove(apiEndpoint);
            }
        }
        public string GetApiEndpoint(string apiEndpointName = null)
        {
            if (_namedApiEndpoints.Count > 0)
            {
                if (apiEndpointName == null)
                    return _namedApiEndpoints.First().Value;
                else if (_namedApiEndpoints.TryGetValue(apiEndpointName, out string apiEndpoint))
                    return apiEndpoint;
            }
            return null;
        }
        public List<string> GetAllApiEndpoints()
        {
            return _namedApiEndpoints.Values.ToList();
        }



        // Access Tokens - Store for access tokens - temporary -  should be replaced in production version with secure Key store
        public string AccessToken {
            get { return GetAccessToken(); }
            set { AddDefaultAccessToken(value); }
        }
        
        //public string AccessTokenType { get; private set; } = string.Empty; // only for ADAL

        private static Dictionary<string, string> _accessTokens = new Dictionary<string, string>();
        public Dictionary<string, string> AccessTokens { get { return _accessTokens; } }
        public void AddAccessToken(string accessTokenName, string accessToken)
        {
            _accessTokens[accessTokenName] = accessToken;
        }
        public void AddDefaultAccessToken(string accessToken)
        {
            _accessTokens["Default"] = accessToken;
        }
        public void RemoveAccessToken(string accessTokenName)
        {
            _accessTokens.Remove(accessTokenName);
        }
        public string GetAccessToken(string accessTokenName = null)
        {
            if (_accessTokens.Count > 0)
            {
                if (accessTokenName == null)
                {
                    if (_accessTokens.TryGetValue("Default", out string accessToken))
                        return accessToken;
                    else
                        return _accessTokens.First().Value;
                }
                else if (_accessTokens.TryGetValue(accessTokenName, out string accessToken))
                {
                    return accessToken;
                }
            }
            return null;
        }
        public List<string> GetAccessTokens()
        {
            return _accessTokens.Values.ToList();
        }


        // UniqueUserID - temporary -  should be replaced in production version with secure Key store

        public string UniqueUserId { get; set; }


        public AuthenticationService() { }
        public AuthenticationService(string tentant, string clientId)
        {
            B2CTenant = tentant;
            ClientId = clientId;
        }
        public AuthenticationService(string tentant, string clientId, string authenticatePolicy)
        {
            B2CTenant = tentant;
            ClientId = clientId;
            AuthenticatePolicy = authenticatePolicy;

        }
        public AuthenticationService(string tentant, string clientId, string authenticatePolicy, string redirectUri)
        {
            B2CTenant = tentant;
            ClientId = clientId;
            AuthenticatePolicy = authenticatePolicy;
            PublicClientApplication = new PublicClientApplication(ClientId, Authority);
            PublicClientApplication.RedirectUri = redirectUri;
        }

        public string GetAuthority(string policyName)
        {
            if (!string.IsNullOrWhiteSpace(policyName))
            {
                string policy = GetPolicy(policyName);
                if (policy != null)
                {
                    return $"{AuthorityBase}{policy}";
                }
            }
            return null;
        }






        public async Task<AuthenticationResult> AuthenticateSilentlyAsync(List<string> scopes)
        {
            return await AuthenticateSilentlyAsync(Authority, AuthenticatePolicy, scopes, false);
        }
        public async Task<AuthenticationResult> AuthenticateSilentlyAsync(string policy, List<string> scopes, bool forceRefresh = false)
        {
            return await AuthenticateSilentlyAsync(Authority, policy, scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AuthenticateSilentlyAsync(List<string> scopes, bool forceRefresh = false)
        {
            return await AuthenticateSilentlyAsync(Authority, AuthenticatePolicy, scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AuthenticateSilentlyAsync(string authority, string policy, List<string> scopes, bool forceRefresh = false)
        {
            return await AuthenticateSilentlyAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AuthenticateSilentlyAsync(string authority, IUser user, List<string> scopes, bool forceRefresh = false)
        {
            if (scopes == null || scopes.Count == 0)
                scopes = new List<string> { string.Empty };
            try
            {
                return await PublicClientApplication.AcquireTokenSilentAsync(scopes, user, authority, forceRefresh);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(string policy, List<string> scopes, bool forceRefresh = false)
        {
            return await AcquireTokenSilentlyAsync(Authority, policy, scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(List<string> scopes, bool forceRefresh = false)
        {
            return await AcquireTokenWithPolicySilentlyAsync(Authority, AuthenticatePolicy, scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(string authority, string policy, List<string> scopes, bool forceRefresh = false)
        {
            return await AcquireTokenSilentlyAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh);
        }
        public async Task<AuthenticationResult> AcquireTokenWithPolicySilentlyAsync(string authority, string policy, List<string> scopes, bool forceRefresh = false)
        {
            return await AcquireTokenSilentlyAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh);
        }

        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(string authority, IUser user, List<string> scopes, bool forceRefresh = false)
        {
            if (scopes == null || scopes.Count == 0)
                scopes = new List<string> { string.Empty };
            try
            {
                return await PublicClientApplication.AcquireTokenSilentAsync(scopes, user, authority, forceRefresh);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(List<string> scopes)
        {
            return await AuthenticateWithCredentialsAsync(AuthenticatePolicy, scopes, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateWithCredentialsAsync(AuthenticatePolicy, scopes, uiParent);
        }
        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(string policy, List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateWithCredentialsAsync(GetUserByPolicy(PublicClientApplication.Users, policy), scopes, uiParent);
        }
        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(string policy, List<string> scopes)
        {
            return await AuthenticateWithCredentialsAsync(GetUserByPolicy(PublicClientApplication.Users, policy), scopes, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(IUser user, List<string> scopes)
        {
            return await AuthenticateWithCredentialsAsync(user, scopes, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateWithCredentialsAsync(IUser user, List<string> scopes, UIParent uiParent = null)
        {
            if (scopes == null || scopes.Count == 0)
                scopes = new List<string> { string.Empty };
            try
            {
                return await PublicClientApplication.AcquireTokenAsync(scopes, user, uiParent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public async Task<AuthenticationResult> AuthenticateAsync(List<string> scopes)
        {
            return await AuthenticateAsync(Authority, AuthenticatePolicy, scopes, false, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateAsync(Authority, AuthenticatePolicy, scopes, false, uiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(List<string> scopes, bool forceRefresh )
        {
            return await AuthenticateAsync(Authority, AuthenticatePolicy, scopes, forceRefresh, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(List<string> scopes, bool forceRefresh, UIParent uiParent)
        {
            return await AuthenticateAsync(Authority, AuthenticatePolicy, scopes, forceRefresh, uiParent);
        }


        public async Task<AuthenticationResult> AuthenticateAsync(string policy, List<string> scopes)
        {
            return await AuthenticateAsync(Authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, false, UiParent);
        }
       public async Task<AuthenticationResult> AuthenticateAsync(string policy, List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateAsync(Authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, false, uiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string policy, List<string> scopes, bool forceRefresh)
        {
            return await AuthenticateAsync(Authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string policy, List<string> scopes, bool forceRefresh, UIParent uiParent)
        {
            return await AuthenticateAsync(Authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh, uiParent);
        }


        public async Task<AuthenticationResult> AuthenticateAsync(string authority, string policy, List<string> scopes)
        {
            return await AuthenticateAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, false, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, string policy, List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, false, uiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, string policy, List<string> scopes, bool forceRefresh)
        {
            return await AuthenticateAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, string policy, List<string> scopes, bool forceRefresh, UIParent uiParent)
        {
            return await AuthenticateAsync(authority, GetUserByPolicy(PublicClientApplication.Users, policy), scopes, forceRefresh, uiParent);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string authority, IUser user, List<string> scopes)
        {
            return await AuthenticateAsync(authority, user, scopes, false, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, IUser user, List<string> scopes, UIParent uiParent)
        {
            return await AuthenticateAsync(authority, user, scopes, false, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, IUser user, List<string> scopes, bool forceRefresh)
        {
            return await AuthenticateAsync(authority, user, scopes, forceRefresh, UiParent);
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string authority, IUser user, List<string> scopes, bool forceRefresh, UIParent uiParent)
        {
            try
            {
                return await AuthenticateSilentlyAsync(authority, user, scopes, forceRefresh);
            }
            catch (Exception ex)
            {
                try
                {
                    return await AuthenticateWithCredentialsAsync(user, scopes, uiParent);
                }
                catch (Exception ex1)
                {
                    throw ex1;
                }
            }
        }






        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower()))
                    return user;
            }
            return null;
        }


        public Dictionary<string, string> GetUserInfo(AuthenticationResult ar)
        {
            Dictionary<string, string> userInfo = new Dictionary<string, string>();

            string name = ar?.User?.Name;
            string userid = ar?.User?.Identifier;
            string displaybleId = ar?.User?.DisplayableId;
            string idProvider = ar?.User?.IdentityProvider;
            string idToken = ar?.IdToken;

            JObject user = ParseIdToken(ar.IdToken);
            string email = user["emails"]?.ToString();
            if (email.Length > 13)
            {
                int length = email.Length;
                int start = email.IndexOf("\"");
                int end = email.IndexOf("\"\n]");
                email = email.Substring(start + 1, end - start - 1);
            }

            userInfo.Add("Name", name);
            userInfo.Add("UserID", userid);
            userInfo.Add("DisplayableID", displaybleId);
            userInfo.Add("Email", email);
            userInfo.Add("IDProvider", idProvider);
            userInfo.Add("ID", idToken);
            userInfo.Add("UserJson", user.ToString());

            return userInfo;
        }

        public Dictionary<string, object> GetAuthenticationInfo(AuthenticationResult ar)
        {
            Dictionary<string, object> authenticationInfo = new Dictionary<string, object>();

            var tenantId = ar?.TenantId;
            string uniqueId = ar?.UniqueId;
            var expOn = ar?.ExpiresOn;
            var scopes = ar?.Scopes;
            string accessToken = ar?.AccessToken;
            string idToken = ar?.IdToken;

            authenticationInfo.Add("TenantId", tenantId);
            authenticationInfo.Add("UniqueId",uniqueId);
            authenticationInfo.Add("ExpiresOn",expOn);
            authenticationInfo.Add("Scopes",scopes);
            authenticationInfo.Add("AccessToken",accessToken);
            authenticationInfo.Add("IdToken",idToken);

            return authenticationInfo;
        }



        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
        }

        private JObject ParseIdToken(string idToken)
        {
            string idToken1 = idToken;
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }




        public static class B2CDefaultPolicyTypes
        {
            public const string SignInSignUp = "SignInSignUp";
            public const string SignIn = "SignIn";
            public const string SignUp = "SignUp";
            public static string EditProfile = "EditProfile";
            public static string ResetPassword = "ResetPassword";
        }

        public static class DefaultAuthenticationSettings
        {
            // Default AD Authority
            public const string CommonAuthority = "https://login.windows.net/common";
            //Default Resource URI - Graph URI if you've given permission to Azure Active Directory from AD when registering App
            public const string ResourceUri = "https://graph.microsoft.com"; //"https://graph.windows.net";
        }


    }


    public class ADAuthenticatorException : Exception
    {
        public ADAuthenticatorException() { }
        public ADAuthenticatorException(string message) : base(message) { }

        public ADAuthenticatorException(string message, Exception inner) : base(message, inner) { }
    }


}
