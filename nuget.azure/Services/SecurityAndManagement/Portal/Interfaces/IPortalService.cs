using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.SecurityAndManagement.Portal.Interfaces
{
    public interface IPortalService
    {
        Task<ApiResponse<string>> GetAzureAccessToken(string tenantId, string clientId, string clientSecret);
        Task<ApiResponse<string>> GetAcrRefreshToken(string tenantId, string acrServer, string azureAccessToken);
        Task<ApiResponse<string>> GetAcrAccessToken(string acrServer, string scope, string acrRefreshToken);
        Task<ApiResponse<Subscriptions>> GetSubscriptions(string azureAccessToken);
    }
}