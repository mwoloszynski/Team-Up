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
    public class ChatModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public ChatModel(
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

                var teamChats = context.TeamChat.Where(x => x.TeamId == id).ToList();
                foreach(var chat in teamChats)
                {
                    TeamChat.Add(chat);
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

        public string UserId { get; set; }

        public List<TeamUpTeamChat> TeamChat { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {

            TeamChat = new List<TeamUpTeamChat>();
            TeamUser = _model(id);
            if (TeamUser == null)
            {
                string returnUrl = null;
                returnUrl = returnUrl ?? Url.Content("~/");

                return LocalRedirect(returnUrl);
            }
            var _userId = _userManager.GetUserId(User);
            UserId = _userId;
            

            TeamId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id=0, string message="")
        {
            /*
            var _userId = _userManager.GetUserId(User);
            UserId = _userId;

            if (ModelState.IsValid)
            {
                if(message != "")
                {
                    var newMessage = new TeamUpTeamChat
                    {
                        SendDate = DateTime.Now,
                        UserId = this.UserId,
                        TeamId = id,
                        Message = message
                    };
                    newMessage.SendDate = newMessage.SendDate.AddTicks(-(newMessage.SendDate.Ticks % TimeSpan.TicksPerSecond));

                    var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
                    try
                    {
                        context.TeamChat.Add(newMessage);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await context.DisposeAsync();
                    }
                }
            }


            TeamChat = new List<TeamUpTeamChat>();
            TeamUser = _model(id);
            if (TeamUser == null)
            {
                string returnUrl = null;
                returnUrl = returnUrl ?? Url.Content("~/");

                return LocalRedirect(returnUrl);
            }
            TeamId = id;

            
            */
            return Page();
        }
    }
}
