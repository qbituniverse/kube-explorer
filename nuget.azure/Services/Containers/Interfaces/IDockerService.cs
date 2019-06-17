using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.Containers.Interfaces
{
    public interface IDockerService
    {
        Task<ApiResponse<string>> CheckScope(string acrServer, string acrAccessToken);
        Task<ApiResponse<ImagesCatalog>> GetImagesCatalog(string acrServer, string acrAccessToken);
        Task<ApiResponse<ImagesCatalog>> GetImagesCatalogWithTags(string acrServer, string acrAccessToken);
        Task<ApiResponse<ImagesCatalog>> GetImagesCatalogFull(string acrServer, string acrAccessToken);

        Task<ApiResponse<ImagesCatalog>> GetImageDetails(
            string acrServer, 
            string imageName,
            IEnumerable<string> imageTags, 
            string acrAccessToken);

        Task<ApiResponse<bool>> DeleteImageTag(
            string acrServer, string imageName, 
            string imageDigest,
            string acrAccessToken);

        Task<ApiResponse<bool>> DeleteImageRepository(string acrServer, string imageName, string acrAccessToken);
    }
}