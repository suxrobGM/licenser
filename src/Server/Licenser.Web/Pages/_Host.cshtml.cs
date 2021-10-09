using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using Licenser.Sdk.Server.Abstractions;

namespace Licenser.Web.Pages
{
    public class HostModel : PageModel
    {
        private readonly ISmsApiServer _apiServer;

        public HostModel(ISmsApiServer apiServer)
        {
            _apiServer = apiServer;
        }

        public IActionResult OnGetLogin()
        {
            return Challenge(AuthProps(),"oidc");
        }

        public async Task OnGetLogout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc", AuthProps());
        }

        public async Task<IActionResult> OnGetSignInOidc()
        {
            var accessToken = await HttpContext.GetUserAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await HttpContext.RevokeUserRefreshTokenAsync();
                Log.Warning("Refresh token was revoked.");
                await OnGetLogout();
            }
            
            _apiServer.SetAccessToken(accessToken);
            return RedirectToPage();
        }

        private AuthenticationProperties AuthProps()
            => new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/")
            };
    }
}