using System;
using System.Threading.Tasks;
using Sms.Licensing.Sdk.Client.Abstractions;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Sdk.Client
{
    /// <summary>
    /// SmsApiClient is main class for interacting with SMS RESTful API services.
    /// </summary>
    public class SmsApiClient : SmsApiAuthClient, ISmsApiClient
    {
        private readonly SmsApiClientOptions _clientOptions;

        #region Constructor

        /// <summary>
        /// Constructor for SmsApiClient class
        /// </summary>
        /// <param name="clientOptions">Client settings</param>
        /// <param name="apiVersion">SMS API version</param>
        /// <exception cref="ArgumentNullException">Throws if SmsApiClientOptions is null</exception>
        public SmsApiClient(SmsApiClientOptions clientOptions, ApiVersion apiVersion = ApiVersion.V1) : base(clientOptions, apiVersion)
        {
            _clientOptions = clientOptions;
        }

        #endregion

        #region Methods

        #region License methods

        /// <inheritdoc/>
        public async Task<ApiResponse<LicenseStatus>> CheckLicenseAsync()
        {
            var apiResponse = new ApiResponse<LicenseStatus>();

            if (string.IsNullOrWhiteSpace(_clientOptions.ProductName))
            {
                apiResponse.Status = ApiResponseStatus.Error;
                apiResponse.Message = "Product name is empty";
                return apiResponse;
            }

            var license = new LicenseDto()
            {
                MachineId = KeyGenerator.GetMachineId(),
                OwnerId = _userCredentials.Id,
                ProductName = _clientOptions.ProductName
            };

            apiResponse = await PostRequestAsync<LicenseStatus, LicenseDto>("licenses/check", license);
            return apiResponse;
        }

        #endregion

        #region ActivationRequest methods

        /// <inheritdoc/>
        public async Task<ApiResponse<ActivationRequestStatus>> CreateActivationRequestAsync()
        {
            var apiResponse = new ApiResponse<ActivationRequestStatus>();

            if (string.IsNullOrWhiteSpace(_clientOptions.ProductName))
            {
                apiResponse.Status = ApiResponseStatus.Error;
                apiResponse.Message = "Product name is empty";
                return apiResponse;
            }

            var activationRequest = new ActivationRequestDto()
            {
                ActivationId = KeyGenerator.GetMachineId(),
                RequestedClientId = _userCredentials.Id,
                RequestedClientUserName = _userCredentials.UserName,
                ProductName = _clientOptions.ProductName
            };
            apiResponse = await PostRequestAsync<ActivationRequestStatus, ActivationRequestDto>("licenses/sendActivationRequest", activationRequest);
            return apiResponse;
        }

        /// <inheritdoc/>
        public string GetProductName()
        {
            return _clientOptions.ProductName;
        }

        #endregion

        #endregion
    }
}
