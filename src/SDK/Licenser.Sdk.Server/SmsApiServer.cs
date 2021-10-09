using System.Collections.Generic;
using System.Threading.Tasks;
using Licenser.Sdk.Client;
using Licenser.Sdk.Server.Abstractions;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Server
{
    /// <summary>
    /// SmsApiServer is main class for interacting with SMS RESTful API services.
    /// </summary>
    public class SmsApiServer : SmsApiAuthClient, ISmsApiServer
    {
        #region Constructor

        /// <summary>
        /// Constructor for SmsApiServer class
        /// </summary>
        /// <param name="clientOptions">Server settings</param>
        /// <param name="apiVersion">SMS API version</param>
        public SmsApiServer(SmsApiClientOptions clientOptions, ApiVersion apiVersion = ApiVersion.V1) : base(clientOptions, apiVersion)
        {
        }

        #endregion

        #region Methods

        #region License methods

        /// <inheritdoc/>
        public async Task<ApiResponse<LicenseDto>> GetLicenseAsync(string id)
        {
            var apiResponse = await GetRequestAsync<LicenseDto>($"licenses/{id}");
            return apiResponse;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<LicenseDto>>> GetLicensesAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<LicenseDto>>("licenses");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> AddLicenseAsync(LicenseDto license)
        {
            return PostRequestAsync("licenses", license);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateLicenseAsync(LicenseDto license)
        {
            return PutRequestAsync($"licenses/{license.Id}", license);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteLicenseAsync(LicenseDto license)
        {
            return DeleteRequestAsync($"licenses/{license.Id}");
        }

        #endregion

        #region ActivationRequest methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<ActivationRequestDto>>> GetActivationRequestsAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<ActivationRequestDto>>("licenses/activationRequests");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteActivationRequestAsync(ActivationRequestDto activationRequest)
        {
            return DeleteRequestAsync($"licenses/activationRequest/{activationRequest.Id}");
        }

        #endregion

        #region User methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<UserDto>>("users");
            return apiResponse;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsersInRoleAsync(string roleName)
        {
            var apiResponse = await GetRequestAsync<IEnumerable<UserDto>>($"users/inRole/{roleName}");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> AddUserAsync(UserDto user)
        {
            return PostRequestAsync("users", user);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateUserAsync(UserDto user)
        {
            return PutRequestAsync($"users/{user.Id}", user);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteUserAsync(UserDto user)
        {
            return DeleteRequestAsync($"users/{user.Id}");
        }

        #endregion

        #region ApplicationRole methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<RoleDto>>("roles");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateRoleAsync(RoleDto roleDto)
        {
            return PutRequestAsync($"roles/{roleDto.Id}", roleDto);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> AssignRoleAsync(string roleName, string userId)
        {
            return PostRequestAsync($"roles/assign/{roleName}", userId);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> RemoveRoleFromUserAsync(string roleName, string userId)
        {
            return PostRequestAsync($"roles/removeFromUser/{roleName}", userId);
        }
        
        /// <inheritdoc/>
        public Task<ApiResponse> RemoveAllRolesFromUserAsync(string userId)
        {
            return PostRequestAsync("roles/removeAllFromUser", userId);
        }

        #endregion

        #endregion
    }
}
