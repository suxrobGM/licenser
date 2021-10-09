using System.IO;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Options;
using Licenser.Server.Domain.Services.Abstractions;
using Licenser.Server.Infrastructure.Data;
using Licenser.Server.Infrastructure.Services;

namespace Licenser.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } 

        public void ConfigureServices(IServiceCollection services)
        {
            // Infrastructure layer
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddScoped(_ => Configuration.GetSection("GoogleRecaptchaV2").Get<GoogleCaptchaOptions>());
            services.AddScoped<IGoogleCaptchaService, GoogleCaptchaService>();

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                        Configuration.GetConnectionString("RemoteDbConnection"))
                    .UseLazyLoadingProxies());

            // Identity
            services.AddIdentity<User, Role>(options => 
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_.-";
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<CustomClaimsFactory>();

            // Identity Server 4
            services.AddIdentityServer()
                .AddApiAuthorization<User, ApplicationDbContext>(options =>
                {
                    options.Clients = new ClientCollection(Config.GetClients(Configuration));
                    options.ApiResources = new ApiResourceCollection(Config.GetApiResources(Configuration));
                    options.IdentityResources = new IdentityResourceCollection(Config.GetIdentityResources());
                    options.SigningCredential = new SigningCredentials(new JsonWebKey(GetJwk()), "RS256");
                });

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private string GetJwk()
        {
            var fileRelPath = Configuration["IdentityServerConfig:JWK:FilePath"];
            var fileAbsPath = Path.Combine(Directory.GetCurrentDirectory(), fileRelPath);
            return File.ReadAllText(fileAbsPath);
        }
    }
}
