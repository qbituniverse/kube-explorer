using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qu.kubeexplorer.webapp.Helpers;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.azure.Services.SecurityAndManagement.Portal.Interfaces;

namespace qu.kubeexplorer.webapp.Controllers
{
    [Authorize(Policy = Constants.Policies.Aks)]
    public class K8SController : Controller
    {
        private readonly IPortalService _portalService;
        private readonly IAksService _aksService;

        public K8SController(
            IPortalService portalService,
            IAksService aksService)
        {
            _portalService = portalService;
            _aksService = aksService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            var apiResponse = await _portalService.GetSubscriptions(
                User.FindFirstValue(Constants.AccessTokens.AzureAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpPost]
        public async Task<IActionResult> GetClusters(IEnumerable<string> subscriptionIds)
        {
            var apiResponse = await _aksService.GetClusters(
                subscriptionIds,
                User.FindFirstValue(Constants.AccessTokens.AzureAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpPost]
        public async Task<IActionResult> GetClustersWithAccessToken(IEnumerable<string> subscriptionIds)
        {
            var apiResponse = await _aksService.GetClustersWithAccessToken(
                subscriptionIds,
                User.FindFirstValue(Constants.AccessTokens.AzureAccessToken));
            return new ObjectResult(apiResponse) { StatusCode = (int)apiResponse.StatusCode };
        }

        [HttpPost]
        public async Task<IActionResult> GetClustersWithDeployment(IEnumerable<string> subscriptionIds)
        {
            var apiResponse = await _aksService.GetClustersWithDeployment(
                subscriptionIds,
                User.FindFirstValue(Constants.AccessTokens.AzureAccessToken));
            return new ObjectResult(apiResponse) { StatusCode = (int)apiResponse.StatusCode };
        }

        [HttpPost]
        public async Task<IActionResult> GetDeployment(string k8SUri, string k8SAccessToken)
        {
            var apiResponse = await _aksService.GetDeployment(k8SUri, k8SAccessToken);
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }
    }
}