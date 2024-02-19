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

namespace TeamUp.Areas.Panel.Pages
{
    public class ApplicationsModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public ApplicationsModel(
            UserManager<TeamUpUser> userManager,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        private TeamUserModel _model(int id)
        {
            TeamUserModel model = new TeamUserModel();
            List<TeamUpUser> TeamUsers = new List<TeamUpUser>();

            bool userAccess = false;
            var userId = _userManager.GetUserId(User);
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var teamUsers = context.TeamUsers.Where(x => x.TeamId == id).ToList();
                foreach (var user in teamUsers)
                {
                    if (user.UserId == userId)
                        userAccess = true;
                }


                if (!userAccess)
                {
                    string returnUrl = null;
                    returnUrl = returnUrl ?? Url.Content("~/");

                    return null;
                }

                #region Load model

                List<TeamUpUser> _teamUsers = new List<TeamUpUser>();
                TeamUpTeam team = new TeamUpTeam();
                TeamUpUser admin = new TeamUpUser();
                List<TeamUpTeamSpecializations> slots = new List<TeamUpTeamSpecializations>();
                Specializations = new List<TeamUpSpecialization>();



                team = context.Teams.Where(x => x.Id == id).FirstOrDefault();
                var usersId = context.TeamUsers.Where(x => x.TeamId == id).ToList();
                foreach (var user in usersId)
                {
                    var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                    if (findUserDetails.Id == team.AdminId)
                        admin = findUserDetails;
                    else
                        _teamUsers.Add(findUserDetails);
                }

                var specsId = context.TeamSpecializations.Where(x => x.TeamId == id).ToList();
                foreach (var spec in specsId)
                {
                    slots.Add(spec);
                }
                TeamUsers = _teamUsers;

                if (team == null)
                {
                    string returnUrl = null;
                    returnUrl = returnUrl ?? Url.Content("~/");

                    return null;
                }

                model.Admin = admin;
                model.Team = team;
                model.Users = TeamUsers;
                model.Slots = slots;

                var specs = context.Specializations.ToList();
                foreach (var spec in specs)
                    Specializations.Add(spec);

            }


            #endregion

            return model;
        }

        public TeamUserModel TeamUser { get; set; }

        public List<TeamUpUser> ApplicationUsers { get; set; }

        public List<TeamUpTeamApplications> Applications { get; set; }

        public List<TeamUpSpecialization> Specializations { get; set; }

        public int TeamId { get; set; }
        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            ApplicationUsers = new List<TeamUpUser>();
            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
            try
            {
                var applications = context.TeamApplications.Where(x => x.TeamId == id).ToList();
                Applications = applications;
                foreach(var app in Applications)
                {
                    var user = context.Users.Where(x => x.Id == app.UserId).FirstOrDefault();
                    ApplicationUsers.Add(user);
                }
            }
            catch (Exception ex)
            {
                await context.DisposeAsync();
            }

            TeamUser = _model(id);
            if (TeamUser == null)
            {
                string returnUrl = null;
                returnUrl = returnUrl ?? Url.Content("~/");

                return LocalRedirect(returnUrl);
            }

            

            TeamId = id;
            return Page();
        }
    }
}
