using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qu.nuget.azure.Models;
using qu.nuget.azure.Services.SecurityAndManagement.Portal.Interfaces;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.SecurityAndManagement.Portal
{
    public class PortalService : IPortalService
    {
        private const string SubscriptionApiVersion = "2016-06-01";

        public async Task<ApiResponse<string>> GetAzureAccessToken(
            string tenantId,
            string clientId,
            string clientSecret)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://login.microsoftonline.com");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("resource", "https://management.azure.com/")
                    });
                    var response = await client.PostAsync($"/{tenantId}/oauth2/token", content);

                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<string>(response.StatusCode, null,
                            response.Content.ReadAsStringAsync().Result);

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        response.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<string>(response.StatusCode,
                        result.First(t => t.Key == "access_token").Value);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> GetAcrRefreshToken(
            string tenantId,
            string acrServer,
            string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "access_token"),
                        new KeyValuePair<string, string>("service", acrServer),
                        new KeyValuePair<string, string>("tenant", tenantId),
                        new KeyValuePair<string, string>("access_token", azureAccessToken)
                    });
                    var response = await client.PostAsync("/oauth2/exchange", content);

                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<string>(response.StatusCode, null,
                            response.Content.ReadAsStringAsync().Result);

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        response.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<string>(response.StatusCode,
                        result.First(t => t.Key == "refresh_token").Value);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> GetAcrAccessToken(string acrServer, string scope, string acrRefreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{acrServer}");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("service", acrServer),
                        new KeyValuePair<string, string>("scope",
                            $"repository:*:{scope} registry:catalog:* artifact-repository:repo:{scope}"),
                        new KeyValuePair<string, string>("refresh_token", acrRefreshToken)
                    });
                    var response = await client.PostAsync("/oauth2/token", content);

                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<string>(response.StatusCode, null,
                            response.Content.ReadAsStringAsync().Result);

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        response.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<string>(response.StatusCode,
                        result.First(t => t.Key == "access_token").Value);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<ApiResponse<Subscriptions>> GetSubscriptions(string azureAccessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://management.azure.com");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", azureAccessToken);

                    var subscriptionsResponse =
                        await client.GetAsync($"/subscriptions?api-version={SubscriptionApiVersion}");

                    if (!subscriptionsResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<Subscriptions>(subscriptionsResponse.StatusCode, null,
                            subscriptionsResponse.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<Subscriptions>(subscriptionsResponse.StatusCode,
                        JsonConvert.DeserializeObject<Subscriptions>(
                            subscriptionsResponse.Content.ReadAsStringAsync().Result));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Subscriptions>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }
    }
}