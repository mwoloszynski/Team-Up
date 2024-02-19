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
    public class UsersModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public UsersModel(
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
            var _userId = _userManager.GetUserId(User);
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var teamUsers = context.TeamUsers.Where(x => x.TeamId == id).ToList();
                foreach (var user in teamUsers)
                {
                    if (user.UserId == _userId)
                        userAccess = true;
                }


                if (!userAccess)
                {
                    string returnUrl = null;
                    returnUrl = returnUrl ?? Url.Content("~/");

                    return null;
                }
                else
                    UserId = _userId;

                #region Load model

                List<TeamUpUser> _teamUsers = new List<TeamUpUser>();
                TeamUpTeam team = new TeamUpTeam();
                TeamUpUser admin = new TeamUpUser();
                List<TeamUpTeamSpecializations> slots = new List<TeamUpTeamSpecializations>();
                List<TeamUpTeamUsers> users = new List<TeamUpTeamUsers>();



                team = context.Teams.Where(x => x.Id == id).FirstOrDefault();
                var usersId = context.TeamUsers.Where(x => x.TeamId == id).ToList();
                foreach (var user in usersId)
                {
                    var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                    if (findUserDetails.Id == team.AdminId)
                        admin = findUserDetails;
                    else
                        _teamUsers.Add(findUserDetails);
                    if (user.UserId != team.AdminId)
                        users.Add(user);
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
                model.TeamUsers = users;


                var specs = context.Specializations.ToList();

                foreach (var spec in specs)
                    Specializations.Add(spec);
                Specializations.Sort((p, q) => p.Name.CompareTo(q.Name));
            }


            #endregion

            return model;
        }

        public string UserId { get; set; }

        public TeamUserModel TeamUser { get; set; }

        public List<TeamUpSpecialization> Specializations { get; set; }

        public int TeamId { get; set; }


        [BindProperty]
        public int AddSpecId { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            Specializations = new List<TeamUpSpecialization>();
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

        public async Task<IActionResult> OnPostAsync(string submitButton, int id = 0, int spec = 0, int user = 0)
        {
            var submitOption = submitButton.Split(' ');
            var option = submitOption[0];

            if(ModelState.IsValid)
            {
                switch(option)
                {
                    case "add":
                        if (AddSpecId != 0)
                        {
                            var newSlot = new TeamUpTeamSpecializations
                            {
                                IsAvaible = true,
                                SpecId = AddSpecId,
                                TeamId = id
                            };

                            {
                                var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
                                try
                                {
                                    context.TeamSpecializations.Add(newSlot);
                                    await context.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                    await context.DisposeAsync();
                                }
                            }
                        }
                        break;
                    case "delete-spec":
                        if(spec != 0)
                        {
                            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
                            try
                            {
                                TeamUpTeamSpecializations toDelete = new TeamUpTeamSpecializations() { Id = spec };
                                context.TeamSpecializations.Attach(toDelete);
                                context.TeamSpecializations.Remove(toDelete);
                                await context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                await context.DisposeAsync();
                            }
                        }
                        break;
                    case "delete-user":
                        if(user != 0)
                        {
                            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
                            try
                            {
                                var toEdit = context.TeamSpecializations.Where(x => x.Id == spec).FirstOrDefault();
                                toEdit.IsAvaible = true;
                                //TeamUpTeamSpecializations toEdit = new TeamUpTeamSpecializations() { Id = edit., SpecId = edit.SpecId, TeamId = edit.TeamId };
                                TeamUpTeamUsers toDelete = new TeamUpTeamUsers() { Id = user };
                                context.TeamSpecializations.Attach(toEdit);
                                context.TeamSpecializations.Update(toEdit);
                                context.TeamUsers.Attach(toDelete);
                                context.TeamUsers.Remove(toDelete);
                                await context.SaveChangesAsync();
                            } catch (Exception ex)
                            {
                                await context.DisposeAsync();
                            }
                        }
                        break;
                }
            }

            



            Specializations = new List<TeamUpSpecialization>();
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
