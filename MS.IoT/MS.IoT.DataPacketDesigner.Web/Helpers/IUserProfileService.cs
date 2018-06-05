using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Threading.Tasks;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    /// <summary>
    /// Interface for User Profile Service
    /// </summary>
	public interface IUserProfileService
	{
        Task<IUser> GetCurrentUser();
    }
}