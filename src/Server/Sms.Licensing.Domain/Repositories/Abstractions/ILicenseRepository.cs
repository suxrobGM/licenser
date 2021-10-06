using System.Threading.Tasks;
using Sms.Licensing.Domain.Entities;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Domain.Repositories.Abstractions
{
    public interface ILicenseRepository : IRepository<License>
    {
        /// <summary>
        /// Get license by comparing its MachineId, OwnerId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="licenseDto">License DTO type</param>
        /// <returns>License entity type</returns>
        Task<License> GetAsync(LicenseDto licenseDto);

        /// <summary>
        /// Checks valid license exists in database. If no then adds to database. 
        /// </summary>
        /// <param name="licenseDto">License DTO type</param>
        /// <returns></returns>
        Task AddLicenseAsync(LicenseDto licenseDto);

        /// <summary>
        /// Checks license validity.
        /// </summary>
        /// <param name="licenseDto">License DTO type</param>
        /// <returns>Valid if license exists and has not expired yet. Expired if license has already expired. Invalid is license does not exist in database.</returns>
        Task<LicenseStatus> CheckLicenseAsync(LicenseDto licenseDto);
    }
}