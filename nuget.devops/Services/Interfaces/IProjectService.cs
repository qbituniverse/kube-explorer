using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.devops.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ApiResponse<bool>> CheckAccess(
            IEnumerable<string> projects, 
            string organisation, 
            string devOpsPatToken);
    }
}