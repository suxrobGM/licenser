using System;
using System.Threading.Tasks;
using Licenser.Domain.Entities;
using Licenser.Domain.Repositories.Abstractions;
using Licenser.Infrastructure.Data;
using Licenser.Shared.Models;

namespace Licenser.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for license entity.
    /// </summary>
    public class LicenseRepository : Repository<License>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public Task<License> GetAsync(LicenseDto licenseDto)
        {
            return GetAsync(i => i.MachineId == licenseDto.MachineId && 
                                       i.OwnerId == licenseDto.OwnerId && 
                                       i.ProductName == licenseDto.ProductName);
        }

        /// <inheritdoc/>
        public async Task AddLicenseAsync(LicenseDto licenseDto)
        {
            var existingLicense = await GetAsync(licenseDto);

            if (existingLicense == null)
            {
                // add new license with machine ID
                await AddAsync(new License()
                {
                    Id = licenseDto.Id,
                    MachineId = licenseDto.MachineId,
                    OwnerId = licenseDto.OwnerId,
                    ObjectName = licenseDto.ObjectName,
                    ProductName = licenseDto.ProductName,
                    IssueDate = licenseDto.IssueDate,
                    ExpiryDate = licenseDto.ExpiryDate
                });
            }
            else
            {
                // if license exists then update only expiry date!
                existingLicense.ExpiryDate = licenseDto.ExpiryDate;
                await UpdateAsync(existingLicense);
            }
        }

        /// <inheritdoc/>
        public async Task<LicenseStatus> CheckLicenseAsync(LicenseDto licenseDto)
        {
            var existingLicense = await GetAsync(licenseDto);

            if (existingLicense == null)
            {
                return LicenseStatus.Invalid;
            }

            return existingLicense.ExpiryDate < DateTime.Now ? LicenseStatus.Expired : LicenseStatus.Valid;
        }
    }
}