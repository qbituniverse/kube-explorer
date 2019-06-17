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
    public class BuildService : IBuildService
    {
        private const string DevOpsApiVersion = "5.0";

        public async Task<ApiResponse<Build>> Get(
            string buildNumber, 
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
                            Convert.ToBase64String(
                                Encoding.UTF8.GetBytes($":{devOpsPatToken}")));

                    foreach (var project in projects)
                    {
                        var buildResponse = await client.GetAsync(
                            $"{organisation}/{project}/_apis/build/builds?api-version=" +
                            $"{DevOpsApiVersion}&buildNumber={buildNumber}");

                        if (!buildResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var builds = JsonConvert.DeserializeObject<Builds>(
                            buildResponse.Content.ReadAsStringAsync().Result);

                        if (builds.value.Count.Equals(0)) continue;
                        
                        return new ApiResponse<Build>(buildResponse.StatusCode, builds.value[0]);
                    }

                    return new ApiResponse<Build>(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Build>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }
    }
}