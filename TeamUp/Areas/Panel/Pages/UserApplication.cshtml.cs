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
    public class UserApplicationModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public UserApplicationModel(
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

            }


            #endregion

            return model;
        }

        public TeamUserModel TeamUser { get; set; }

        public int TeamId { get; set; }

        public TeamUpTeamApplications UserApplication { get; set; }

        public TeamUpSpecialization AppSpec { get; set; }

        public TeamUpUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            UserApplication = new TeamUpTeamApplications();
            var teamId = 0;


            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
            try
            {
                var application = context.TeamApplications.Where(x => x.Id == id).FirstOrDefault();
                if(application == null)
                {
                    string returnUrl = null;
                    returnUrl = returnUrl ?? Url.Content("~/");

                    return LocalRedirect(returnUrl);
                }
                teamId = application.TeamId;
                UserApplication = application;

                var teamSpec = context.TeamSpecializations.Where(x => x.Id == UserApplication.TeamSpecId).FirstOrDefault();
                var spec = context.Specializations.Where(x => x.Id == teamSpec.SpecId).FirstOrDefault();
                AppSpec = spec;

                var user = context.Users.Where(x => x.Id == UserApplication.UserId).FirstOrDefault();
                AppUser = user;

            }
            catch (Exception ex)
            {
                await context.DisposeAsync();
            }

            TeamUser = _model(teamId);
            if (TeamUser == null)
            {
                string returnUrl = null;
                returnUrl = returnUrl ?? Url.Content("~/");

                return LocalRedirect(returnUrl);
            }
            TeamId = teamId;

            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string isAccepted, int id)
        {
            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
            int teamId = 0;
            switch (isAccepted)
            {
                case "Accept":
                    try
                    {
                        var application = context.TeamApplications.Where(x => x.Id == id).FirstOrDefault();
                        if (application == null)
                        {
                            string returnUrl = null;
                            returnUrl = returnUrl ?? Url.Content("~/");

                            return LocalRedirect(returnUrl);
                        }
                        teamId = application.TeamId;

                        var newUser = new TeamUpTeamUsers();
                        newUser.TeamId = teamId;
                        newUser.TeamSpecId = application.TeamSpecId;
                        newUser.UserId = application.UserId;

                        var slot = context.TeamSpecializations.Where(x => x.Id == newUser.TeamSpecId).FirstOrDefault();
                        slot.IsAvaible = false;


                        context.TeamSpecializations.Attach(slot);
                        context.TeamSpecializations.Update(slot);
                        context.TeamUsers.Add(newUser);

                        var teamApplications = context.TeamApplications.Where(x => x.TeamId == teamId).ToList();
                        var toRemove = teamApplications.Where(x => x.TeamSpecId == application.TeamSpecId).ToList();

                        foreach (var remove in toRemove)
                            context.TeamApplications.Remove(remove);

                        context.TeamApplications.Remove(application);
                        await context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        await context.DisposeAsync();
                    }
                    break;
                case "Reject":
                    try
                    {
                        var application = context.TeamApplications.Where(x => x.Id == id).FirstOrDefault();
                        if (application == null)
                        {
                            string returnUrl = null;
                            returnUrl = returnUrl ?? Url.Content("~/");

                            return LocalRedirect(returnUrl);
                        }
                        teamId = application.TeamId;

                        context.TeamApplications.Remove(application);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await context.DisposeAsync();
                    }
                    break;
            }

            return RedirectToPage("Applications", "Panel", new { id = teamId });
        }
    }
}
