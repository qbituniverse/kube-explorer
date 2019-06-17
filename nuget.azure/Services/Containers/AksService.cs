using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qu.nuget.azure.Dto;
using qu.nuget.azure.Models;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.infrastructure.Models;
using YamlDotNet.RepresentationModel;

namespace qu.nuget.azure.Services.Containers
{
    public class AksService : IAksService
    {
        private const string AksApiVersion = "2018-03-31";

        public async Task<ApiResponse<ClustersCatalog>> GetClusters(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://management.azure.com");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", azureAccessToken);

                    var clustersCatalog = new ClustersCatalog();

                    foreach (var subscriptionId in subscriptionIds)
                    {
                        var aksClustersResponse = await client.GetAsync(
                            $"/subscriptions/{subscriptionId}/providers/Microsoft.ContainerService" +
                            $"/managedClusters?api-version={AksApiVersion}");

                        if (!aksClustersResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        clustersCatalog.Add(subscriptionId, JsonConvert.DeserializeObject<Clusters>(
                            aksClustersResponse.Content.ReadAsStringAsync().Result));
                    }

                    return clustersCatalog.Count.Equals(0)
                        ? new ApiResponse<ClustersCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ClustersCatalog>(HttpStatusCode.OK, clustersCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClustersCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ClustersCatalog>> GetClustersWithAccessToken(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://management.azure.com");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", azureAccessToken);

                    var clustersCatalog = new ClustersCatalog();

                    foreach (var subscriptionId in subscriptionIds)
                    {
                        var aksClustersResponse = await client.GetAsync(
                            $"/subscriptions/{subscriptionId}/providers/Microsoft.ContainerService" +
                            $"/managedClusters?api-version={AksApiVersion}");

                        if (!aksClustersResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var clustersResponse = JsonConvert.DeserializeObject<Clusters>(
                            aksClustersResponse.Content.ReadAsStringAsync().Result);

                        var clusters = new Clusters();

                        foreach (var clusterResponse in clustersResponse.value)
                        {
                            var aksCredentialsResponse = await client.PostAsync(
                                $"{clusterResponse.id}/accessProfiles/clusterAdmin/listCredential?api-version={AksApiVersion}",
                                null);

                            if (!aksCredentialsResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                            var credential = JsonConvert.DeserializeObject<ClusterDto.AdminUser>(
                                aksCredentialsResponse.Content.ReadAsStringAsync().Result);

                            var cluster = clusterResponse;
                            var accessToken = await GetAccessTokenFromKubeconfig(credential.properties.kubeConfig);
                            cluster.accessToken = accessToken;
                            clusters.value.Add(cluster);
                        }

                        clustersCatalog.Add(subscriptionId, clusters);
                    }

                    return clustersCatalog.Count.Equals(0)
                        ? new ApiResponse<ClustersCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ClustersCatalog>(HttpStatusCode.OK, clustersCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClustersCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ClustersCatalog>> GetClustersWithDeployment(
            IEnumerable<string> subscriptionIds,
            string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://management.azure.com");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", azureAccessToken);

                    var clustersCatalog = new ClustersCatalog();

                    foreach (var subscriptionId in subscriptionIds)
                    {
                        var aksClustersResponse = await client.GetAsync(
                            $"/subscriptions/{subscriptionId}/providers/Microsoft.ContainerService" +
                            $"/managedClusters?api-version={AksApiVersion}");

                        if (!aksClustersResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var clustersResponse = JsonConvert.DeserializeObject<Clusters>(
                            aksClustersResponse.Content.ReadAsStringAsync().Result);

                        var clusters = new Clusters();

                        foreach (var clusterResponse in clustersResponse.value)
                        {
                            var aksCredentialsResponse = await client.PostAsync(
                                $"{clusterResponse.id}/accessProfiles/clusterAdmin/listCredential?api-version={AksApiVersion}",
                                null);

                            if (!aksCredentialsResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                            var credential = JsonConvert.DeserializeObject<ClusterDto.AdminUser>(
                                aksCredentialsResponse.Content.ReadAsStringAsync().Result);

                            var cluster = clusterResponse;
                            var accessToken = await GetAccessTokenFromKubeconfig(credential.properties.kubeConfig);
                            cluster.accessToken = accessToken;

                            var handler = new HttpClientHandler
                            {
                                ClientCertificateOptions = ClientCertificateOption.Manual,
                                ServerCertificateCustomValidationCallback =
                                    (httpRequestMessage, cert, cetChain, policyErrors) => true
                            };
                            using (var clientHandler = new HttpClient(handler))
                            {
                                clientHandler.BaseAddress = new Uri($"https://{cluster.properties.fqdn}");
                                clientHandler.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue("Bearer", accessToken);

                                var deploymentResponse = await clientHandler.GetAsync("/apis/apps/v1/deployments");

                                if (!deploymentResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                                var deployment = JsonConvert.DeserializeObject<Deployment>(
                                    deploymentResponse.Content.ReadAsStringAsync().Result);

                                cluster.deployment = deployment;
                            }

                            clusters.value.Add(cluster);
                        }

                        clustersCatalog.Add(subscriptionId, clusters);
                    }

                    return clustersCatalog.Count.Equals(0)
                        ? new ApiResponse<ClustersCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ClustersCatalog>(HttpStatusCode.OK, clustersCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClustersCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<Deployment>> GetDeployment(string k8SUri, string k8SAccessToken)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri($"https://{k8SUri}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", k8SAccessToken);

                    var deploymentResponse = await client.GetAsync("/apis/apps/v1/deployments");

                    if (!deploymentResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<Deployment>(deploymentResponse.StatusCode, null,
                            deploymentResponse.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<Deployment>(deploymentResponse.StatusCode,
                        JsonConvert.DeserializeObject<Deployment>(
                            deploymentResponse.Content.ReadAsStringAsync().Result));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Deployment>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        private static async Task<string> GetAccessTokenFromKubeconfig(string kubeconfig)
        {
            try
            {
                var decodedBytes = Convert.FromBase64String(kubeconfig);
                var kubeconfigDecoded = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var kubeconfigSr = new StringReader(kubeconfigDecoded);
                var yamlStream = new YamlStream();
                yamlStream.Load(kubeconfigSr);
                var yamlDocument = yamlStream.Documents.First();
                var rootMapping = (YamlMappingNode) yamlDocument.RootNode;
                var usersMapping = (YamlSequenceNode) rootMapping.Children["users"];
                var token = ((YamlScalarNode) usersMapping.Children.Select(n =>
                    ((YamlMappingNode) n).Children["user"]["token"]).First()).Value;
                return await Task.FromResult(token);
            }
            catch
            {
                return await Task.FromResult(string.Empty);
            }
        }
    }
}