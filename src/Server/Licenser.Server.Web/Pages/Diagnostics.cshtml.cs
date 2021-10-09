using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenser.Server.Web.Pages
{
    [Authorize]
    public class DiagnosticsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
