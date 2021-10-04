using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sms.IdentityServer.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return LocalRedirect("/Identity/Account/Manage");
        }
    }
}
