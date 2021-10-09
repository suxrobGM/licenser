using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

using Licenser.Domain.Services.Abstractions;
using Licenser.Infrastructure.Services;
using Licenser.Sdk.Client;
using Licenser.Sdk.Server;
using Licenser.Web.Authentication;
using Licenser.Web.Resources;

namespace Licenser.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Syncfusion
            SyncfusionLicenseProvider.RegisterLicense(Configuration.GetSection("SyncfusionLicenseKey").Value);

            // Infrastructure layer
            services.AddScoped<AuthenticationHelper>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddSingleton(_ => Configuration.GetSection("SmsApiClientOptions").Get<SmsApiClientOptions>());
            services.AddSingleton<ISmsApiServer, SmsApiServer>();

            // Web layer
            ConfigureLocalization(services);

            services.AddAuthentication(options => 
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc",options =>
                {
                    options.Authority = Configuration["SmsApiClientOptions:IdentityServerAddress"];
                    options.ClientId = Configuration["SmsApiClientOptions:ClientId"];
                    options.ClientSecret = Configuration["SmsApiClientOptions:ClientSecret"];
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.Add(new JsonKeyClaimAction(ClaimTypes.Role, ClaimValueTypes.String, "role"));
                    options.ClaimActions.Add(new JsonKeyClaimAction(ClaimTypes.Name, ClaimValueTypes.String, "name"));

                    var scopes = Configuration["SmsApiClientOptions:Scope"].Split(' ');
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                });

            services.AddAccessTokenManagement();

            services.AddSyncfusionBlazor();
            services.AddServerSideBlazor()
                .AddCircuitOptions(options =>
                {
                    if (_env.IsDevelopment())
                        options.DetailedErrors = true;
                });

            services.AddRazorPages()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => 
                        factory.Create(typeof(SharedResource));
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
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
            
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions?.Value);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        #region Configuration Layers

        private static void ConfigureLocalization(IServiceCollection services)
        {
            // Localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        #endregion
    }
}
