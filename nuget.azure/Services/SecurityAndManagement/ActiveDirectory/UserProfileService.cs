using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using qu.nuget.azure.Models;
using qu.nuget.azure.Services.SecurityAndManagement.ActiveDirectory.Interfaces;
using qu.nuget.infrastructure.Models;

namespace qu.nuget.azure.Services.SecurityAndManagement.ActiveDirectory
{
    public class UserProfileService : IUserProfileService
    {
        private const string GraphApiVersion = "1.6";

        public async Task<ApiResponse<UserProfile>> GetUserFromAzureActiveDirectory(
            string tenant,
            string clientId,
            string secret,
            string emailAddress)
        {
            try
            {
                var token = await GetAuthenticationToken(tenant, clientId, secret);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var queryStringParameters = $"$filter=userPrincipalName eq '{emailAddress}'";
                    var uri =
                        $"https://graph.windows.net/{tenant}/users?api-version={GraphApiVersion}&{queryStringParameters}";

                    var userResponse = await client.GetAsync(uri);

                    if (!userResponse.StatusCode.Equals(HttpStatusCode.OK))
                        return new ApiResponse<UserProfile>(userResponse.StatusCode, null,
                            userResponse.Content.ReadAsStringAsync().Result);

                    return new ApiResponse<UserProfile>(userResponse.StatusCode,
                        JsonConvert.DeserializeObject<UserProfile>(
                            userResponse.Content.ReadAsStringAsync().Result));
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserProfile>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        private async Task<AuthenticationResult> GetAuthenticationToken(string tenant, string clientId, string secret)
        {
            var clientCred = new ClientCredential(clientId, secret);
            var context = new AuthenticationContext($"https://login.windows.net/{tenant}");
            var result = await context.AcquireTokenAsync("https://graph.windows.net", clientCred);

            return result;
        }
    }
}