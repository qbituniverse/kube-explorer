using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.devops.Models;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.devops.Services.Interfaces
{
    public interface IReleaseService
    {
        Task<ApiResponse<Release>> Get(
            string releaseName,
            IEnumerable<string> projects,
            string organisation,
            string devOpsPatToken);
    }
}