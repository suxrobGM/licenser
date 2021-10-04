using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Sms.Licensing.Client.Activator
{
    public static class Startup
    {
        public static IConfiguration BuildConfiguration(string rootPath = null)
        {
            var appRootPath = string.IsNullOrEmpty(rootPath) ? Directory.GetCurrentDirectory() : rootPath;
            var envName = Environment.GetEnvironmentVariable("ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(appRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{envName}.json", true);

            return configuration.Build();
        }

        public static ILogger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}