using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Licenser.IdentityServer.Areas.Identity.IdentityHostingStartup))]
namespace Licenser.IdentityServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}