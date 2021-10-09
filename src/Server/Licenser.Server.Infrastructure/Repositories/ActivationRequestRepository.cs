using System.Threading.Tasks;
using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Repositories.Abstractions;
using Licenser.Server.Infrastructure.Data;
using Licenser.Shared.Models;
using ActivationRequest = Licenser.Shared.Models.ActivationRequest;
using License = Licenser.Shared.Models.License;

namespace Licenser.Server.Infrastructure.Repositories
{
    public class ActivationRequestRepository : Repository<Domain.Entities.ActivationRequest>, IActivationRequestRepository
    {
        private readonly ILicenseRepository _licenseRepository;

        public ActivationRequestRepository(ApplicationDbContext context,
            ILicenseRepository licenseRepository) : base(context)
        {
            _licenseRepository = licenseRepository;
        }

        /// <inheritdoc/>
        public Task<Domain.Entities.ActivationRequest> GetAsync(License license)
        {
            return GetAsync(i =>
                i.ActivationId == license.MachineId &&
                i.RequestedClientId == license.OwnerId &&
                i.ProductName == license.ProductName);
        }

        /// <inheritdoc/>
        public Task<Domain.Entities.ActivationRequest> GetAsync(ActivationRequest activationRequest)
        {
            return GetAsync(i =>
                i.ActivationId == activationRequest.ActivationId &&
                i.RequestedClientId == activationRequest.RequestedClientId &&
                i.ProductName == activationRequest.ProductName);
        }

        /// <inheritdoc/>
        public async Task<ActivationRequestStatus> CreateActivationRequest(ActivationRequest activationRequest)
        {
            // check existing activation request
            var existingRequest = await GetAsync(activationRequest);

            // check existing license
            var existingLicenseStatus = await _licenseRepository.CheckLicenseAsync(new License()
            {
                MachineId = activationRequest.ActivationId,
                OwnerId = activationRequest.RequestedClientId,
                ProductName = activationRequest.ProductName
            });

            if (existingLicenseStatus == LicenseStatus.Valid)
                return ActivationRequestStatus.AlreadyHaveValidLicense;

            if (existingRequest != null) 
                return ActivationRequestStatus.RequestAlreadyMade;

            await AddAsync(new Domain.Entities.ActivationRequest()
            {
                Id = activationRequest.Id,
                ActivationId = activationRequest.ActivationId,
                RequestedClientId = activationRequest.RequestedClientId,
                Timestamp = activationRequest.Timestamp,
                ProductName = activationRequest.ProductName
            });

            return ActivationRequestStatus.RequestCreated;
        }
    }
}