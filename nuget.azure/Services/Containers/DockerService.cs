using System;
using System.Collections.Generic;
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

namespace qu.nuget.azure.Services.Containers
{
    public class DockerService : IDockerService
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
                        "/v2/test/tags/list"));

                    return new ApiResponse<string>(checkResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ImagesCatalog>> GetImagesCatalog(string acrServer, string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var catalogResponse = await client.GetAsync("/v2/_catalog");

                    if (!catalogResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<ImagesCatalog>(catalogResponse.StatusCode, null,
                            catalogResponse.Content.ReadAsStringAsync().Result);

                    var catalog = JsonConvert.DeserializeObject<ImagesDto>(
                        catalogResponse.Content.ReadAsStringAsync().Result);

                    var imagesCatalog = new ImagesCatalog();

                    foreach (var image in catalog.repositories) imagesCatalog.Add(image, new List<Image>());

                    return imagesCatalog.Count.Equals(0)
                        ? new ApiResponse<ImagesCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ImagesCatalog>(catalogResponse.StatusCode, imagesCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImagesCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ImagesCatalog>> GetImagesCatalogWithTags(string acrServer, string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var catalogResponse = await client.GetAsync("/v2/_catalog");

                    if (!catalogResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<ImagesCatalog>(catalogResponse.StatusCode, null,
                            catalogResponse.Content.ReadAsStringAsync().Result);

                    var catalog = JsonConvert.DeserializeObject<ImagesDto>(
                        catalogResponse.Content.ReadAsStringAsync().Result);

                    var imagesCatalog = new ImagesCatalog();

                    foreach (var image in catalog.repositories)
                    {
                        var tagsResponse = await client.GetAsync($"/v2/{image}/tags/list");

                        if (!tagsResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var tags = JsonConvert.DeserializeObject<ImageDto.Tag>(
                            tagsResponse.Content.ReadAsStringAsync().Result);
                        tags.tags.Reverse();

                        var imageItems = tags.tags.Select(tag => new Image {tag = tag}).ToList();
                        imagesCatalog.Add(image, imageItems);
                    }

                    return imagesCatalog.Count.Equals(0)
                        ? new ApiResponse<ImagesCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ImagesCatalog>(HttpStatusCode.OK, imagesCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImagesCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ImagesCatalog>> GetImagesCatalogFull(string acrServer, string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var catalogResponse = await client.GetAsync("/v2/_catalog");

                    if (!catalogResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<ImagesCatalog>(catalogResponse.StatusCode, null,
                            catalogResponse.Content.ReadAsStringAsync().Result);

                    var catalog = JsonConvert.DeserializeObject<ImagesDto>(
                        catalogResponse.Content.ReadAsStringAsync().Result);

                    var imagesCatalog = new ImagesCatalog();

                    foreach (var image in catalog.repositories)
                    {
                        var tagsResponse = await client.GetAsync($"/v2/{image}/tags/list");

                        if (!tagsResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var tags = JsonConvert.DeserializeObject<ImageDto.Tag>(
                            tagsResponse.Content.ReadAsStringAsync().Result);
                        tags.tags.Reverse();

                        var imageItems = new List<Image>();
                        var acceptV1 =
                            new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v1+json");
                        var acceptV2 =
                            new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v2+json");

                        foreach (var tag in tags.tags)
                        {
                            client.DefaultRequestHeaders.Accept.Add(acceptV1);

                            var manifestResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                                $"/v2/{image}/manifests/{tag}"));

                            if (!manifestResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                            var manifest = JsonConvert.DeserializeObject<ImageDto.ManifestData>(
                                manifestResponse.Content.ReadAsStringAsync().Result);
                            var history =
                                JsonConvert.DeserializeObject<ImageDto.V1Compatibility>(manifest.history[0].v1Compatibility);

                            client.DefaultRequestHeaders.Accept.Remove(acceptV1);

                            client.DefaultRequestHeaders.Accept.Add(acceptV2);

                            var digestResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head,
                                $"/v2/{image}/manifests/{tag}"));

                            if (!digestResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                            var digest = string.Empty;
                            if (digestResponse.Headers.TryGetValues("Docker-Content-Digest", out var header))
                                digest = header.First();

                            client.DefaultRequestHeaders.Accept.Remove(acceptV2);

                            imageItems.Add(new Image
                            {
                                tag = tag,
                                digest = digest,
                                created = DateTime.Parse(history.created),
                                architecture = history.architecture,
                                labels = new ImageDto.Labels
                                {
                                    maintainer = history.config.Labels.maintainer,
                                    description = history.config.Labels.description
                                }
                            });
                        }
                        imagesCatalog.Add(image, imageItems);
                    }

                    return imagesCatalog.Count.Equals(0)
                        ? new ApiResponse<ImagesCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ImagesCatalog>(HttpStatusCode.OK, imagesCatalog);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImagesCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<ImagesCatalog>> GetImageDetails(
            string acrServer, 
            string imageName,
            IEnumerable<string> imageTags, 
            string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var imageItems = new List<Image>();
                    var acceptV1 =
                        new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v1+json");
                    var acceptV2 =
                        new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v2+json");

                    foreach (var tag in imageTags)
                    {
                        client.DefaultRequestHeaders.Accept.Add(acceptV1);

                        var manifestResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                            $"/v2/{imageName}/manifests/{tag}"));

                        if (!manifestResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var manifest = JsonConvert.DeserializeObject<ImageDto.ManifestData>(
                            manifestResponse.Content.ReadAsStringAsync().Result);
                        var history =
                            JsonConvert.DeserializeObject<ImageDto.V1Compatibility>(manifest.history[0].v1Compatibility);

                        client.DefaultRequestHeaders.Accept.Remove(acceptV1);

                        client.DefaultRequestHeaders.Accept.Add(acceptV2);

                        var digestResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head,
                            $"/v2/{imageName}/manifests/{tag}"));

                        if (!digestResponse.StatusCode.Equals(HttpStatusCode.OK)) continue;

                        var digest = string.Empty;
                        if (digestResponse.Headers.TryGetValues("Docker-Content-Digest", out var header))
                            digest = header.First();

                        client.DefaultRequestHeaders.Accept.Remove(acceptV2);

                        imageItems.Add(new Image
                        {
                            tag = tag,
                            digest = digest,
                            created = DateTime.Parse(history.created),
                            architecture = history.architecture,
                            labels = new ImageDto.Labels
                            {
                                maintainer = history.config.Labels.maintainer,
                                description = history.config.Labels.description
                            }
                        });
                    }

                    return imageItems.Count.Equals(0)
                        ? new ApiResponse<ImagesCatalog>(HttpStatusCode.NotFound, null)
                        : new ApiResponse<ImagesCatalog>(HttpStatusCode.OK,
                            new ImagesCatalog {{imageName, imageItems}});
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImagesCatalog>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageTag(
            string acrServer, 
            string imageName, 
            string imageDigest,
            string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var deleteResponse = await client.DeleteAsync($"/v2/{imageName}/manifests/{imageDigest}");

                    return new ApiResponse<bool>(deleteResponse.StatusCode,
                        deleteResponse.StatusCode.Equals(HttpStatusCode.Accepted));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageRepository(
            string acrServer, 
            string imageName,
            string acrAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", acrAccessToken);

                    var deleteResponse = await client.DeleteAsync($"/v2/_acr/{imageName}/repository");

                    return new ApiResponse<bool>(deleteResponse.StatusCode,
                        deleteResponse.StatusCode.Equals(HttpStatusCode.Accepted));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }
    }
}