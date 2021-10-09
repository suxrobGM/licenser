using System.Threading.Tasks;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Client
{
    public interface ISmsApiClient : ISmsApiAuthClient
    {
        #region License methods

        /// <summary>
        /// Check client's license status.
        /// </summary>
        /// <returns>Invalid if license does not exist for current machine, Expire if license has already expired, Valid if license valid and has not expired.</returns>
        Task<ApiResponse<LicenseStatus>> CheckLicenseAsync();

        #endregion

        #region ActivationRequest methods

        /// <summary>
        /// Send activation request.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<ActivationRequestStatus>> CreateActivationRequestAsync();

        /// <summary>
        /// Gets client's software product name.
        /// </summary>
        /// <returns>Product name</returns>
        string GetProductName();

        #endregion
    }
}