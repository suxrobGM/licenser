using System.Threading.Tasks;
using Licenser.Server.Domain.Entities;
using Licenser.Shared.Models;
using ActivationRequest = Licenser.Shared.Models.ActivationRequest;
using License = Licenser.Shared.Models.License;

namespace Licenser.Server.Domain.Repositories.Abstractions
{
    public interface IActivationRequestRepository : IRepository<Entities.ActivationRequest>
    {
        /// <summary>
        /// Get activation request by comparing license's MachineId, OwnerId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>Activation request entity type</returns>
        Task<Entities.ActivationRequest> GetAsync(License license);

        /// <summary>
        /// Get activation request by comparing its ActivationId, RequestedClientId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="activationRequest">Activation request DTO type</param>
        /// <returns>Activation request entity type</returns>
        Task<Entities.ActivationRequest> GetAsync(ActivationRequest activationRequest);

        /// <summary>
        /// Creates new activation request if already request have not created.
        /// </summary>
        /// <param name="activationRequest">Activation request DTO type</param>
        /// <returns>Status of activation request</returns>
        Task<ActivationRequestStatus> CreateActivationRequest(ActivationRequest activationRequest);
    }
}