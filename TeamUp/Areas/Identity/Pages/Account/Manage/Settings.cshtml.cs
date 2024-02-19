using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Areas.Identity.Pages.Account.Manage
{
    public class SettingsModel : PageModel
    {

        private readonly UserManager<TeamUpUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly SignInManager<TeamUpUser> _signInManager;
        public SettingsModel(
            UserManager<TeamUpUser> userManager,
            SignInManager<TeamUpUser> signInManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Obecne has�o")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Has�o musi mie� co najmniej {1} i maksymalnie {2} znak�w.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nowe has�o")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierd� has�o")]
            [Compare("NewPassword", ErrorMessage = "Has�a musz� by� takie same.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Zmiana has�a u�ytkownika zako�czy�a si� powodzeniem.");
            StatusMessage = "Has�o zosta�o zmienione.";

            return RedirectToPage();
        }
    }
}
