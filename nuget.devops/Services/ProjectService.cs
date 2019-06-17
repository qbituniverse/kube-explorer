using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qu.nuget.devops.Models;
using qu.nuget.devops.Services.Interfaces;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.devops.Services
{
    public class ProjectService : IProjectService
    {
        private const string DevOpsApiVersion = "5.0";

        public async Task<ApiResponse<bool>> CheckAccess(
            IEnumerable<string> projects, 
            string organisation, 
            string devOpsPatToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://dev.azure.com/");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes($":{devOpsPatToken}")));

                    var organisationResponse = await client.GetAsync(
                        $"{organisation}/_apis/projects?api-version={DevOpsApiVersion}");

                    if (!organisationResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<bool>(organisationResponse.StatusCode, false, string.Empty);

                    var projectsList = JsonConvert.DeserializeObject<Projects>(
                        organisationResponse.Content.ReadAsStringAsync().Result);

                    var hasProjectMatch = false;
                    foreach (var project in projects)
                    {
                        foreach (var projectList in projectsList.value)
                        {
                            if (projectList.name.Equals(project)) hasProjectMatch = true;
                        }
                    }

                    return new ApiResponse<bool>(HttpStatusCode.InternalServerError, hasProjectMatch, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }
    }
}