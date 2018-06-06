using System.Threading.Tasks;

namespace MS.IoT.MarketingPortal.Web
{
    public interface ITokenServices
    {
        Task<string> GetAzureManagementTokenAsync();
        Task<string> GetGraphTokenAsync();
        string TenantId { get; }
        string DirectoryInstance { get; }
    }
}