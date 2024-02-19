using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;

namespace TeamUp.Areas.Identity.Pages.Account.Manage
{
    public class UserTeamsModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly SignInManager<TeamUpUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IServiceProvider _serviceProvider;

        public UserTeamsModel(
            UserManager<TeamUpUser> userManager,
            SignInManager<TeamUpUser> signInManager,
            IWebHostEnvironment hostEnvironment,
            IServiceProvider serviceProvier)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
            _serviceProvider = serviceProvier;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<TeamUpTeam> UserTeams { get; set; }

        private async Task LoadAsync(TeamUpUser user)
        {
            List<TeamUpTeam> _userTeams = new List<TeamUpTeam>();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var teamUserId = context.TeamUsers.Where(x => x.UserId == user.Id);
                foreach(var teamId in teamUserId)
                {
                    var singleTeam = context.Teams.Where(x => x.Id == teamId.TeamId).ToList();
                    foreach (var team in singleTeam)
                        _userTeams.Add(team);
                }
            }
            UserTeams = _userTeams;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (ModelState.IsValid)
            {

            }


            return Page();
        }
    }
}
