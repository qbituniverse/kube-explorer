using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qu.nuget.azure.Models;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.infrastructure.Models;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace qu.nuget.azure.Services.Containers
{
    public class HelmService : IHelmService
    {
        public async Task<ApiResponse<string>> CheckScope(string acrServer, string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var checkResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head,
                        "/helm/v1/repo/_charts/"));

                    return new ApiResponse<string>(checkResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ChartsCatalog>> GetChartsCatalogFull(string acrServer, string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var catalogResponse = await client.GetAsync("/helm/v1/repo/_charts/");

                    if (!catalogResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<ChartsCatalog>(catalogResponse.StatusCode, null,
                            catalogResponse.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<ChartsCatalog>(catalogResponse.StatusCode,
                        JsonConvert.DeserializeObject<ChartsCatalog>(
                            catalogResponse.Content.ReadAsStringAsync().Result));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ChartsCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteChartVersion(
            string acrServer,
            string chartBlobUrl,
            string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var deleteResponse = await client.DeleteAsync($"/helm/v1/repo/{chartBlobUrl}");

                    return new ApiResponse<bool>(deleteResponse.StatusCode,
                        deleteResponse.StatusCode.Equals(HttpStatusCode.Accepted));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteChartRepository(
            string acrServer,
            string chartName,
            string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var deleteResponse = await client.DeleteAsync($"/helm/v1/repo/_charts/{chartName}/");

                    return new ApiResponse<bool>(deleteResponse.StatusCode,
                        deleteResponse.StatusCode.Equals(HttpStatusCode.Accepted));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }

        private static async Task<ChartsCatalog> GetChartsCatalogFromYaml(string acrServer, string acrAccessToken)
        {
            var chartsCatalog = new ChartsCatalog();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://{acrServer}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);

                var catalog = await client.GetAsync("/helm/v1/repo/index.yaml");
                var srCatalog = new StringReader(catalog.Content.ReadAsStringAsync().Result);
                var yamlStream = new YamlStream();
                yamlStream.Load(srCatalog);

                var yamlDocument = yamlStream.Documents.First();
                var rootMapping = (YamlMappingNode) yamlDocument.RootNode;
                var entriesMapping = (YamlMappingNode) rootMapping.Children["entries"];

                foreach (var entry in entriesMapping.Children)
                {
                    var mapping = (YamlSequenceNode) entry.Value;
                    var items = new List<Chart>();
                    foreach (var map in mapping.Children)
                    {
                        var serializer = new SerializerBuilder().Build();
                        var swMapping = new StringWriter();
                        serializer.Serialize(swMapping, map);
                        var yamlMapping = swMapping.ToString();
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(new CamelCaseNamingConvention()).Build();
                        var chartItem = deserializer.Deserialize<Chart>(yamlMapping);
                        items.Add(chartItem);
                    }
                    chartsCatalog.Add(entry.Key.ToString(), items);
                }
            }
            return chartsCatalog;
        }
    }
}