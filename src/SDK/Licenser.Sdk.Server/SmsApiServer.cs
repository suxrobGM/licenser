using System.Collections.Generic;
using System.Threading.Tasks;
using Licenser.Sdk.Client;
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
        public async Task<ApiResponse<License>> GetLicenseAsync(string id)
        {
            var apiResponse = await GetRequestAsync<License>($"licenses/{id}");
            return apiResponse;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<License>>> GetLicensesAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<License>>("licenses");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> AddLicenseAsync(License license)
        {
            return PostRequestAsync("licenses", license);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateLicenseAsync(License license)
        {
            return PutRequestAsync($"licenses/{license.Id}", license);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteLicenseAsync(License license)
        {
            return DeleteRequestAsync($"licenses/{license.Id}");
        }

        #endregion

        #region ActivationRequest methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<ActivationRequest>>> GetActivationRequestsAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<ActivationRequest>>("licenses/activationRequests");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteActivationRequestAsync(ActivationRequest activationRequest)
        {
            return DeleteRequestAsync($"licenses/activationRequest/{activationRequest.Id}");
        }

        #endregion

        #region User methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<User>>> GetUsersAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<User>>("users");
            return apiResponse;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<User>>> GetUsersInRoleAsync(string roleName)
        {
            var apiResponse = await GetRequestAsync<IEnumerable<User>>($"users/inRole/{roleName}");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> AddUserAsync(User user)
        {
            return PostRequestAsync("users", user);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateUserAsync(User user)
        {
            return PutRequestAsync($"users/{user.Id}", user);
        }

        /// <inheritdoc/>
        public Task<ApiResponse> DeleteUserAsync(User user)
        {
            return DeleteRequestAsync($"users/{user.Id}");
        }

        #endregion

        #region ApplicationRole methods

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Role>>> GetRolesAsync()
        {
            var apiResponse = await GetRequestAsync<IEnumerable<Role>>("roles");
            return apiResponse;
        }

        /// <inheritdoc/>
        public Task<ApiResponse> UpdateRoleAsync(Role role)
        {
            return PutRequestAsync($"roles/{role.Id}", role);
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
