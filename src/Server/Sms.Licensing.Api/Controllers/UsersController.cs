using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Sms.Licensing.Api.Authorization;
using Sms.Licensing.Api.Extensions;
using Sms.Licensing.Domain.Entities;
using Sms.Licensing.Domain.Repositories.Abstractions;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = Policies.IsAdmin)]
    [Route("v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IActivationRequestRepository _activationRequestRepository;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public UsersController(UserManager<ApplicationUser> userManager,
            ILicenseRepository licenseRepository,
            IActivationRequestRepository activationRequestRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _licenseRepository = licenseRepository;
            _activationRequestRepository = activationRequestRepository;
            _configuration = configuration;
        }

        #endregion

        #region GET methods

        // GET v1/users
        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/users
        /// 
        /// </remarks>
        /// <returns>List of User DTO schemes</returns>
        /// <response code="200">Successful API response</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var usersDtoList = new List<UserDto>();
            var usersDb = await _userManager.Users.ToListAsync();

            foreach (var userEntity in usersDb)
            {
                var userDto = await ConvertToUserDtoAsync(userEntity);
                usersDtoList.Add(userDto);
            }

            return Ok(new ApiResponse<IEnumerable<UserDto>>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned list of users",
                Data = usersDtoList
            });
        }

        // GET v1/users/{id}
        /// <summary>
        /// Get user which have specified ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/users/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        /// 
        /// </remarks>
        /// <param name="id">License ID</param>
        /// <returns>User DTO scheme</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="400">Bad request API response which indicates user could not found with specified UserID</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userEntity = await _userManager.FindByIdAsync(id);

            if (userEntity == null)
            {
                return BadRequest(new ApiResponse<UserDto>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {id}",
                    Data = null
                });
            }
            
            var userDto = await ConvertToUserDtoAsync(userEntity);

            return Ok(new ApiResponse<UserDto>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned user data",
                Data = userDto
            });
        }

        // GET: v1/users/inRole/{roleName}
        /// <summary>
        /// Get users who have specified role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/users/inRole/Client
        /// 
        /// </remarks>
        /// <param name="roleName">Role name</param>
        /// <returns>User DTO scheme</returns>
        /// <response code="200">Successful API response</response>
        [HttpGet("inRole/{roleName}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            var usersDtoList = new List<UserDto>();
            foreach (var userEntity in users)
            {
                var userDto = await ConvertToUserDtoAsync(userEntity);
                usersDtoList.Add(userDto);
            }

            return Ok(new ApiResponse<IEnumerable<UserDto>>()
            {
                Status = ApiResponseStatus.Success,
                Message = $"Returned list of users who have {roleName} role",
                Data = usersDtoList
            });
        }

        #endregion

        #region POST methods

        // POST v1/users
        /// <summary>
        /// Add new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/users
        ///     {
        ///         UserName = "TestUser",
        ///         Email = "TestUser@gmail.com",
        ///         Password = "TestPassword1"
        ///     }
        /// 
        /// </remarks>
        /// <param name="userDto">User DTO scheme</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="409">Conflict status code response which indicates user already have with same UserName or Email</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddUser([FromBody] UserDto userDto)  
        {  
            var userExists = await _userManager.FindUserAsync(userDto);
            if (userExists != null)
                return StatusCode(StatusCodes.Status409Conflict, 
                    new ApiResponse 
                    {
                        Status = ApiResponseStatus.Error,
                        Message = "User already exists!"
                    });  
  
            var user = new ApplicationUser()  
            {  
                Id = userDto.Id,
                Email = userDto.Email,
                UserName = userDto.UserName,
                Timestamp = userDto.RegistrationTime
            };
            
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                var errorsMessage = new StringBuilder();
                foreach (var identityError in result.Errors)
                {
                    errorsMessage.AppendLine(identityError.Description);
                }

                return StatusCode(StatusCodes.Status409Conflict,
                    new ApiResponse 
                    { 
                        Status = ApiResponseStatus.Error, 
                        Message = errorsMessage.ToString()
                    });
            }

            return Ok(new ApiResponse 
            { 
                Status = ApiResponseStatus.Success, 
                Message = $"User {userDto.UserName} created successfully!"
            });  
        }

        #endregion

        #region PUT methods

        // PUT v1/user/{id}
        /// <summary>
        /// Update existing user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /v1/users/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        ///     {
        ///         UserName = "TestUser",
        ///         Email = "TestUser@gmail.com"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">User ID</param>
        /// <param name="userDto">User DTO scheme</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="400">Bad request API response which indicates user could not found with specified ID</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
        {
            var userEntity = await _userManager.FindByIdAsync(id);

            if (userEntity == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {id}"
                });
            }

            if (!string.Equals(userEntity.Email.Trim(), userDto.Email.Trim(), 
                StringComparison.CurrentCultureIgnoreCase))
            {
                await _userManager.SetEmailAsync(userEntity, userDto.Email);
            }
            
            if (!string.Equals(userEntity.UserName.Trim(), userDto.UserName.Trim(), 
                StringComparison.CurrentCultureIgnoreCase))
            {
                await _userManager.SetUserNameAsync(userEntity, userDto.UserName);
            }

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "User updated successfully"
            });
        }

        #endregion

        #region DELETE methods

        // DELETE v1/users/{id}
        /// <summary>
        /// Delete existing user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /v1/licenses/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        /// 
        /// </remarks>
        /// <param name="id">User ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userEntity = await _userManager.FindByIdAsync(id);

            if (userEntity != null)
            {
                // Set all references to DELETED_USER
                var referencedLicenses = await _licenseRepository.GetListAsync(i => i.OwnerId == userEntity.Id);
                var referencedActivationRequests = await _activationRequestRepository.GetListAsync(i => i.RequestedClientId == userEntity.Id);
                var deletedUser = await _userManager.FindByNameAsync(_configuration["ServerAccounts:DeletedUser:UserName"]);

                if (deletedUser != null)
                {
                    foreach (var referenceLicense in referencedLicenses)
                    {
                        referenceLicense.Owner = deletedUser;
                        await _licenseRepository.UpdateAsync(referenceLicense);
                    }

                    foreach (var referencedActivationRequest in referencedActivationRequests)
                    {
                        referencedActivationRequest.RequestedClient = deletedUser;
                        await _activationRequestRepository.UpdateAsync(referencedActivationRequest);
                    }
                }

                await _userManager.DeleteAsync(userEntity);
            }

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "User deleted successfully"
            });
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts ApplicationUser to UserDto class
        /// </summary>
        /// <param name="userEntity">User entity object</param>
        /// <returns>User DTO object</returns>
        private async Task<UserDto> ConvertToUserDtoAsync(ApplicationUser userEntity)
        {
            var userRoles = (await _userManager.GetRolesAsync(userEntity)).ToArray();
            var userDto = new UserDto()
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName,
                Email = userEntity.Email,
                UserRoles = userRoles,
                RegistrationTime = userEntity.Timestamp
            };

            return userDto;
        }

        #endregion
    }
}
