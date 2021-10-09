using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityModel;
using Licenser.Server.Domain.Entities;

namespace Licenser.IdentityServer
{
    public class CustomClaimsFactory : UserClaimsPrincipalFactory<User>
    {
        private readonly UserManager<User> _userManager;

        public CustomClaimsFactory(
            UserManager<User> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            
            identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            return identity;
        }
    }
}