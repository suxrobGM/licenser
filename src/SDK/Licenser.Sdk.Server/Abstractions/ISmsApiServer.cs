using System.Collections.Generic;
using System.Threading.Tasks;
using Licenser.Sdk.Client;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Server
{
    public interface ISmsApiServer : ISmsApiAuthClient
    {
        #region License CRUD methods

        /// <summary>
        /// Get license by ID from API server
        /// </summary>
        /// <param name="id">License ID</param>
        /// <returns>Licenses</returns>
        Task<ApiResponse<License>> GetLicenseAsync(string id);

        /// <summary>
        /// Get all licenses from API server
        /// </summary>
        /// <returns>Licenses</returns>
        Task<ApiResponse<IEnumerable<License>>> GetLicensesAsync();

        /// <summary>
        /// Add new license item to API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> AddLicenseAsync(License license);

        /// <summary>
        /// Update existing license item to API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateLicenseAsync(License license);

        /// <summary>
        /// Delete license item from API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> DeleteLicenseAsync(License license);

        #endregion

        #region ActivationRequest methods

        /// <summary>
        /// Get list of activation requests.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ActivationRequest>>> GetActivationRequestsAsync();

        /// <summary>
        /// Delete activation request.
        /// </summary>
        /// <param name="activationRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> DeleteActivationRequestAsync(ActivationRequest activationRequest);

        #endregion

        #region User CRUD methods

        /// <summary>
        /// Get all user from API server
        /// </summary>
        /// <returns>Users</returns>
        Task<ApiResponse<IEnumerable<User>>> GetUsersAsync();

        /// <summary>
        /// Get users who have specified role
        /// </summary>
        /// <returns>Users</returns>
        Task<ApiResponse<IEnumerable<User>>> GetUsersInRoleAsync(string roleName);

        /// <summary>
        /// Add new user to API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> AddUserAsync(User user);

        /// <summary>
        /// Update existing user to API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateUserAsync(User user);

        /// <summary>
        /// Delete user from API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> DeleteUserAsync(User user);

        #endregion

        #region ApplicationRole methods

        /// <summary>
        /// Get all roles from API server
        /// </summary>
        /// <returns>Roles</returns>
        Task<ApiResponse<IEnumerable<Role>>> GetRolesAsync();

        /// <summary>
        /// Update existing role to API server
        /// </summary>
        /// <param name="role">Role DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateRoleAsync(Role role);

        /// <summary>
        /// Assign specified role to specified user
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> AssignRoleAsync(string roleName, string userId);

        /// <summary>
        /// Remove specified role from specified user
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> RemoveRoleFromUserAsync(string roleName, string userId);

        /// <summary>
        /// Remove all roles from specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> RemoveAllRolesFromUserAsync(string userId);

        #endregion
    }
}