using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qu.kubeexplorer.webapp.Helpers;
using qu.nuget.azure.Services.Containers.Interfaces;

namespace qu.kubeexplorer.webapp.Controllers
{
    [Authorize(Policy = Constants.Policies.Acr)]
    public class ImagesController : Controller
    {
        private readonly IDockerService _dockerService;

        public ImagesController(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetImagesCatalog()
        {
            var apiResponse = await _dockerService.GetImagesCatalog(
                User.FindFirstValue(Constants.Uri.AcrUri),
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpGet]
        public async Task<IActionResult> GetImagesCatalogWithTags()
        {
            var apiResponse = await _dockerService.GetImagesCatalogWithTags(
                User.FindFirstValue(Constants.Uri.AcrUri),
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpGet]
        public async Task<IActionResult> GetImagesCatalogFull()
        {
            var apiResponse = await _dockerService.GetImagesCatalogFull(
                User.FindFirstValue(Constants.Uri.AcrUri),
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpPost]
        public async Task<IActionResult> GetImageDetails(string imageName, IEnumerable<string> imageTags)
        {
            var apiResponse = await _dockerService.GetImageDetails(
                User.FindFirstValue(Constants.Uri.AcrUri),
                imageName,
                imageTags,
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.AcrContributor)]
        public async Task<IActionResult> DeleteImageTag(string imageName, string imageDigest)
        {
            var apiResponse = await _dockerService.DeleteImageTag(
                User.FindFirstValue(Constants.Uri.AcrUri),
                imageName,
                imageDigest,
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.AcrContributor)]
        public async Task<IActionResult> DeleteImageRepository(string imageName)
        {
            var apiResponse = await _dockerService.DeleteImageRepository(
                User.FindFirstValue(Constants.Uri.AcrUri),
                imageName,
                User.FindFirstValue(Constants.AccessTokens.AcrAccessToken));
            return new ObjectResult(apiResponse) {StatusCode = (int) apiResponse.StatusCode};
        }
    }
}