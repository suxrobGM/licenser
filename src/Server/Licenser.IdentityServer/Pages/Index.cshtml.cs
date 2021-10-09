using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenser.IdentityServer.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return LocalRedirect("/Identity/Account/Manage");
        }
    }
}
