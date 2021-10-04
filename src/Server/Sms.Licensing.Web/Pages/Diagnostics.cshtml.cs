using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sms.Licensing.Web.Pages
{
    [Authorize]
    public class DiagnosticsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
