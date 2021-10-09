using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Licenser.Client.Activator.ViewModels.Abstractions;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using Serilog;
using Licenser.Sdk.Client.Abstractions;
using Licenser.Shared;
using Licenser.Shared.Models;

namespace Licenser.Client.Activator.ViewModels
{
    public class LoginPageViewModel : PageViewModelBase, INavigationAware
    {
        private readonly string _credentialsFile;
        private readonly ISmsApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly IDialogService _dialogService;
        private UserAdvancedCredentials _clientCredentials;
        private bool _loadCredentials;
        
        public LoginPageViewModel(IRegionManager regionManager, 
            ISmsApiClient apiClient, 
            IConfiguration configuration,
            IDialogService dialogService) : base(regionManager)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _dialogService = dialogService;
            _credentialsFile = _configuration["ClientCredentialsFile"];
            _loadCredentials = true;
            SendLoginCommand = new DelegateCommand(async () => await SendDataAsync(), CanExecuteSendLoginCommand);
            RegisterCommand = new DelegateCommand(NavigateToResultPage);
            OpenSignUpCommand = new DelegateCommand(OpenSignUpPage);
            OpenForgotPasswordCommand = new DelegateCommand(OpenForgotPasswordPage);
            DialogOkCommand = new DelegateCommand(() => ShowDialog = false);
            SendLoginCommand.ObservesProperty(() => UserName);
            SendLoginCommand.ObservesProperty(() => Password);
            SendLoginCommand.ObservesProperty(() => IsBusy);
            ShowMainContent = true;

            OnBusyAction += (sender, args) =>
            {
                RaisePropertyChanged(nameof(InputEnabled));
            };
        }

        #region Commands
        
        public DelegateCommand SendLoginCommand { get; }
        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand OpenSignUpCommand { get; }
        public DelegateCommand OpenForgotPasswordCommand { get; }
        public DelegateCommand DialogOkCommand { get; }

        #endregion

        #region Bindable properties

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password;
        public string Password
        {
            get => _password; 
            set => SetProperty(ref _password, value);
        }

        private string _dialogMessage;
        public string DialogMessage
        {
            get => _dialogMessage;
            set => SetProperty(ref _dialogMessage, value);
        }

        public bool InputEnabled => !IsBusy;

        private bool _showMainContent;

        public bool ShowMainContent
        {
            get => _showMainContent;
            set => SetProperty(ref _showMainContent, value);
        }

        private bool _showDialog;
        public bool ShowDialog
        {
            get => _showDialog;
            set
            {
                SetProperty(ref _showDialog, value);
                ShowMainContent = !value;
            }
        }

        #endregion

        #region Methods

        protected override async Task LoadPageAsync()
        {
            if (!_loadCredentials || string.IsNullOrEmpty(_credentialsFile))
                return;

            try
            {
                IsBusy = true;
                _clientCredentials = BinarySerializer.Deserialize<UserAdvancedCredentials>(_credentialsFile);

                if (_clientCredentials != null)
                {
                    UserName = _clientCredentials.UserName;
                    Password = _clientCredentials.Password;

                    LoadingText = "Trying to sign in...";
                    Log.Information($"Loaded user credentials from {_credentialsFile}");

                    var accessTokenResponse = await _apiClient.AuthenticatePasswordAsync(_clientCredentials);
                    if (accessTokenResponse.Status == ApiResponseStatus.Success)
                    {
                        Log.Information($"User {UserName} logged successfully.");
                        NavigateToResultPage();
                    }
                    else
                    {
                        ShowDialog = true;
                        DialogMessage = "Incorrect login. Please check your username or password." +
                                        $"\nResponse message: {accessTokenResponse.Message}";
                        Log.Warning(DialogMessage);
                    }
                }
            }
            catch (Exception e)
            {
                ShowDialog = true;
                DialogMessage = e.Message;
                Log.Error(DialogMessage);
            }
            finally
            {
                Password = "";
                IsBusy = false;
            }
        }

        private async Task SendDataAsync()
        {
            try
            {
                IsBusy = true;
                var userCredentials = new UserAdvancedCredentials()
                {
                    UserName = UserName,
                    Password = Password
                };
                
                LoadingText = "Trying to sign in...";
                var accessTokenResponse = await _apiClient.AuthenticatePasswordAsync(userCredentials);

                if (accessTokenResponse.Status == ApiResponseStatus.Success)
                {
                    Log.Information($"User {UserName} logged successfully.");
                    BinarySerializer.Serialize(userCredentials, _credentialsFile); // save login info to binary file

                    Log.Information($"Saved user credentials to {_credentialsFile}");
                    NavigateToResultPage();
                }
                else
                {
                    ShowDialog = true;
                    DialogMessage = "Incorrect login. Please check your username or password." +
                                    $"\nResponse message: {accessTokenResponse.Message}";
                    Log.Warning(DialogMessage);
                }
            }
            catch (Exception e)
            {
                ShowDialog = true;
                DialogMessage = e.Message;
                Log.Error(DialogMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OpenSignUpPage()
        {
            var webUrl = _configuration["SignUpPageUrl"];
            Process.Start(new ProcessStartInfo(webUrl)
            {
                UseShellExecute = true
            });
        }

        private void OpenForgotPasswordPage()
        {
            var webUrl = _configuration["ForgotPasswordPageUrl"];
            Process.Start(new ProcessStartInfo(webUrl)
            {
                UseShellExecute = true
            });
        }

        private void NavigateToResultPage()
        {
            _regionManager.RequestNavigate("MainPageFrame", "ResultPage");
        }

        private bool CanExecuteSendLoginCommand()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !IsBusy;
        }

        #endregion

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameters.TryGetValue("LoadCredentials", out _loadCredentials);
        }

        #endregion
    }
}