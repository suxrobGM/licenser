using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Services.Abstractions;
using Licenser.Server.Domain.Options;

namespace Licenser.IdentityServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IGoogleCaptchaService _googleCaptcha;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            GoogleCaptchaOptions captchaOptions,
            ILogger<LoginModel> logger,
            IGoogleCaptchaService googleCaptcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _googleCaptcha = googleCaptcha;
            CaptchaSiteKey = captchaOptions.SiteKey;
        }

        [BindProperty]
        public InputModel Input { get; init; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }
        public string CaptchaSiteKey { get; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username or email")]
            public string Username { get; init; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; init; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; init; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var responseToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            var validCaptcha = await _googleCaptcha.CheckCaptchaResponseAsync(responseToken);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (!validCaptcha)
                ModelState.AddModelError("captcha", "Invalid captcha verification");
        
            // Match input is username or email
            if (Input.Username.IndexOf('@') > -1)
            {
                // Validate email format
                const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                          @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                var re = new Regex(emailRegex);
                if (!re.IsMatch(Input.Username))
                {
                    ModelState.AddModelError("Email", "Email is not valid");
                }
            }
            else
            {
                // Validate Username format
                const string emailRegex = @"^[a-zA-Z0-9]*$";
                var re = new Regex(emailRegex);
                if (!re.IsMatch(Input.Username))
                {
                    ModelState.AddModelError("Username", "Username is not valid");
                }
            }

            if (!ModelState.IsValid) 
                return Page();

            ApplicationUser user;
            if (Input.Username.IndexOf('@') > -1)
            {
                user = await _userManager.FindByEmailAsync(Input.Username);
            }
            else
            {
                user = await _userManager.FindByNameAsync(Input.Username);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User \'{user.UserName}\' logged in.");
                return LocalRedirect(returnUrl);
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning($"User \'{user.UserName}\' account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
