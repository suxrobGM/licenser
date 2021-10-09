using System.Collections.Generic;
using System.Threading.Tasks;
using Licenser.Sdk.Client.Abstractions;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Server.Abstractions
{
    public interface ISmsApiServer : ISmsApiAuthClient
    {
        #region License CRUD methods

        /// <summary>
        /// Get license by ID from API server
        /// </summary>
        /// <param name="id">License ID</param>
        /// <returns>Licenses</returns>
        Task<ApiResponse<LicenseDto>> GetLicenseAsync(string id);

        /// <summary>
        /// Get all licenses from API server
        /// </summary>
        /// <returns>Licenses</returns>
        Task<ApiResponse<IEnumerable<LicenseDto>>> GetLicensesAsync();

        /// <summary>
        /// Add new license item to API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> AddLicenseAsync(LicenseDto license);

        /// <summary>
        /// Update existing license item to API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateLicenseAsync(LicenseDto license);

        /// <summary>
        /// Delete license item from API server
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> DeleteLicenseAsync(LicenseDto license);

        #endregion

        #region ActivationRequest methods

        /// <summary>
        /// Get list of activation requests.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<ActivationRequestDto>>> GetActivationRequestsAsync();

        /// <summary>
        /// Delete activation request.
        /// </summary>
        /// <param name="activationRequest"></param>
        /// <returns></returns>
        Task<ApiResponse> DeleteActivationRequestAsync(ActivationRequestDto activationRequest);

        #endregion

        #region User CRUD methods

        /// <summary>
        /// Get all user from API server
        /// </summary>
        /// <returns>Users</returns>
        Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync();

        /// <summary>
        /// Get users who have specified role
        /// </summary>
        /// <returns>Users</returns>
        Task<ApiResponse<IEnumerable<UserDto>>> GetUsersInRoleAsync(string roleName);

        /// <summary>
        /// Add new user to API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> AddUserAsync(UserDto user);

        /// <summary>
        /// Update existing user to API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateUserAsync(UserDto user);

        /// <summary>
        /// Delete user from API server
        /// </summary>
        /// <param name="user">User DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> DeleteUserAsync(UserDto user);

        #endregion

        #region ApplicationRole methods

        /// <summary>
        /// Get all roles from API server
        /// </summary>
        /// <returns>Roles</returns>
        Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync();

        /// <summary>
        /// Update existing role to API server
        /// </summary>
        /// <param name="roleDto">Role DTO type</param>
        /// <returns>ApiResponse which indicates response status</returns>
        Task<ApiResponse> UpdateRoleAsync(RoleDto roleDto);

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