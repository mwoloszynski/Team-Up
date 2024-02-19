using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;

namespace TeamUp.Areas.Identity.Pages.Account.Manage
{
    public class ChangeSpecializationsModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly SignInManager<TeamUpUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IServiceProvider _serviceProvider;

        public ChangeSpecializationsModel(
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

        public List<TeamUpSpecialization> UserSpecs { get; set; }

        public List<TeamUpSpecialization> AllSpecs { get; set; }

        public List<TeamUpUserSpecializations> UserSpecRating { get; set; }


        [BindProperty]
        public int AddSpecId { get; set; }

        [BindProperty]
        public int AddSpecRating { get; set; }


        [BindProperty]
        public int EditSpecId { get; set; }

        [BindProperty]
        public int EditSpecRating { get; set; }


        [BindProperty]
        public int DeletespecId { get; set; }

        

        [TempData]
        public string StatusMessage { get; set; }

        private async Task LoadAsync(TeamUpUser user)
        {
            List<TeamUpSpecialization> _allSpecs = new List<TeamUpSpecialization>();
            List<TeamUpSpecialization> _userSpecs = new List<TeamUpSpecialization>();
            List<TeamUpUserSpecializations> _userSpecRating = new List<TeamUpUserSpecializations>();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                var userSpecsId = context.UserSpecializations.Where(x => x.UserId == user.Id);
                foreach(var specId in userSpecsId)
                {
                    var singleSpec = context.Specializations.Where(x => x.Id == specId.SpecId).ToList();
                    foreach(var spec in singleSpec)
                    {
                        _userSpecs.Add(spec);
                        var specRating = context.UserSpecializations.Where(x => x.SpecId == spec.Id).ToList();
                        foreach (var rating in specRating)
                            _userSpecRating.Add(rating);
                    }
                        
                }
                foreach (var spec in context.Specializations)
                    _allSpecs.Add(spec);
            }
            _allSpecs.Sort((p, q) => p.Name.CompareTo(q.Name));
            AllSpecs = _allSpecs;
            UserSpecs = _userSpecs;
            UserSpecRating = _userSpecRating;
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

        public async Task<IActionResult> OnPostAsync(string submitButton)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var submitOption = submitButton.Split(' ');
            var option = submitOption[0];

            if (ModelState.IsValid)
            {
                switch(option)
                {
                    case "save":
                        var saveSpecId = submitOption[1];


                        using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
                        {
                            var entity = context.UserSpecializations.FirstOrDefault(x => x.Id == Convert.ToInt32(saveSpecId));
                            if(entity != null)
                            {
                                entity.Stars = EditSpecRating;
                                context.Attach(entity);
                                context.Entry(entity).Property(p => p.Stars).IsModified = true;

                                await context.SaveChangesAsync();
                                StatusMessage = "Zaktualizowano specjalizacje.";
                            }
                            return RedirectToPage();
                        }
                        return RedirectToPage();

                    case "delete":
                        var deleteSpecId = submitOption[1];
                        TeamUpUserSpecializations toDelete = new TeamUpUserSpecializations() { Id = Convert.ToInt32(deleteSpecId) };
                        using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
                        {
                            context.UserSpecializations.Attach(toDelete);
                            context.UserSpecializations.Remove(toDelete);
                            await context.SaveChangesAsync();
                            StatusMessage = "Usuniêto specjalizacje.";
                            return RedirectToPage();
                        }

                        return RedirectToPage();
                    case "add":
                        if (AddSpecId != 0)
                        {
                            if (AddSpecRating != 0)
                            {
                                var userSpec = new TeamUpUserSpecializations
                                {
                                    UserId = user.Id,
                                    SpecId = AddSpecId,
                                    Stars = AddSpecRating
                                };
                                using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
                                {
                                    var singleSpec = context.UserSpecializations.Where(x => x.SpecId == AddSpecId);
                                    if (singleSpec.Any())
                                    {
                                        StatusMessage = "Posiadasz ju¿ wybran¹ specjalizacje.";
                                        return RedirectToPage();
                                    }

                                    context.UserSpecializations.Add(userSpec);
                                    await context.SaveChangesAsync();
                                    StatusMessage = "Dodano specjalizacje.";
                                    return RedirectToPage();
                                }
                            }
                            else
                            {
                                StatusMessage = "Nie wybra³eœ poziomu zaawansowania.";
                                return RedirectToPage();
                            }
                        }
                        else
                        {
                            StatusMessage = "Nie wybra³eœ specjalizacji.";
                            return RedirectToPage();
                        }
                    default:
                        return RedirectToPage();
                }
            }
            return Page();
        }
    }
}
