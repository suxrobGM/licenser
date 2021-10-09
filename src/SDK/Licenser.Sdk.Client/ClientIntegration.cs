using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Licenser.Sdk.Client.Abstractions;
using Licenser.Shared;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Client
{
    /// <summary>
    /// Builtin methods to validate license status, it can be easily integrate in client startup page.
    /// </summary>
    public class ClientIntegration
    {
        private readonly ISmsApiClient _apiClient;
        private readonly string _activatorAppPath;
        private readonly string _credentialsFilePath;

        /// <summary>
        /// Activator exit code means that activator successfully validated license then close itself without terminating client app.
        /// </summary>
        public const int ActivatorSuccessExitCode = 19980729; // haha my birthday :)

        /// <summary>
        /// Constructor of ClientIntegration.
        /// </summary>
        /// <param name="apiClient">Instance of ISmsApiClient</param>
        /// <param name="activatorAppPath">Absolute path of activator app. By default, its path is {AppDirectory}\Activator\SmsActivator.exe</param>
        /// <exception cref="ArgumentNullException">Throws exception if apiClient is null</exception>
        public ClientIntegration(ISmsApiClient apiClient, string activatorAppPath = null)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

            _activatorAppPath = string.IsNullOrEmpty(activatorAppPath) ? 
                Path.Combine(Directory.GetCurrentDirectory(), "Activator", "SmsActivator.exe") : activatorAppPath;
            
            _credentialsFilePath = Path.Combine(Path.GetDirectoryName(_activatorAppPath) ?? string.Empty,
                "ClientCredentials.dat");
        }

        /// <summary>
        /// Check authorization then validate license
        /// </summary>
        /// <param name="actionOnAuthorizing">Invokes action when authorizing client to remote server, use it for displaying messages in client app.</param>
        /// <param name="actionOnCheckingLicense">Invokes action when checking license status, use it for displaying messages in client app.</param>
        /// <param name="actionOnValidLicense">Invokes action when license status is valid, use it for displaying messages in client app.</param>
        /// <param name="actionOnExpiredLicense">Invokes action when license status is expired, use it for displaying messages in client app.</param>
        /// <param name="actionOnInvalidLicense">Invokes action when license status is invalid, use it for displaying messages in client app.</param>
        /// <param name="actionOnAnyException">Invokes action when thrown any exception (Exception class), use it for displaying messages in client app.</param>
        /// <returns>True if license valid, otherwise false (in case expired or invalid status)</returns>
        public async Task<bool> ValidateLicenseAsync(
            Action actionOnAuthorizing = null, 
            Action actionOnCheckingLicense = null,
            Action actionOnValidLicense = null,
            Action actionOnExpiredLicense = null,
            Action actionOnInvalidLicense = null,
            Action<Exception> actionOnAnyException = null)
        {
            try
            {
                actionOnAuthorizing?.Invoke();

                if (!File.Exists(_credentialsFilePath))
                    return false;

                var clientCredentials =
                    BinarySerializer.Deserialize<UserAdvancedCredentials>(_credentialsFilePath);

                if (clientCredentials == null)
                    return false;

                var authenticateResponse = await _apiClient.AuthenticatePasswordAsync(clientCredentials);

                if (authenticateResponse.Status == ApiResponseStatus.Error)
                    return false;

                actionOnCheckingLicense?.Invoke();
                var checkLicenseResponse = await _apiClient.CheckLicenseAsync();

                if (checkLicenseResponse.Status == ApiResponseStatus.Error)
                    return false;

                switch (checkLicenseResponse.Data)
                {
                    case LicenseStatus.Valid:
                        actionOnValidLicense?.Invoke();
                        return true;
                    case LicenseStatus.Expired:
                        actionOnExpiredLicense?.Invoke();
                        break;
                    case LicenseStatus.Invalid:
                        actionOnInvalidLicense?.Invoke();
                        break;
                }
            }
            catch (Exception e)
            {
                actionOnAnyException?.Invoke(e);
            }

            return false;
        }

        /// <summary>
        /// Starts activator application to sign in account then activate license.
        /// </summary>
        /// <param name="actionOnException">Invokes action when thrown any exception (Exception class), use it for displaying messages in client app.</param>
        /// <returns></returns>
        public void StartActivatorApp(Action<Exception> actionOnException = null)
        {
            var activatorExitCode = -1;
            try
            {
                var fileName = Path.GetFileName(_activatorAppPath);
                var directory = Path.GetDirectoryName(_activatorAppPath);
                var activatorApp = new Process
                {
                    StartInfo = {
                        FileName = fileName ?? string.Empty,
                        WorkingDirectory = directory ?? string.Empty,
                        UseShellExecute = true,
                        Arguments = $"--product {_apiClient.GetProductName()}"
                    }
                };
                activatorApp.Start();
                activatorApp.WaitForExit();
                activatorExitCode = activatorApp.ExitCode;
            }
            catch (Exception e)
            {
                actionOnException?.Invoke(e);
            }
            finally
            {
                if (activatorExitCode != ActivatorSuccessExitCode)
                {
                    // Exit from client app
                    Environment.Exit(-1);
                }
            }
        }
    }
}