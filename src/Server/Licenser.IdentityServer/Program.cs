using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Licenser.Server.Infrastructure.Data;

namespace Licenser.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SMS Identity Server";
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            SeedData.Initialize(serviceProvider);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);

                }, writeToProviders: true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
