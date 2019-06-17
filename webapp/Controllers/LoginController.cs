using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qu.kubeexplorer.webapp.Helpers;
using qu.kubeexplorer.webapp.Models;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.azure.Services.SecurityAndManagement.Portal.Interfaces;
using qu.nuget.devops.Services.Interfaces;
using qu.nuget.infrastructure.Extensions;

namespace qu.kubeexplorer.webapp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private Configuration Configuration { get; }
        private readonly IPortalService _portalService;
        private readonly IAcrService _acrService;
        private readonly IDockerService _dockerService;
        private readonly IHelmService _helmService;
        private readonly IAksService _aksService;
        //private readonly IProjectService _projectService;
        //private readonly IBuildService _buildService;

        public LoginController(
            Configuration configuration,
            IPortalService portalService,
            IAcrService acrService,
            IDockerService dockerService,
            IHelmService helmService,
            IAksService aksService,
            IProjectService projectService,
            IBuildService buildService)
        {
            Configuration = configuration;
            _portalService = portalService;
            _acrService = acrService;
            _dockerService = dockerService;
            _helmService = helmService;
            _aksService = aksService;
            //_projectService = projectService;
            //_buildService = buildService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserLogin userLogin)
        {
            ViewBag.ErrorMessage = "User login details not found.";
            if (!userLogin.Exists()) return View();

            try
            {
                if (!Configuration.Application.AutoLogin)
                {
                    // check username config
                    ViewBag.ErrorMessage = "Username config missing";
                    if (!UsernameOk()) return View();

                    // check password config
                    ViewBag.ErrorMessage = "Password config missing";
                    if (!PasswordOk()) return View();

                    // check username provided
                    ViewBag.ErrorMessage = "Please enter Username";
                    if (string.IsNullOrWhiteSpace(userLogin.UserName)) return View();

                    // check password provided
                    ViewBag.ErrorMessage = "Please enter Password";
                    if (string.IsNullOrWhiteSpace(userLogin.Password)) return View();

                    // check password match
                    ViewBag.ErrorMessage = "Invalid Password entered";
                    if (!userLogin.Password.Equals(Configuration.Application.Password)) return View();

                    // check Azure AD membership
                    ViewBag.ErrorMessage = "Invalid Username entered";
                    if (!userLogin.UserName.Equals(Configuration.Application.Username)) return View();
                }
                else
                {
                    userLogin.UserName = "anonymous";
                }
                
                // start building user claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userLogin.UserName)
                };

                // check Azure
                if (AzureClientOk())
                {
                    var azureAccessToken = await AcquireAzureAccessToken();
                    if (!string.IsNullOrWhiteSpace(azureAccessToken))
                    {
                        var subscriptions = await _portalService.GetSubscriptions(azureAccessToken);
                        if (subscriptions.Result.value.Count > 0)
                        {
                            // check ACR
                            var acrs = await _acrService.GetAcrs(
                                subscriptions.Result.value.Select(s => s.subscriptionId).ToList(),
                                azureAccessToken);

                            if (acrs.Result.Values.Count > 0 && acrs.Result.Values.First().value.Count > 0)
                            {
                                var acrServer = acrs.Result.Values.First().value.First().properties.loginServer;
                                if (!string.IsNullOrWhiteSpace(acrServer))
                                {
                                    var acrAccessToken = await AcquireAcrAccessToken(
                                        Constants.Scopes.Push,
                                        acrServer,
                                        azureAccessToken);
                                    if (!string.IsNullOrWhiteSpace(acrAccessToken))
                                    {
                                        var imageScope = await _dockerService.CheckScope(acrServer, acrAccessToken);
                                        var chartScope = await _helmService.CheckScope(acrServer, acrAccessToken);

                                        if (imageScope.StatusCode.Equals(HttpStatusCode.Unauthorized) &&
                                            chartScope.StatusCode.Equals(HttpStatusCode.Unauthorized))
                                        {
                                            acrAccessToken = await AcquireAcrAccessToken(
                                                Constants.Scopes.Pull,
                                                acrServer,
                                                azureAccessToken);
                                            if (!string.IsNullOrWhiteSpace(acrAccessToken))
                                            {
                                                claims.Add(new Claim(ClaimTypes.Role, Constants.Roles.AcrReader));
                                                claims.Add(new Claim(Constants.Uri.AcrUri, acrServer));
                                                claims.Add(new Claim(Constants.AccessTokens.AcrAccessToken,
                                                    acrAccessToken));
                                            }
                                        }
                                        else
                                        {
                                            claims.Add(new Claim(ClaimTypes.Role, Constants.Roles.AcrContributor));
                                            claims.Add(new Claim(Constants.Uri.AcrUri, acrServer));
                                            claims.Add(new Claim(Constants.AccessTokens.AcrAccessToken,
                                                acrAccessToken));
                                        }
                                    }
                                }
                            }

                            // check AKS
                            var aksScope = await _aksService.GetClustersWithAccessToken(
                                subscriptions.Result.value.Select(s => s.subscriptionId).ToList(),
                                azureAccessToken);
                            var hasAccessToAks = false;
                            foreach (var subs in aksScope.Result.Values)
                            {
                                if (subs.value.Count > 0) hasAccessToAks = true;
                            }
                            if (hasAccessToAks)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, Constants.Roles.AksReader));
                                claims.Add(new Claim(Constants.AccessTokens.AzureAccessToken, azureAccessToken));
                            }
                        }
                    }
                }

                // check DevOps
                //if (AzureDevOpsOk())
                //{
                //    var hasDevopsAccess = await _projectService.CheckAccess(
                //        Configuration.DevOps.Projects.Split('|').ToList(),
                //        Configuration.DevOps.Organisation,
                //        Configuration.DevOps.PatToken);
                //    if (hasDevopsAccess.Result)
                //    {

                //        var build = await _buildService.Get("4.72.19023",
                //            Configuration.DevOps.Projects.Split('|').ToList(),
                //            Configuration.DevOps.Organisation,
                //            Configuration.DevOps.PatToken);

                //        claims.Add(new Claim(ClaimTypes.Role, Constants.Roles.DevOpsReader));
                //        claims.Add(new Claim(Constants.AccessTokens.DevOpsAccessToken, Configuration.DevOps.PatToken));
                //    }
                //}

                // create user principal
                ViewBag.ErrorMessage = null;

                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddHours(1),
                        IsPersistent = false,
                        AllowRefresh = false
                    });
                
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.ErrorMessage = "Error ocurred during login process! Please try again later...";
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        private bool UsernameOk()
            => !string.IsNullOrWhiteSpace(Configuration.Application.Username);

        private bool PasswordOk()
            => !string.IsNullOrWhiteSpace(Configuration.Application.Password);

        private bool AzureClientOk()
            => !string.IsNullOrWhiteSpace(Configuration.Azure.ClientId) &&
               !string.IsNullOrWhiteSpace(Configuration.Azure.ClientSecret);

        //private bool AzureDevOpsOk()
        //    => !string.IsNullOrWhiteSpace(Configuration.DevOps.Organisation) &&
        //       !string.IsNullOrWhiteSpace(Configuration.DevOps.Projects) &&
        //       !string.IsNullOrWhiteSpace(Configuration.DevOps.PatToken);

        private async Task<string> AcquireAzureAccessToken()
        {
            var azureAccessToken = await _portalService.GetAzureAccessToken(
                Configuration.Azure.TenantId,
                Configuration.Azure.ClientId,
                Configuration.Azure.ClientSecret);
            return azureAccessToken.Result;
        }

        private async Task<string> AcquireAcrAccessToken(string scope, string acrServer, string azureAccessToken)
        {
            var acrRefreshToken = await _portalService.GetAcrRefreshToken(
                Configuration.Azure.TenantId,
                acrServer,
                azureAccessToken);
            var acrAccessToken = await _portalService.GetAcrAccessToken(
                acrServer,
                scope,
                acrRefreshToken.Result);
            return acrAccessToken.Result;
        }
    }
}