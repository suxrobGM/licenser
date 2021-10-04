using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Sms.Licensing.Core.Entities;

namespace Sms.Licensing.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IPersistedGrantDbContext
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public DbSet<License> Licenses { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS; Initial Catalog=SmsLicensingDB; Trusted_Connection=True")
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(m => m.UserLicenses)
                    .WithOne(m => m.Owner)
                    .HasForeignKey(m => m.OwnerId);
            });

            builder.Entity<ActivationRequest>(entity =>
            {
                entity.HasOne(m => m.RequestedClient)
                    .WithOne()
                    .HasForeignKey<ActivationRequest>(i => i.RequestedClientId);
            });
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS; Initial Catalog=SmsLicensingDB; Trusted_Connection=True")
                .UseLazyLoadingProxies();

            return new ApplicationDbContext(optionsBuilder.Options, new OptionsWrapper<OperationalStoreOptions>(new OperationalStoreOptions()));
        }
    }
}
