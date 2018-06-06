using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MS.IoT.DataPacketDesigner.Web.Models;
using MS.IoT.Common;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IOptionsSnapshot<AzureAdOptions> options;
        private readonly IHttpContextAccessor httpContextAccessor;
        private IUser _currentUser;

        public UserProfileService(IOptionsSnapshot<AzureAdOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            this.options = options;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<IUser> GetCurrentUser()
        {
            if (_currentUser != null)
                return Task.FromResult(_currentUser);

            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                Log.Information("Creating CurrentUser object.");
                _currentUser = new User()
                {
                    UserPrincipalName = httpContextAccessor.HttpContext.User.FindFirst("upn").Value,
                    DisplayName = httpContextAccessor.HttpContext.User.FindFirst("name").Value,
                };
                return Task.FromResult(_currentUser);
            }
            else
            {
                Log.Error("Current user Claim could not be found.");
                return null;
            }
        }
    }
   
}