using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Licenser.Server.Domain.Entities;
using Licenser.Server.Domain.Options;
using Licenser.Server.Domain.Services.Abstractions;

namespace Licenser.IdentityServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IGoogleCaptchaService _googleCaptcha;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GoogleCaptchaOptions captchaOptions,
            ILogger<RegisterModel> logger,
            IEmailSenderService emailSenderService,
            IGoogleCaptchaService googleCaptcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
            _googleCaptcha = googleCaptcha;
            CaptchaSiteKey = captchaOptions.SiteKey;
        }

        [BindProperty]
        public InputModel Input { get; init; }

        public string ReturnUrl { get; set; }
        public string CaptchaSiteKey { get; }
        public bool AddClient { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; init; }
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; init; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; init; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; init; }
        }

        public async Task OnGetAsync(string returnUrl = null, bool addClient = false)
        {
            ReturnUrl = returnUrl;
            AddClient = addClient;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, bool addClient = false)
        {
            returnUrl ??= Url.Content("~/");
            var responseToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            var validCaptcha = await _googleCaptcha.CheckCaptchaResponseAsync(responseToken);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!validCaptcha)
                ModelState.AddModelError("captcha", "Invalid captcha verification");

            if (!ModelState.IsValid) 
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.UserName, 
                Email = Input.Email
            };
                
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                if (addClient)
                {
                    await _userManager.AddToRoleAsync(user, "Client");
                    _logger.LogInformation($"Added 'Client' role to user '{user.UserName}'.");
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    null,
                    new { area = "Identity", userId = user.Id, code, returnUrl },
                    Request.Scheme);

                await _emailSenderService.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                }

                await _signInManager.SignInAsync(user, false);
                return LocalRedirect(returnUrl);
                //return RedirectToPage("./Login", new { returnUrl = returnUrl });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
