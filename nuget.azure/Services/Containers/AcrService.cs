using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qu.nuget.azure.Models;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.Containers
{
    public class AcrService : IAcrService
    {
        private const string AcrApiVersion = "2017-10-01";

        public async Task<ApiResponse<AcrsCatalog>> GetAcrs(IEnumerable<string> subscriptionIds, string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://management.azure.com");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", azureAccessToken);

                    var acrsCatalog = new AcrsCatalog();

                    foreach (var subscriptionId in subscriptionIds)
                    {
                        var acrsResponse = await client.GetAsync(
                            $"/subscriptions/{subscriptionId}/providers/Microsoft.ContainerRegistry" +
                            $"/registries?api-version={AcrApiVersion}");

                        if (!acrsResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var acrs = JsonConvert.DeserializeObject<Acrs>(
                            acrsResponse.Content.ReadAsStringAsync().Result);
                        if (acrs.value.Count.Equals(0)) continue;

                        acrsCatalog.Add(subscriptionId, acrs);
                    }

                    return acrsCatalog.Count.Equals(0)
                        ? new ApiResponse<AcrsCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<AcrsCatalog>(HttpStatusCode.OK, acrsCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<AcrsCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }
    }
}