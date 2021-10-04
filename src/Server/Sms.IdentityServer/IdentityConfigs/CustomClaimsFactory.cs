using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityModel;
using Sms.Licensing.Core.Entities;

namespace Sms.IdentityServer
{
    public class CustomClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomClaimsFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            
            identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            return identity;
        }
    }
}