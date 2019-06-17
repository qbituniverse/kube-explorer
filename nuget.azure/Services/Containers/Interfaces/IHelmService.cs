using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.Containers.Interfaces
{
    public interface IHelmService
    {
        Task<ApiResponse<string>> CheckScope(string acrServer, string acrAccessToken);
        Task<ApiResponse<ChartsCatalog>> GetChartsCatalogFull(string acrServer, string acrAccessToken);
        Task<ApiResponse<bool>> DeleteChartVersion(string acrServer, string chartBlobUrl, string acrAccessToken);
        Task<ApiResponse<bool>> DeleteChartRepository(string acrServer, string chartName, string acrAccessToken);
    }
}