using System.Threading.Tasks;
using Licenser.Server.Domain.Entities;
using Licenser.Shared.Models;
using License = Licenser.Shared.Models.License;

namespace Licenser.Server.Domain.Repositories.Abstractions
{
    public interface ILicenseRepository : IRepository<Entities.License>
    {
        /// <summary>
        /// Get license by comparing its MachineId, OwnerId and ProductName properties. All data is case sensitive.
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>License entity type</returns>
        Task<Entities.License> GetAsync(License license);

        /// <summary>
        /// Checks valid license exists in database. If no then adds to database. 
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns></returns>
        Task AddLicenseAsync(License license);

        /// <summary>
        /// Checks license validity.
        /// </summary>
        /// <param name="license">License DTO type</param>
        /// <returns>Valid if license exists and has not expired yet. Expired if license has already expired. Invalid is license does not exist in database.</returns>
        Task<LicenseStatus> CheckLicenseAsync(License license);
    }
}