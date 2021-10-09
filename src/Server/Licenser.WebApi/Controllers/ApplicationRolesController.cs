using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Licenser.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licenser.Domain.Entities;
using Licenser.Shared.Models;

namespace Licenser.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = Policies.IsAdmin)]
    [Route("v{version:apiVersion}/roles")]
    public class ApplicationRolesController : ControllerBase
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        #endregion

        #region Constructor

        public ApplicationRolesController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        #region GET methods

        // GET: v1/roles
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/roles
        /// 
        /// </remarks>
        /// <returns>List of Role DTO schemes</returns>
        /// <response code="200">Successful API response</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var rolesDb = await _roleManager.Roles.ToListAsync();
            var rolesDtoList = rolesDb.Select(roleEntity => new RoleDto()
            {
                Id = roleEntity.Id, 
                Name = roleEntity.Name, 
                Description = roleEntity.Description
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<RoleDto>>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned list of roles",
                Data = rolesDtoList
            });
        }

        #endregion

        #region PUT methods

        // PUT: v1/roles/{id}
        /// <summary>
        /// Update existing role description
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /v1/roles/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        ///     {
        ///         Description: "Role description"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">Role ID</param>
        /// <param name="roleDto">Role DTO scheme</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="400">Bad request API response which indicates license could not found with specified ID</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AccessToken>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateApplicationRole(string id, [FromBody] RoleDto roleDto)
        {
            var roleEntity = await _roleManager.FindByIdAsync(id);

            if (roleEntity == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found role with ID {id}"
                });
            }

            roleEntity.Description = roleDto.Description;
            await _roleManager.UpdateAsync(roleEntity);

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "Role updated successfully"
            });
        }

        #endregion

        #region POST methods

        // POST v1/roles/assign/{roleName}
        /// <summary>
        /// Add new license
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/roles/assign/Admin
        ///     {
        ///         UserId: "ab123a45-6d78-9030-af00-0e0c0c0fdaa0"
        ///     }
        /// 
        /// </remarks>
        /// <param name="roleName">Role name</param>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="302">User already have specified role</response>
        /// <response code="409">Conflict status code response which indicates user could not found with specified UserID</response>
        /// <response code="501">Could not found specified role in server database</response>
        [HttpPost("assign/{roleName}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> AssignRole(string roleName, [FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {userId}"
                });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found {roleName} role"
                });
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, roleName);

                return Ok(new ApiResponse 
                { 
                    Status = ApiResponseStatus.Success, 
                    Message = $"Added {roleName} role to user {user.UserName}"
                });
            }

            return StatusCode(StatusCodes.Status302Found, new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = $"User {user.UserName} already have {roleName} role"
            });
        }

        // POST v1/roles/removeFromUser/{roleName}
        /// <summary>
        /// Remove specified role from specified User
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/roles/removeFromUser/Admin
        ///     {
        ///         UserId: "ab123a45-6d78-9030-af00-0e0c0c0fdaa0"
        ///     }
        /// 
        /// </remarks>
        /// <param name="roleName">Role name</param>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="202">User do not have specified role</response>
        /// <response code="501">Could not found specified role in server database</response>
        [HttpPost("removeFromUser/{roleName}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> RemoveRoleFromUser(string roleName, [FromBody] string userId)
        {
            var userEntity = await _userManager.FindByIdAsync(userId);

            if (userEntity == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {userId}"
                });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found {roleName} role"
                });
            }

            var isInRole = await _userManager.IsInRoleAsync(userEntity, roleName);
            if (!isInRole)
            {
                return StatusCode(StatusCodes.Status202Accepted, new ApiResponse()
                {
                    Status = ApiResponseStatus.Success,
                    Message = $"User {userEntity.UserName} do not have {roleName} role"
                });
            }

            await _userManager.RemoveFromRoleAsync(userEntity, roleName);
            return Ok(new ApiResponse 
            { 
                Status = ApiResponseStatus.Success, 
                Message = $"Removed {roleName} role from user {userEntity.UserName}"
            });
        }

        // POST v1/roles/removeAllFromUser
        /// <summary>
        /// Remove all roles from specified User
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/roles/removeAllFromUser
        ///     {
        ///         UserId: "ab123a45-6d78-9030-af00-0e0c0c0fdaa0"
        ///     }
        /// 
        /// </remarks>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        [HttpPost("removeAllFromUser")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveAllRolesFromUser([FromBody] string userId)
        {
            var userEntity = await _userManager.FindByIdAsync(userId);

            if (userEntity == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {userId}"
                });
            }

            var previousRoles = await _userManager.GetRolesAsync(userEntity);
            await _userManager.RemoveFromRolesAsync(userEntity, previousRoles);
            
            return Ok(new ApiResponse 
            { 
                Status = ApiResponseStatus.Success, 
                Message = $"Removed all roles from user {userEntity.UserName}"
            });
        }

        #endregion
    }
}
