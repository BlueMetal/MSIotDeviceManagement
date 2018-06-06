using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Extensions.Logging;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<UserProfileService> logger;
        private IUser _currentUser;

        public UserProfileService(IHttpContextAccessor httpContextAccessor, ILogger<UserProfileService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public Task<IUser> GetCurrentUser()
        {
            if (_currentUser != null)
                return Task.FromResult(_currentUser);



            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                
                logger.LogInformation("Creating CurrentUser object.");
                
                _currentUser = new User()
                {
                    UserPrincipalName = httpContextAccessor.HttpContext.User.FindFirst("upn").Value,
                    DisplayName = httpContextAccessor.HttpContext.User.FindFirst("name").Value,
                };
                return Task.FromResult(_currentUser);

       
            }
            else
            {
                logger.LogError("Current user Claim could not be found.");
                return null;
            }
        }
    }
   
}