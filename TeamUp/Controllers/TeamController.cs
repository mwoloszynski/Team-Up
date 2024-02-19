using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;
using TeamUp.Models;

namespace TeamUp.Controllers
{
    public class TeamController : Controller
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public TeamController(
            UserManager<TeamUpUser> userManager,
            IServiceProvider serviceProvier)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvier;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Wprowadź nazwę.")]
            [DataType(DataType.Text)]
            [Display(Name = "Nazwa")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Wprowadź opis.")]
            [DataType(DataType.Text)]
            [Display(Name = "Opis")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Wprowadź lokalizację.")]
            [DataType(DataType.Text)]
            [Display(Name = "Lokalizacja")]
            public string Localization { get; set; }

            [Display(Name = "Praca zdalna")]
            public bool Remote { get; set; }
        }


        [Authorize]
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            return View(Input);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(InputModel _input, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var userId = _userManager.GetUserId(User);
                TeamUpTeam team = new TeamUpTeam();
                team.AdminId = userId;
                team.Description = _input.Description;
                team.Localization = _input.Localization;
                team.RemoteWork = _input.Remote;
                team.Name = _input.Name;
                if(_input.Localization != null)
                {
                    float lon = 0;
                    float lat = 0;

                    XmlDocument document = new XmlDocument();
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync("https://api.tomtom.com/search/2/search/" + _input.Localization + ".xml?limit=1&countrySet=PL&typeahead=true&key=wxkgUVf1BTjRH5GZO7IhMQbnUsbrgXR1");
                        if (response.IsSuccessStatusCode)
                        {
                            string returnString = await response.Content.ReadAsStringAsync();
                            document.LoadXml(returnString);
                        }
                    }
                    if (document != null)
                    {
                        XmlNode root = document.DocumentElement;
                        if (root != null)
                        {
                            XmlNode result = root.SelectSingleNode("results");
                            XmlNode item = result.FirstChild;
                            XmlNode position = item.SelectSingleNode("position");
                            var latitude = position.SelectSingleNode("lat").InnerText;
                            var longtitude = position.SelectSingleNode("lon").InnerText;
                            lat = float.Parse(latitude, System.Globalization.CultureInfo.InvariantCulture);
                            lon = float.Parse(longtitude, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    team.latitude = lat;
                    team.longtitude = lon;
                }

                context.Teams.Add(team);
                await context.SaveChangesAsync();

                TeamUpTeamUsers userTeam = new TeamUpTeamUsers();
                userTeam.TeamId = team.Id;
                userTeam.UserId = userId;

                context.TeamUsers.Add(userTeam);
                await context.SaveChangesAsync();

                returnUrl = Url.Content("~/Panel?Id=" + team.Id.ToString());
            }

            return LocalRedirect(returnUrl);
        }

        public List<TeamUpUser> TeamUsers { get; set; }

        private TeamUserModel _model(int id)
        {
            TeamUserModel model = new TeamUserModel();
            List<TeamUpUser> TeamUsers = new List<TeamUpUser>();

            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
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

                List<TeamUpSpecialization> specializations = new List<TeamUpSpecialization>();
                var specs = context.Specializations.ToList();
                foreach (var spec in specs)
                    specializations.Add(spec);

                model.Specializations = specializations;
                
            }


            #endregion

            return model;
        }


        [Authorize]
        public IActionResult Profile(int id)
        {
            TeamUserModel model = _model(id);
            if (model == null)
            {
                string returnUrl = null;
                returnUrl = returnUrl ?? Url.Content("~/");

                return LocalRedirect(returnUrl);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string submitButton)
        {
            string returnUrl = null;
            returnUrl = returnUrl ?? Url.Content("~/");

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public IActionResult Application(int id)
        {
            ApplicationTeamSpecModel model = new ApplicationTeamSpecModel();
            
            var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext;
            try
            {
                var teamSpec = context.TeamSpecializations.Where(x => x.Id == id).FirstOrDefault();
                var teamId = teamSpec.TeamId;
                TeamUserModel tsm = _model(teamId);
                model.TeamUserModel = tsm;
                model.TeamSpecialization = teamSpec;

                var _userId = _userManager.GetUserId(User);
                if(tsm.TeamUsers.Where(x => x.UserId == _userId).Any() || tsm.Admin.Id == _userId)
                {
                    string returnUrl = null;
                    returnUrl = returnUrl ?? Url.Content("~/");

                    return LocalRedirect(returnUrl);
                }

            }
            catch (Exception ex)
            {
                context.Dispose();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Application(int id, string message = "")
        {
            string returnUrl = null;
            returnUrl = returnUrl ?? Url.Content("~/");

            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var _userId = _userManager.GetUserId(User);
                var teamSpec = context.TeamSpecializations.Where(x => x.Id == id).FirstOrDefault();
                var teamId = teamSpec.TeamId;

                var applications = context.TeamApplications.Where(x => x.UserId == _userId).ToList();
                foreach(var app in applications)
                {
                    if(app.TeamId == teamId)
                    {
                        return LocalRedirect(returnUrl);
                    }
                }

                TeamUpTeamApplications application = new TeamUpTeamApplications();
                
                application.UserId = _userId;
                application.Message = message;
                application.TeamId = teamId;
                application.TeamSpecId = teamSpec.Id;
                application.CreationDate = DateTime.Now;

                context.TeamApplications.Add(application);
                await context.SaveChangesAsync();
            }

            return LocalRedirect(returnUrl);
        }
    }

    public class ApplicationTeamSpecModel
    {
        public TeamUserModel TeamUserModel { get; set; }

        public TeamUpTeamSpecializations TeamSpecialization { get; set; }
    }
}
