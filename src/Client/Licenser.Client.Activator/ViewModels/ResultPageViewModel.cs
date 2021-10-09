using System;
using System.Threading.Tasks;
using System.Windows;
using Licenser.Client.Activator.ViewModels.Abstractions;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using Licenser.Sdk.Client;
using Licenser.Sdk.Client.Abstractions;
using Licenser.Shared;
using Licenser.Shared.Models;

namespace Licenser.Client.Activator.ViewModels
{
    public class ResultPageViewModel : PageViewModelBase
    {
        private readonly string _credentialsFile;
        private readonly ISmsApiClient _apiClient;

        public ResultPageViewModel(IRegionManager regionManager, 
            ISmsApiClient apiClient,
            IConfiguration configuration) : base(regionManager)
        {
            _apiClient = apiClient;
            _credentialsFile = configuration["ClientCredentialsFile"];
            DialogOkCommand = new DelegateCommand(async () => await SendActivationRequestAsync());
            DialogCancelCommand = new DelegateCommand(NavigateToLoginPage);
        }

        #region Commands

        private DelegateCommand _dialogOkCommand;

        public DelegateCommand DialogOkCommand
        {
            get => _dialogOkCommand;
            set => SetProperty(ref _dialogOkCommand, value);
        }
        public DelegateCommand DialogCancelCommand { get; }

        #endregion

        #region Bindable properties

        private string _dialogMessage;
        public string DialogMessage
        {
            get => _dialogMessage;
            set => SetProperty(ref _dialogMessage, value);
        }

        private bool _dialogOkButtonVisible;
        public bool DialogOkButtonVisible
        {
            get => _dialogOkButtonVisible;
            set => SetProperty(ref _dialogOkButtonVisible, value);
        }

        private bool _dialogCancelButtonVisible;
        public bool DialogCancelButtonVisible
        {
            get => _dialogCancelButtonVisible;
            set => SetProperty(ref _dialogCancelButtonVisible, value);
        }

        #endregion

        #region Methods

        protected override Task LoadPageAsync()
        {
            return CheckLicenseStatusAsync();
        }

        private async Task SendActivationRequestAsync()
        {
            IsBusy = true;
            DialogOkButtonVisible = false;
            DialogCancelButtonVisible = false;

            if (string.IsNullOrEmpty(_credentialsFile) || !_apiClient.IsAuthenticated())
            {
                DisplayError("Authorization error, please try to login again.", NavigateToLoginPage);
                return;
            }

            try
            {
                var clientCredentials = BinarySerializer.Deserialize<UserAdvancedCredentials>(_credentialsFile);

                if (clientCredentials == null)
                {
                    DisplayError("Authorization error, please try to login again.", NavigateToLoginPage);
                    return;
                }

                DialogMessage = "Sending activation request...";
                var apiResponse = await _apiClient.CreateActivationRequestAsync();

                if (apiResponse.Status == ApiResponseStatus.Error)
                {
                    DisplayError(apiResponse.Message, () => CloseApp(-1));
                    return;
                }

                DialogOkButtonVisible = true;
                DialogOkCommand = new DelegateCommand(async () => await CheckLicenseStatusAsync(true));

                var requestStatus = apiResponse.Data;
                switch (requestStatus)
                {
                    case ActivationRequestStatus.RequestCreated:
                        DialogMessage = "Your activation request sent to server.\nClick OK to check license status.";
                        break;
                    case ActivationRequestStatus.RequestAlreadyMade:
                        DialogMessage =
                            "Your already sent activation request to server.\nPlease wait while your request to be approved." +
                            "\nClick OK to check again.";
                        DialogOkCommand = new DelegateCommand(async () => await CheckLicenseStatusAsync(true));
                        break;
                    case ActivationRequestStatus.AlreadyHaveValidLicense:
                        DialogMessage = "You already have valid license.\nClick OK to close activator.";
                        DialogOkCommand = new DelegateCommand(() => CloseApp(ClientIntegration.ActivatorSuccessExitCode));
                        break;
                }
            }
            catch (Exception e)
            {
                DisplayError(e.Message, () => CloseApp(-1));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CheckLicenseStatusAsync(bool recheckingLicense = false)
        {
            IsBusy = true;
            DialogOkButtonVisible = false;
            DialogCancelButtonVisible = false;
            DialogMessage = "Checking license status...";
            
            try
            {
                var apiResponse = await _apiClient.CheckLicenseAsync();
                DialogOkButtonVisible = true;
                DialogCancelButtonVisible = true;

                if (apiResponse.Status == ApiResponseStatus.Error)
                {
                    DisplayError(apiResponse.Message, () => CloseApp(-1));
                    return;
                }

                var licenseStatus = apiResponse.Data;
                switch (licenseStatus)
                {
                    case LicenseStatus.Valid:
                        DialogMessage = "Your already have valid license.";
                        DialogCancelButtonVisible = false;
                        DialogOkCommand = new DelegateCommand(() => CloseApp(ClientIntegration.ActivatorSuccessExitCode));
                        break;

                    case LicenseStatus.Expired when recheckingLicense:
                        DialogMessage =
                            "Your request has not yet been approved.\nPlease wait while your request to be approved." +
                            "\nClick OK to check again.";

                        DialogOkCommand = new DelegateCommand(async () => await CheckLicenseStatusAsync(true));
                        break;

                    case LicenseStatus.Expired:
                        DialogMessage =
                            "Your license already expired. \nPlease send new activation request in order to get new license." +
                            "\nDo you want to renew license? \nClick OK to send activation request.";

                        DialogOkCommand = new DelegateCommand(async () => await SendActivationRequestAsync());
                        break;

                    case LicenseStatus.Invalid when recheckingLicense:
                        DialogMessage =
                            "Your request has not yet been approved.\nPlease wait while your request to be approved." +
                            "\nClick OK to check again.";

                        DialogOkCommand = new DelegateCommand(async () => await CheckLicenseStatusAsync(true));
                        break;

                    case LicenseStatus.Invalid:
                        DialogMessage =
                            "Your do not have valid license. \nPlease send new activation request in order to get new license." +
                            "\nDo you want to request new license? \nClick OK to send activation request.";

                        DialogOkCommand = new DelegateCommand(async () => await SendActivationRequestAsync());
                        break;
                }
            }
            catch (Exception e)
            {
                DisplayError(e.Message, () => CloseApp(-1));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void NavigateToLoginPage()
        {
            DialogOkButtonVisible = false;
            DialogCancelButtonVisible = false;
            var navigationParameters = new NavigationParameters
            {
                {"LoadCredentials", false}
            };
            _regionManager.RequestNavigate("MainPageFrame", "LoginPage", navigationParameters);
        }

        private void CloseApp(int exitCode)
        {
            Application.Current.Shutdown(exitCode);
        }

        private void DisplayError(string errorMsg, Action okButtonAction)
        {
            DialogMessage = $"ERROR: {errorMsg}";
            DialogOkButtonVisible = true;
            DialogOkCommand = new DelegateCommand(okButtonAction);
            Log.Error(errorMsg);
        }

        #endregion
    }
}