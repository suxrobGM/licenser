using System.Threading.Tasks;
using Sms.Licensing.Domain.Entities;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Domain.Repositories.Abstractions
{
    public interface IActivationRequestRepository : IRepository<ActivationRequest>
    {
        /// <summary>
        /// Get activation request by comparing license's MachineId, OwnerId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="licenseDto">License DTO type</param>
        /// <returns>Activation request entity type</returns>
        Task<ActivationRequest> GetAsync(LicenseDto licenseDto);

        /// <summary>
        /// Get activation request by comparing its ActivationId, RequestedClientId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="activationRequestDto">Activation request DTO type</param>
        /// <returns>Activation request entity type</returns>
        Task<ActivationRequest> GetAsync(ActivationRequestDto activationRequestDto);

        /// <summary>
        /// Creates new activation request if already request have not created.
        /// </summary>
        /// <param name="activationRequestDto">Activation request DTO type</param>
        /// <returns>Status of activation request</returns>
        Task<ActivationRequestStatus> CreateActivationRequest(ActivationRequestDto activationRequestDto);
    }
}