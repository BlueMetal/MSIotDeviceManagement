using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Threading.Tasks;

namespace MS.IoT.DeviceManagementPortal.Web.Helpers
{
    /// <summary>
    /// Interface for User Profile Service
    /// </summary>
	public interface IUserProfileService
	{
        Task<IUser> GetCurrentUser();
    }
}