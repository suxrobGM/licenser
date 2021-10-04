using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Sms.IdentityServer.Areas.Identity.IdentityHostingStartup))]
namespace Sms.IdentityServer.Areas.Identity
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