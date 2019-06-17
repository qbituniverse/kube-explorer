using System.Threading.Tasks;
using qu.nuget.azure.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.SecurityAndManagement.ActiveDirectory.Interfaces
{
    public interface IUserProfileService
    {
        Task<ApiResponse<UserProfile>> GetUserFromAzureActiveDirectory(
            string tenant, 
            string clientId, 
            string secret, 
            string emailAddress);
    }
}