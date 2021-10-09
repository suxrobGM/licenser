using System.Threading.Tasks;
using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Repositories.Abstractions;
using Licenser.Server.Infrastructure.Data;
using Licenser.Shared.Models;

namespace Licenser.Server.Infrastructure.Repositories
{
    public class ActivationRequestRepository : Repository<ActivationRequest>, IActivationRequestRepository
    {
        private readonly ILicenseRepository _licenseRepository;

        public ActivationRequestRepository(ApplicationDbContext context,
            ILicenseRepository licenseRepository) : base(context)
        {
            _licenseRepository = licenseRepository;
        }

        /// <inheritdoc/>
        public Task<ActivationRequest> GetAsync(LicenseDto licenseDto)
        {
            return GetAsync(i =>
                i.ActivationId == licenseDto.MachineId &&
                i.RequestedClientId == licenseDto.OwnerId &&
                i.ProductName == licenseDto.ProductName);
        }

        /// <inheritdoc/>
        public Task<ActivationRequest> GetAsync(ActivationRequestDto activationRequestDto)
        {
            return GetAsync(i =>
                i.ActivationId == activationRequestDto.ActivationId &&
                i.RequestedClientId == activationRequestDto.RequestedClientId &&
                i.ProductName == activationRequestDto.ProductName);
        }

        /// <inheritdoc/>
        public async Task<ActivationRequestStatus> CreateActivationRequest(ActivationRequestDto activationRequestDto)
        {
            // check existing activation request
            var existingRequest = await GetAsync(activationRequestDto);

            // check existing license
            var existingLicenseStatus = await _licenseRepository.CheckLicenseAsync(new LicenseDto()
            {
                MachineId = activationRequestDto.ActivationId,
                OwnerId = activationRequestDto.RequestedClientId,
                ProductName = activationRequestDto.ProductName
            });

            if (existingLicenseStatus == LicenseStatus.Valid)
                return ActivationRequestStatus.AlreadyHaveValidLicense;

            if (existingRequest != null) 
                return ActivationRequestStatus.RequestAlreadyMade;

            await AddAsync(new ActivationRequest()
            {
                Id = activationRequestDto.Id,
                ActivationId = activationRequestDto.ActivationId,
                RequestedClientId = activationRequestDto.RequestedClientId,
                Timestamp = activationRequestDto.Timestamp,
                ProductName = activationRequestDto.ProductName
            });

            return ActivationRequestStatus.RequestCreated;
        }
    }
}