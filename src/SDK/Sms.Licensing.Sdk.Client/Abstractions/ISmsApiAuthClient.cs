using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Sdk.Client.Abstractions
{
    /// <summary>
    /// SMS Api authentication client interface
    /// </summary>
    public interface ISmsApiAuthClient
    {
        #region Authentication methods

        /// <summary>
        /// Checks client already authorized with JWT
        /// </summary>
        /// <returns>True if client authorized</returns>
        bool IsAuthenticated();

        /// <summary>
        /// Sets an authorization header with a bearer token.
        /// </summary>
        /// <param name="accessToken"></param>
        void SetAccessToken(string accessToken);

        /// <summary>
        /// Authenticates user using its username and password (grant type is 'password').
        /// </summary>
        /// <param name="userCredentials">User credentials (username and password)</param>
        /// <exception cref="ArgumentNullException">Throws if argument of userCredentials is null</exception>
        /// <returns>Get access token</returns>
        Task<ApiResponse<TokenResponse>> AuthenticatePasswordAsync(UserAdvancedCredentials userCredentials);

        /// <summary>
        /// Renew access token after expired lifetime using refresh token.
        /// </summary>
        /// <returns>Get access token</returns>
        Task<ApiResponse<TokenResponse>> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoke access token.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<ApiResponse<TokenRevocationResponse>> RevokeToken(string accessToken);

        #endregion
    }
}