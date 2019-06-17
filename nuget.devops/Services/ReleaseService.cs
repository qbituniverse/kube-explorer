using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using qu.nuget.devops.Models;
using qu.nuget.devops.Services.Interfaces;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.devops.Services
{
    public class ReleaseService : IReleaseService
    {
        public Task<ApiResponse<Release>> Get(
            string releaseName, 
            IEnumerable<string> projects, 
            string organisation, 
            string devOpsPatToken)
        {
            throw new NotImplementedException();
        }
    }
}