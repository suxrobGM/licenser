using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using IdentityModel;
using IdentityServer4.Models;

namespace Sms.IdentityServer
{
    public static class Config
    {
        public static IList<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource()
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    UserClaims = { JwtClaimTypes.Role }
                }
            };
        }

        public static IList<ApiResource> GetApiResources(IConfiguration configuration)
        {
            var apiResources = configuration.GetSection("IdentityServerConfig:ApiResources").Get<List<ApiResource>>();
            return apiResources;
        }

        public static IList<Client> GetClients(IConfiguration configuration)
        {
            var clients = configuration.GetSection("IdentityServerConfig:Clients").Get<List<Client>>();
            return clients;
        }
    }
}