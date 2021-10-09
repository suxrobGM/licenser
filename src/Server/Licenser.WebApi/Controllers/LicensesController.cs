using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Licenser.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Licenser.WebApi.Extensions;
using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Repositories.Abstractions;
using Licenser.Shared.Models;

namespace Licenser.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class LicensesController : ControllerBase
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IActivationRequestRepository _activationRequestRepository;

        #endregion

        #region Constructor

        public LicensesController(UserManager<ApplicationUser> userManager,
            ILicenseRepository licenseRepository,
            IActivationRequestRepository activationRequestRepository)
        {
            _userManager = userManager;
            _licenseRepository = licenseRepository;
            _activationRequestRepository = activationRequestRepository;
        }

        #endregion

        #region GET methods

        // GET v1/licenses
        /// <summary>
        /// Get all licenses
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/licenses
        /// 
        /// </remarks>
        /// <returns>List of License DTO schemes</returns>
        /// <response code="200">Successful API response</response>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LicenseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLicenses()
        {
            var licensesDb = await _licenseRepository.GetListAsync();
            var licensesDtoList = licensesDb.Select(licenseEntity => new LicenseDto()
            {
                Id = licenseEntity.Id,
                MachineId = licenseEntity.MachineId,
                ExpiryDate = licenseEntity.ExpiryDate,
                IssueDate = licenseEntity.IssueDate,
                ObjectName = licenseEntity.ObjectName,
                ProductName = licenseEntity.ProductName,
                OwnerId = licenseEntity.Owner?.Id,
                OwnerUserName = licenseEntity.Owner?.UserName,
                OwnerEmail = licenseEntity.Owner?.Email,
            });

            return Ok(new ApiResponse<IEnumerable<LicenseDto>>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned list of licenses",
                Data = licensesDtoList
            });
        }

        // GET v1/licenses/{id}
        /// <summary>
        /// Get license which have specified ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/licenses/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        /// 
        /// </remarks>
        /// <param name="id">License ID</param>
        /// <returns>License DTO scheme</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="400">Bad request API response which indicates license could not found with specified ID</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ProducesResponseType(typeof(ApiResponse<LicenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LicenseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLicense(string id)
        {
            var licenseEntity = await _licenseRepository.GetByIdAsync(id);

            if (licenseEntity == null)
            {
                return BadRequest(new ApiResponse<LicenseDto>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found user with ID {id}",
                    Data = null
                });
            }

            var license = new LicenseDto()
            {
                Id = licenseEntity.Id,
                MachineId = licenseEntity.MachineId,
                IssueDate = licenseEntity.IssueDate,
                ExpiryDate = licenseEntity.ExpiryDate,
                ObjectName = licenseEntity.ObjectName,
                ProductName = licenseEntity.ProductName,
                OwnerId = licenseEntity.Owner?.Id,
                OwnerUserName = licenseEntity.Owner?.UserName,
                OwnerEmail = licenseEntity.Owner?.Email
            };

            return Ok(new ApiResponse<LicenseDto>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned license data",
                Data = license
            });
        }

        // GET v1/licenses/activationRequests
        /// <summary>
        /// Get all activation requests
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /v1/licenses/activationRequests
        /// 
        /// </remarks>
        /// <returns>List of ActivationRequest DTO schemes</returns>
        /// <response code="200">Successful API response</response>
        [HttpGet("activationRequests")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ActivationRequestDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivationRequests()
        {
            var activationRequestsDb = await _activationRequestRepository.GetListAsync();

            var activationRequestDtoList = activationRequestsDb.Select(activationRequestEntity => new ActivationRequestDto()
            {
                Id = activationRequestEntity.Id, 
                ActivationId = activationRequestEntity.ActivationId,
                RequestedClientId = activationRequestEntity.RequestedClientId,
                RequestedClientUserName = activationRequestEntity.RequestedClient.UserName,
                ProductName = activationRequestEntity.ProductName,
                Timestamp = activationRequestEntity.Timestamp
            });
            
            return Ok(new ApiResponse<IEnumerable<ActivationRequestDto>>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Returned list of activation requests",
                Data = activationRequestDtoList
            });
        }

        #endregion

        #region POST methods

        // POST v1/licenses
        /// <summary>
        /// Add new license
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/licenses
        ///     {
        ///         MachineId = "Machine ID",
        ///         OwnerId = "ab123a45-6d78-9030-af00-0e0c0c0fdaa0",
        ///         ExpiryDate = "08/10/2021"
        ///     }
        /// 
        /// </remarks>
        /// <param name="license">License DTO scheme</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="409">Conflict status code response which indicates license owner could not found with specified UserID</response>
        [HttpPost]
        [Authorize(Policy = Policies.IsAdmin)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddLicense([FromBody] LicenseDto license)
        {
            var user = await _userManager.FindUserAsync(new UserAdvancedCredentials()
            {
                Id = license.OwnerId,
                UserName = license.OwnerUserName,
                Email = license.OwnerEmail
            });

            if (user == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "License owner could not found"
                });
            }

            license.OwnerId = user.Id;
            license.OwnerUserName = user.UserName;
            license.OwnerEmail = user.Email;
            await _licenseRepository.AddLicenseAsync(license);

            // Delete activation request if exists
            var existingActivationRequest = await _activationRequestRepository.GetAsync(license);
            if (existingActivationRequest != null)
            {
                await _activationRequestRepository.DeleteAsync(existingActivationRequest);
            }

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "License created successfully"
            });
        }

        // POST v1/licenses/check
        /// <summary>
        /// Check existing license status
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/licenses/check
        ///     {
        ///         MachineId = "Machine ID",
        ///         OwnerId = "ab123a45-6d78-9030-af00-0e0c0c0fdaa0"
        ///     }
        /// 
        /// </remarks>
        /// <param name="license">License DTO scheme</param>
        /// <returns>ApiResponse of LicenseStatus enum type which indicates license status</returns>
        /// <response code="200">Successful API response</response>
        [HttpPost("check")]
        [Authorize(Policy = Policies.IsClient)]
        [ProducesResponseType(typeof(ApiResponse<LicenseStatus>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckLicense([FromBody] LicenseDto license)
        {
            var licenseStatus = await _licenseRepository.CheckLicenseAsync(license);
            return Ok(new ApiResponse<LicenseStatus>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Checked license status",
                Data = licenseStatus
            });
        }

        // POST v1/licenses/sendActivationRequest
        /// <summary>
        /// Send activation request to management server
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /v1/licenses/sendActivationRequest
        ///     {
        ///         ActivationId = "Machine ID",
        ///         RequestedClientId = "ab123a45-6d78-9030-af00-0e0c0c0fdaa0"
        ///     }
        /// 
        /// </remarks>
        /// <param name="activationRequest">ActivationRequest DTO scheme</param>
        /// <returns>ApiResponse of ActivationRequestStatus enum type which indicates request status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="409">Conflict status code response which indicates requested client could not found</response>
        [HttpPost("sendActivationRequest")]
        [Authorize(Policy = Policies.IsClient)]
        [ProducesResponseType(typeof(ApiResponse<ActivationRequestStatus>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendActivationRequest([FromBody] ActivationRequestDto activationRequest)
        {
            var clientCredentials = new UserAdvancedCredentials()
            {
                Id = activationRequest.RequestedClientId,
                UserName = activationRequest.RequestedClientUserName
            };

            var user = await _userManager.FindUserAsync(clientCredentials);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Requested client could not found"
                });
            }

            activationRequest.RequestedClientId = user.Id;
            activationRequest.RequestedClientUserName = user.UserName;

            var requestStatus = await _activationRequestRepository.CreateActivationRequest(activationRequest);
            return Ok(new ApiResponse<ActivationRequestStatus>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Activation request status",
                Data = requestStatus
            });
        }

        #endregion

        #region PUT methods

        // PUT v1/licenses/{id}
        /// <summary>
        /// Update existing license item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /v1/licenses/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        ///     {
        ///         MachineId = "Machine ID",
        ///         OwnerId = "ab123a45-6d78-9030-af00-0e0c0c0fdaa0",
        ///         ExpiryDate = "08/10/2021"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">License ID</param>
        /// <param name="license">License DTO scheme</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        /// <response code="400">Bad request API response which indicates license could not found with specified ID</response>
        /// /// <response code="409">Conflict status code response which indicates requested client could not found</response>
        [HttpPut("{id}")]
        [Authorize(Policy = Policies.IsAdmin)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLicense(string id, [FromBody] LicenseDto license)
        {
            var licenseEntity = await _licenseRepository.GetByIdAsync(id);

            var user = await _userManager.FindUserAsync(new UserAdvancedCredentials()
            {
                Id = license.OwnerId,
                UserName = license.OwnerUserName,
                Email = license.OwnerEmail
            });

            if (user == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "License owner could not found"
                });
            }

            if (licenseEntity == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Could not found license with ID {id}"
                });
            }

            licenseEntity.MachineId = license.MachineId;
            licenseEntity.ExpiryDate = license.ExpiryDate;
            licenseEntity.ObjectName = license.ObjectName;
            licenseEntity.ProductName = license.ProductName;
            licenseEntity.Owner = user;
            await _licenseRepository.UpdateAsync(licenseEntity);

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "License updated successfully"
            });
        }

        #endregion

        #region DELETE methods

        // DELETE v1/licenses/{id}
        /// <summary>
        /// Delete existing license item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /v1/licenses/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        /// 
        /// </remarks>
        /// <param name="id">License ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.IsAdmin)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLicense(string id)
        {
            await _licenseRepository.DeleteByIdAsync(id);

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "License deleted successfully"
            });
        }

        // DELETE v1/licenses/activationRequest/{id}
        /// <summary>
        /// Delete existing activation request
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /v1/licenses/activationRequest/ab123a45-6d78-9030-af00-0e0c0c0fdaa0
        /// 
        /// </remarks>
        /// <param name="id">ActivationRequest ID</param>
        /// <returns>ApiResponse which indicates response status</returns>
        /// <response code="200">Successful API response</response>
        [HttpDelete("activationRequest/{id}")]
        [Authorize(Policy = Policies.IsAdmin)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteActivationRequest(string id)
        {
            await _activationRequestRepository.DeleteByIdAsync(id);

            return Ok(new ApiResponse()
            {
                Status = ApiResponseStatus.Success,
                Message = "Activation request deleted successfully"
            });
        }

        #endregion
    }
}
