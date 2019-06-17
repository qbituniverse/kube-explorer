using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using qu.kubeexplorer.webapp.Helpers;
using qu.kubeexplorer.webapp.Models;
using qu.nuget.azure.Services.Containers;
using qu.nuget.azure.Services.Containers.Interfaces;
using qu.nuget.azure.Services.SecurityAndManagement.Portal;
using qu.nuget.azure.Services.SecurityAndManagement.Portal.Interfaces;
using qu.nuget.devops.Services;
using qu.nuget.devops.Services.Interfaces;

namespace qu.kubeexplorer.webapp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new Configuration();
            Configuration.Bind("Configuration", configuration);
            services.AddSingleton(configuration);
            
            services.AddSingleton<IPortalService, PortalService>();
            services.AddSingleton<IAcrService, AcrService>();
            services.AddSingleton<IDockerService, DockerService>();
            services.AddSingleton<IHelmService, HelmService>();
            services.AddSingleton<IAksService, AksService>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<IBuildService, BuildService>();
            services.AddSingleton<IReleaseService, ReleaseService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Constants.Policies.Acr,
                    policyBuilder => policyBuilder
                        .RequireClaim(Constants.AccessTokens.AcrAccessToken)
                        .RequireRole(Constants.Roles.AcrReader, Constants.Roles.AcrContributor));
                options.AddPolicy(
                    Constants.Policies.Aks,
                    policyBuilder => policyBuilder
                        .RequireClaim(Constants.AccessTokens.AzureAccessToken)
                        .RequireRole(Constants.Roles.AksReader));
                options.AddPolicy(
                    Constants.Policies.DevOps,
                    policyBuilder => policyBuilder
                        .RequireClaim(Constants.AccessTokens.DevOpsAccessToken)
                        .RequireRole(Constants.Roles.DevOpsReader));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/DontExist";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/DontExist");
            if (Configuration.GetSection("Configuration").Get<Configuration>().Application.ShowException)
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}