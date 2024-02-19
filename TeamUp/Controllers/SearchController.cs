using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;
using TeamUp.Models;

namespace TeamUp.Controllers
{
    public class SearchController : Controller
    {
        public readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SearchController(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            _serviceProvider = serviceProvider;
            _hostingEnvironment = environment;
        }


        public IActionResult Index()
        {
            return View();
        }

        public string NormalizeText(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD).ToCharArray();
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            var builder = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            var result = "";
            foreach (var letter in builder)
            {
                switch (letter)
                {
                    case 'ł':
                        result += 'l';
                        break;
                    default:
                        result += letter;
                        break;
                }
            }
            result = result.Trim();
            return result;
        }

        public async Task<ActionResult> Team(string name, string localization, string spec)
        {
            List<TeamUserModel> model = new List<TeamUserModel>();

            using(var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {

                if(string.IsNullOrEmpty(spec) && string.IsNullOrEmpty(localization) && string.IsNullOrEmpty(name))
                {
                var teams = context.Teams.ToList();
                    foreach (var team in teams)
                    {
                        var teamAdmin = new TeamUpUser();
                        var teamUsers = new List<TeamUpUser>();
                        var teamSlots = new List<TeamUpTeamSpecializations>();
                        var allSpecs = new List<TeamUpSpecialization>();

                        var users = context.TeamUsers.Where(x => x.TeamId == team.Id).ToList();
                        foreach (var user in users)
                        {
                            var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                            if (findUserDetails.Id == team.AdminId)
                                teamAdmin = findUserDetails;
                            else
                                teamUsers.Add(findUserDetails);
                        }
                        var specs = context.TeamSpecializations.Where(x => x.TeamId == team.Id).ToList();
                        foreach (var slot in specs)
                            if (slot.IsAvaible)
                                teamSlots.Add(slot);

                        var _allSpecs = context.Specializations.ToList();
                        foreach (var specc in _allSpecs)
                            allSpecs.Add(specc);

                        model.Add(new TeamUserModel { Admin = teamAdmin, Team = team, Users = teamUsers, Slots = teamSlots, Specializations = allSpecs });
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        // Nazwa teamu nie pusta
                        List<TeamUpTeam> teamList = new List<TeamUpTeam>();

                        name = name.ToLower();
                        name = NormalizeText(name);

                        var teamsOriginal = context.Teams.ToList();
                        List<TeamUpTeam> teams = new List<TeamUpTeam>();
                        // Trzeba stworzyć "kopię" listy teamów, przypisując WARTOŚCI a nie newTeam=team, w przeciwnym wypadku zmiany w liście "teams" powodują zmianę oryginalnej listy
                        foreach (var team in teamsOriginal)
                        {
                            TeamUpTeam newTeam = new TeamUpTeam();
                            newTeam.Id = team.Id;
                            newTeam.AdminId = team.AdminId;
                            newTeam.Localization = team.Localization;
                            newTeam.RemoteWork = team.RemoteWork;
                            newTeam.TeamPicture = team.TeamPicture;
                            newTeam.Name = team.Name;
                            newTeam.Description = team.Description;
                            teams.Add(newTeam);
                        }

                        foreach (var team in teams)
                        {
                            team.Name = team.Name.ToLower();
                            team.Name = NormalizeText(team.Name);
                            if (team.Name == name)
                                teamList.Add(teamsOriginal.Where(x => x.Id == team.Id).FirstOrDefault());
                        }

                        foreach (var team in teamList)
                        {
                            var toDelete = teams.Where(x => x.Id == team.Id).FirstOrDefault();
                            teams.Remove(toDelete);
                        }

                        var stringWords = name.Split(' ');

                        foreach (var team in teams)
                        {
                            foreach (var word in stringWords)
                            {
                                if (word.Length < 3) ;
                                else if (team.Name.Contains(word))
                                {
                                    teamList.Add(teamsOriginal.Where(x => x.Id == team.Id).FirstOrDefault());
                                    break;
                                }
                            }
                        }

                        foreach (var team in teamList)
                        {
                            var teamAdmin = new TeamUpUser();
                            var teamUsers = new List<TeamUpUser>();
                            var teamSlots = new List<TeamUpTeamSpecializations>();
                            var allSpecs = new List<TeamUpSpecialization>();

                            var users = context.TeamUsers.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var user in users)
                            {
                                var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                                if (findUserDetails.Id == team.AdminId)
                                    teamAdmin = findUserDetails;
                                else
                                    teamUsers.Add(findUserDetails);
                            }
                            var specs = context.TeamSpecializations.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var slot in specs)
                                if (slot.IsAvaible)
                                    teamSlots.Add(slot);

                            var _allSpecs = context.Specializations.ToList();
                            foreach (var specc in _allSpecs)
                                allSpecs.Add(specc);

                            model.Add(new TeamUserModel { Admin = teamAdmin, Team = teamsOriginal.Where(x => x.Id == team.Id).FirstOrDefault(), Users = teamUsers, Slots = teamSlots, Specializations = allSpecs });
                        }

                        if(!string.IsNullOrEmpty(spec))
                        {
                            List<TeamUserModel> modelsToDelete = new List<TeamUserModel>();
                            foreach(var m in model)
                            {
                                bool delete = true;
                                for(int i=0; i<m.Slots.Count; i++)
                                {
                                    if(m.Specializations.Where(x => x.Id == m.Slots[i].SpecId).FirstOrDefault().Name == spec)
                                    {
                                        delete = false;
                                        break;
                                    }
                                }
                                if (delete)
                                    modelsToDelete.Add(m);
                            }
                            foreach (var toDelete in modelsToDelete)
                                model.Remove(toDelete);
                        }

                    } 
                    else if(!string.IsNullOrEmpty(spec))
                    {
                        // Nazwa teamu pusta ale określona specjalizacja
                        var teams = context.Teams.ToList();
                        foreach (var team in teams)
                        {
                            var teamAdmin = new TeamUpUser();
                            var teamUsers = new List<TeamUpUser>();
                            var teamSlots = new List<TeamUpTeamSpecializations>();
                            var allSpecs = new List<TeamUpSpecialization>();

                            var users = context.TeamUsers.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var user in users)
                            {
                                var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                                if (findUserDetails.Id == team.AdminId)
                                    teamAdmin = findUserDetails;
                                else
                                    teamUsers.Add(findUserDetails);
                            }
                            var specs = context.TeamSpecializations.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var slot in specs)
                                if (slot.IsAvaible)
                                    teamSlots.Add(slot);

                            var _allSpecs = context.Specializations.ToList();
                            foreach (var specc in _allSpecs)
                                allSpecs.Add(specc);

                            model.Add(new TeamUserModel { Admin = teamAdmin, Team = team, Users = teamUsers, Slots = teamSlots, Specializations = allSpecs });
                        }

                        List<TeamUserModel> modelsToDelete = new List<TeamUserModel>();
                        foreach (var m in model)
                        {
                            bool delete = true;
                            for (int i = 0; i < m.Slots.Count; i++)
                            {
                                if (m.Specializations.Where(x => x.Id == m.Slots[i].SpecId).FirstOrDefault().Name == spec)
                                {
                                    delete = false;
                                    break;
                                }
                            }
                            if (delete)
                                modelsToDelete.Add(m);
                        }
                        foreach (var toDelete in modelsToDelete)
                            model.Remove(toDelete);

                    } 
                    else
                    {
                        // wxkgUVf1BTjRH5GZO7IhMQbnUsbrgXR1
                        // https://developer.tomtom.com/content/search-api-explorer
                        float lon = 0;
                        float lat = 0;

                        XmlDocument document = new XmlDocument();
                        using(HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync("https://api.tomtom.com/search/2/search/" + localization + ".xml?limit=1&countrySet=PL&typeahead=true&key=wxkgUVf1BTjRH5GZO7IhMQbnUsbrgXR1");
                            if (response.IsSuccessStatusCode)
                            {
                                string returnString = await response.Content.ReadAsStringAsync();
                                document.LoadXml(returnString);
                            }
                        }
                        if(document != null)
                        {
                            XmlNode root = document.DocumentElement;
                            if(root != null)
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
                        var teams = context.Teams.ToList();
                        var resultTeams = teams.Where(x => Math.Sqrt(Math.Pow(Math.Abs(x.longtitude - lon), 2) + Math.Pow(Math.Abs(x.latitude - lat), 2)) <= 0.45).ToList();
                        foreach(var team in resultTeams)
                        {
                            var teamAdmin = new TeamUpUser();
                            var teamUsers = new List<TeamUpUser>();
                            var teamSlots = new List<TeamUpTeamSpecializations>();
                            var allSpecs = new List<TeamUpSpecialization>();

                            var users = context.TeamUsers.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var user in users)
                            {
                                var findUserDetails = context.Users.Where(y => y.Id == user.UserId).FirstOrDefault();
                                if (findUserDetails.Id == team.AdminId)
                                    teamAdmin = findUserDetails;
                                else
                                    teamUsers.Add(findUserDetails);
                            }
                            var specs = context.TeamSpecializations.Where(x => x.TeamId == team.Id).ToList();
                            foreach (var slot in specs)
                                if (slot.IsAvaible)
                                    teamSlots.Add(slot);

                            var _allSpecs = context.Specializations.ToList();
                            foreach (var specc in _allSpecs)
                                allSpecs.Add(specc);

                            model.Add(new TeamUserModel { Admin = teamAdmin, Team = team, Users = teamUsers, Slots = teamSlots, Specializations = allSpecs });
                        }

                    }
                }
            }

            return View(model);
        }
    }
}
