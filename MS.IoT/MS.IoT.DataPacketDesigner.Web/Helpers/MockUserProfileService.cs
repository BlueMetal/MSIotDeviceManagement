using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    /// <summary>
    /// Mock Class for User Profile Service that returns a user "John Doe", for tests.
    /// </summary>
    public class MockUserProfileService : IUserProfileService
    {
        private string _clientId;
        private string _appKey;
        private string _aadInstance;
        private string _graphResourceID;
       // private IUser _currentUser;

        public MockUserProfileService(string graphResourceID, string clientId, string appKey, string aadInstance)
        {
            _graphResourceID = graphResourceID;
            _clientId = clientId;
            _appKey = appKey;
            _aadInstance = aadInstance;
        }

        public Task<IUser> GetCurrentUser()
        {
            return Task.FromResult<IUser>(new User() {
                UserPrincipalName = "JohnDoe@test.com",
                DisplayName = "John Doe",
            });
        }
    }
   
}