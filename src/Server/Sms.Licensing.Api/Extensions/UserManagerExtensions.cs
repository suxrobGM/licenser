using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sms.Licensing.Domain.Entities;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Api.Extensions
{
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Try to get user by finding 3 types of claims (ID, UserName, Email)
        /// </summary>
        /// <param name="userManager">UserManager class</param>
        /// <param name="userDto">User DTO class</param>
        /// <returns>User</returns>
        public static async Task<ApplicationUser> FindUserAsync(this UserManager<ApplicationUser> userManager, UserAdvancedCredentials userDto)
        {
            ApplicationUser user = null;

            if (!string.IsNullOrEmpty(userDto.Id))
            {
                user = await userManager.FindByIdAsync(userDto.Id);
            }

            if (!string.IsNullOrEmpty(userDto.UserName) && user == null)
            {
                user = await userManager.FindByNameAsync(userDto.UserName);
            }

            if (!string.IsNullOrEmpty(userDto.Email) && user == null)
            {
                user = await userManager.FindByEmailAsync(userDto.Email);
            }

            return user;
        }
    }
}
