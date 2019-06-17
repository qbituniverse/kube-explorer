using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qu.kubeexplorer.webapp.Helpers;
using qu.nuget.azure.Services.Containers.Interfaces;

namespace qu.kubeexplorer.webapp.Controllers
{
    [Authorize(Policy = Constants.Policies.Acr)]
    public class ChartsController : Controller
    {
        private readonly IHelmService _helmService;

        public ChartsController(IHelmService helmService)
        {
            _helmService = helmService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetChartsCatalogFull()
        {
            var apiResponse = await _helmService.GetChartsCatalogFull(
                User.FindFirstValue(Constants.Uri.AcrUri),
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int)apiResponse.StatusCode};
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.AcrContributor)]
        public async Task<IActionResult> DeleteChartVersion(string chartBlobUrl)
        {
            var apiResponse = await _helmService.DeleteChartVersion(
                User.FindFirstValue(Constants.Uri.AcrUri),
                chartBlobUrl,
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) { StatusCode = (int)apiResponse.StatusCode };
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.AcrContributor)]
        public async Task<IActionResult> DeleteChartRepository(string chartName)
        {
            var apiResponse = await _helmService.DeleteChartRepository(
                User.FindFirstValue(Constants.Uri.AcrUri),
                chartName,
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }
    }
}