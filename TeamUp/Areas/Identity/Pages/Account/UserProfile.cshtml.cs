using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;
using TeamUp.Models;

namespace TeamUp.Areas.Identity.Pages.Account
{
    public class UserProfileModel : PageModel
    {

        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public UserProfileModel(
            UserManager<TeamUpUser> userManager,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        public TeamUpUser userData { get; set; }

        public List<TeamUpSpecialization> userSpecs { get; set; }

        public List<TeamUpTeam> userTeams { get; set; }


        private void downloadUserData(TeamUpUser user)
        {
            userData = new TeamUpUser();
            userSpecs = new List<TeamUpSpecialization>();
            userTeams = new List<TeamUpTeam>();

            userData = user;
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var specs = context.UserSpecializations.Where(x => x.UserId == user.Id).ToList();
                foreach (var spec in specs)
                    userSpecs.Add(context.Specializations.Where(x => x.Id == spec.SpecId).FirstOrDefault());

                var uTeams = context.TeamUsers.Where(x => x.UserId == user.Id).ToList();
                foreach (var team in uTeams)
                    userTeams.Add(context.Teams.Where(x => x.Id == team.TeamId).FirstOrDefault());

                
            }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            downloadUserData(user);
            
            return Page();
        }
    }
}
