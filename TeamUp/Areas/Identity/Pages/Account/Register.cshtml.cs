using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<TeamUpUser> _signInManager;
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly Models.EmailConfig.IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;

        public RegisterModel(
            UserManager<TeamUpUser> userManager,
            SignInManager<TeamUpUser> signInManager,
            ILogger<RegisterModel> logger,
            Models.EmailConfig.IEmailSender emailSender,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Wprowadź imię.")]
            [DataType(DataType.Text)]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Wprowadź nazwisko.")]
            [DataType(DataType.Text)]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Wprowadź datę urodzenia.")]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime BirthDate { get; set; }

            [Required(ErrorMessage = "Wprowadź adres email.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Wprowadź hasło.")]
            [StringLength(100, ErrorMessage = "Hasło musi mieć minimum {2} i maksimum {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Wprowadź ponownie hasło.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Obydwa hasła muszą być takie same.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Akceptacja regulaminu jest wymagana.")]
            [Display(Name = "Accept terms")]
            public bool AcceptTerms { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                Response.Redirect("/");

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new System.IO.MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        public IFormFile ReturnFormFile(FileStreamResult result)
        {
            var ms = new MemoryStream();
            try
            {
                result.FileStream.CopyTo(ms);
                return new FormFile(ms, 0, ms.Length, "name", result.FileDownloadName);
            }
            catch (Exception e)
            {
                ms.Dispose();
                throw;
            }
            finally
            {
                ms.Dispose();
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (Input.AcceptTerms == true)
                {
                    // Generating default profile picture
                    var path = Path.Combine(_hostEnvironment.WebRootPath, "img", "default-picture.jpg");
                    var bytes = System.IO.File.ReadAllBytes(path);
                    

                    var user = new TeamUpUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        BirthDate = Input.BirthDate,
                        CreationDate = DateTime.Now,
                        isAdmin = false,
                        ProfilePicture = bytes
                    };
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Stworzono nowego użytkownika.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);


                        await _emailSender.SendEmailAsync(new Models.EmailConfig.Message(new string[] { Input.Email }, "Weryfikacja adresu email", "W celu weryfikacji konta kliknij link poniżej:" + Environment.NewLine + Environment.NewLine + callbackUrl));




                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                } else
                {
                    ModelState.AddModelError(string.Empty, "Akceptacja regulaminu jest wymagana.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
