using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sms.Licensing.Domain.Entities;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Infrastructure.Data
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider service)
        {
            await MigrateDatabaseAsync(service);
            await CreateUserRolesAsync(service);
            await CreateOwnerAccount(service);
            await CreateDeletedUserAccountAsync(service);
        }

        private static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static async Task CreateUserRolesAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var superAdminRole = await roleManager.RoleExistsAsync("SuperAdmin");
                var adminRole = await roleManager.RoleExistsAsync("Admin");
                var clientRole = await roleManager.RoleExistsAsync("Client");

                if (!superAdminRole)
                {
                    await roleManager.CreateAsync(new ApplicationRole("SuperAdmin"));
                }
                if (!adminRole)
                {
                    await roleManager.CreateAsync(new ApplicationRole("Admin"));
                }
                if (!clientRole)
                {
                    await roleManager.CreateAsync(new ApplicationRole("Client"));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static async Task CreateOwnerAccount(IServiceProvider service)
        {
            try
            {
                var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                var config = service.GetRequiredService<IConfiguration>();

                var ownerAccount = new UserDto()
                {
                    UserName = config.GetSection("ServerAccounts:Owner:UserName").Value,
                    Email = config.GetSection("ServerAccounts:Owner:Email").Value,
                    Password = config.GetSection("ServerAccounts:Owner:Password").Value
                };

                var siteOwner = await userManager.FindByEmailAsync(ownerAccount.Email);
                if (siteOwner == null)
                {
                    await userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = ownerAccount.UserName,
                        Email = ownerAccount.Email,
                        EmailConfirmed = true

                    }, ownerAccount.Password);
                }

                var hasSuperAdminRole = await userManager.IsInRoleAsync(siteOwner, "SuperAdmin");

                if (!hasSuperAdminRole)
                {
                    await userManager.AddToRoleAsync(siteOwner, "SuperAdmin");
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static async Task CreateDeletedUserAccountAsync(IServiceProvider service)
        {
            try
            {
                var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                var config = service.GetRequiredService<IConfiguration>();

                var deletedUserAccount = new UserDto()
                {
                    UserName = config.GetSection("ServerAccounts:DeletedUser:UserName").Value,
                    Email = config.GetSection("ServerAccounts:DeletedUser:Email").Value,
                    Password = config.GetSection("ServerAccounts:DeletedUser:Password").Value
                };

                var deletedUser = await userManager.FindByNameAsync(deletedUserAccount.UserName);
                if (deletedUser == null)
                {
                    await userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = deletedUserAccount.UserName,
                        Email = deletedUserAccount.Email,
                        EmailConfirmed = true

                    }, deletedUserAccount.Password);
                }

                var hasSuperAdminRole = await userManager.IsInRoleAsync(deletedUser, "SuperAdmin");

                if (!hasSuperAdminRole)
                {
                    await userManager.AddToRoleAsync(deletedUser, "SuperAdmin");
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}