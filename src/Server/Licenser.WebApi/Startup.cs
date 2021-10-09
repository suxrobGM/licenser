using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Licenser.WebApi.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Licenser.Domain.Entities;
using Licenser.Domain.Repositories.Abstractions;
using Licenser.Infrastructure.Data;
using Licenser.Infrastructure.Repositories;

namespace Licenser.WebApi
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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ILicenseRepository), typeof(LicenseRepository));
            services.AddScoped(typeof(IActivationRequestRepository), typeof(ActivationRequestRepository));

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                        Configuration.GetConnectionString("RemoteDbConnection"))
                    .UseLazyLoadingProxies());

            // Identity Core
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_.-";
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // API Controllers
            services.AddControllers();

            // API versions format the version as "'v'major[.minor]"
            services.AddApiVersioning();
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VV" );
            
            // Open API (swagger)
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            // Authentication with JWT Bearer
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>  
            {
                options.Authority = Configuration["JWT:Authority"];
                options.Audience = Configuration["JWT:Audience"];
                options.ClaimsIssuer = Configuration["JWT:Issuer"];
                options.SaveToken = true;  
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()  
                {
                    ValidateIssuer = true,  
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new JsonWebKey(GetJwk())
                };
            });

            // Authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.IsAdmin, builder =>
                {
                    builder.RequireClaim("scope", "sms.api.admin");
                    builder.RequireRole("SuperAdmin", "Admin");
                });

                options.AddPolicy(Policies.IsClient, builder =>
                {
                    builder.RequireClaim("scope", "sms.api.client");
                    builder.RequireRole("Client");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant() );
                    }
                } );
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetJwk()
        {
            var fileRelPath = Configuration["JWT:KeyFilePath"];
            var fileAbsPath = Path.Combine(Directory.GetCurrentDirectory(), fileRelPath);
            return File.ReadAllText(fileAbsPath);
        }
    }

    // Configure swagger
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
            _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = $"SMS API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Contact = new OpenApiContact()
                        {
                            Name = "Smart Meal Service",
                            Email = "info@smartmealservice.com",
                            Url = new Uri("https://smartmealservice.com/")
                        }
                    });
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'", 
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
                
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }
    }
}
