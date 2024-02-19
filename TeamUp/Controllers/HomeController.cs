using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;
using TeamUp.Models;

namespace TeamUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<TeamUpUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<TeamUpUser> userManager,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        public IActionResult Prepare()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Index()
        {
            HomeSpecBlogModel model = new HomeSpecBlogModel();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                List<TeamUpPost> posts = new List<TeamUpPost>();
                var _posts = context.Post.ToList();
                foreach (TeamUpPost p in _posts)
                    posts.Add(p);
                posts.Sort((x, y) => DateTime.Compare(x.CreationDate, y.CreationDate));
                posts.Reverse();

                List<TeamUpSpecialization> specs = new List<TeamUpSpecialization>();
                var _specs = context.Specializations.ToList();
                foreach (TeamUpSpecialization s in _specs)
                    specs.Add(s);
                specs.Sort((p, q) => p.Name.CompareTo(q.Name));


                model.Post = posts.ElementAt(0);
                model.Specs = specs;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name, string location, string category)
        {
            string[] searchObjects = { name, location, category };

            return RedirectToAction("Team", "Search", new { name = name, localization = location, spec = category });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult More()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
