using System.Threading.Tasks;

namespace MS.IoT.DataPacketDesigner.Web.Helpers
{
    public interface ITokenServices
    {
        Task<string> GetAzureManagementTokenAsync();
        Task<string> GetGraphTokenAsync();
        string TenantId { get; }
        string DirectoryInstance { get; }
    }
}