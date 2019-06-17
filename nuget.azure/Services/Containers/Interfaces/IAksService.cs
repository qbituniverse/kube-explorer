using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.Containers.Interfaces
{
    public interface IAksService
    {
        Task<ApiResponse<ClustersCatalog>> GetClusters(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken);

        Task<ApiResponse<ClustersCatalog>> GetClustersWithAccessToken(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken);

        Task<ApiResponse<ClustersCatalog>> GetClustersWithDeployment(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken);

        Task<ApiResponse<Deployment>> GetDeployment(string k8SUri, string k8SAccessToken);
    }
}