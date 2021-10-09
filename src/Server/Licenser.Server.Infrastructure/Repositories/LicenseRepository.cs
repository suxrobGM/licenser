using System;
using System.Threading.Tasks;
using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Repositories.Abstractions;
using Licenser.Server.Infrastructure.Data;
using Licenser.Shared.Models;
using License = Licenser.Shared.Models.License;

namespace Licenser.Server.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for license entity.
    /// </summary>
    public class LicenseRepository : Repository<Domain.Entities.License>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public Task<Domain.Entities.License> GetAsync(License license)
        {
            return GetAsync(i => i.MachineId == license.MachineId && 
                                       i.OwnerId == license.OwnerId && 
                                       i.ProductName == license.ProductName);
        }

        /// <inheritdoc/>
        public async Task AddLicenseAsync(License license)
        {
            var existingLicense = await GetAsync(license);

            if (existingLicense == null)
            {
                // add new license with machine ID
                await AddAsync(new Domain.Entities.License()
                {
                    Id = license.Id,
                    MachineId = license.MachineId,
                    OwnerId = license.OwnerId,
                    ObjectName = license.ObjectName,
                    ProductName = license.ProductName,
                    IssueDate = license.IssueDate,
                    ExpiryDate = license.ExpiryDate
                });
            }
            else
            {
                // if license exists then update only expiry date!
                existingLicense.ExpiryDate = license.ExpiryDate;
                await UpdateAsync(existingLicense);
            }
        }

        /// <inheritdoc/>
        public async Task<LicenseStatus> CheckLicenseAsync(License license)
        {
            var existingLicense = await GetAsync(license);

            if (existingLicense == null)
            {
                return LicenseStatus.Invalid;
            }

            return existingLicense.ExpiryDate < DateTime.Now ? LicenseStatus.Expired : LicenseStatus.Valid;
        }
    }
}