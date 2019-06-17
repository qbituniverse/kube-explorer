using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.Containers.Interfaces
{
    public interface IAcrService
    {
        Task<ApiResponse<AcrsCatalog>> GetAcrs(IEnumerable<string> subscriptionIds, string azureAccessToken);
    }
}