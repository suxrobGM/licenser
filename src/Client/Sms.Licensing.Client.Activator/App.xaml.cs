using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using Serilog;
using Sms.Licensing.Client.Activator.Controls;
using Sms.Licensing.Client.Activator.Views;
using Sms.Licensing.Sdk.Client;
using Sms.Licensing.Sdk.Client.Abstractions;

namespace Sms.Licensing.Client.Activator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private IConfiguration _configuration;
        private string _productName;

        protected override void OnStartup(StartupEventArgs e)
        {
            _configuration = Activator.Startup.BuildConfiguration();
            Log.Logger = Activator.Startup.CreateLogger(_configuration);
            Log.Logger?.Information("-------------------------------------------------");
            Log.Logger?.Information("Application started");
            Log.Logger?.Information($"Arguments: {string.Join(',', e.Args)}");

            ParseArguments(e.Args);
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var apiClientOptions = _configuration.GetSection("SmsApiClientOptions").Get<SmsApiClientOptions>();
            apiClientOptions.ClientSecret = "66DDE100-9DFB-4907-81F5-569B166D0BE0"; // Super secret code, do not reveal in public
            apiClientOptions.ProductName = _productName;

            containerRegistry.RegisterInstance(typeof(IConfiguration), _configuration);
            containerRegistry.RegisterInstance(typeof(SmsApiClientOptions), apiClientOptions);
            containerRegistry.RegisterSingleton<ISmsApiClient, SmsApiClient>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<ResultPage>();
            containerRegistry.RegisterDialog<MessageDialog, MessageDialogViewModel>(nameof(MessageDialog));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        private void ParseArguments(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-p", "product" },
                { "--product", "product" }
            };

            var config = new ConfigurationBuilder()
                .AddCommandLine(args, switchMappings)
                .Build();

            _productName = config["product"];
        }
    }
}
