using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly SignInManager<TeamUpUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(
            UserManager<TeamUpUser> userManager,
            SignInManager<TeamUpUser> signInManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        public string Username { get; set; }

        public TeamUpUser UserAccount { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            /*
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            */
            [Required(ErrorMessage = "Wprowadź imię.")]
            [DataType(DataType.Text)]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Wprowadź nazwisko.")]
            [DataType(DataType.Text)]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Wprowadź datę urodzenia.")]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime BirthDate { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Opis")]
            public string Description { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Lokalizacja")]
            public string Localization { get; set; }


            [Display(Name = "Zdjęcie profilowe")]
            public IFormFile ProfilePicute { get; set; }
        }

        private async Task LoadAsync(TeamUpUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            TeamUpUser account = await _userManager.FindByIdAsync(currentUserName);
            UserAccount = account;

            Username = userName;

            Input = new InputModel
            {
                BirthDate = UserAccount.BirthDate,
                Description = UserAccount.Description,
                FirstName = UserAccount.FirstName,
                LastName = UserAccount.LastName,
                Localization = UserAccount.Localization
            };

           
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

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
            //image.Mutate(x => x.Resize(244, 272));

            using (var target = new System.IO.MemoryStream())
            {
                image.SaveAsJpeg(target);

                return target.ToArray();
            }
        }

        private byte[] CropImage(byte[] byteImg)
        {
            System.Drawing.Image img;
            img = (Bitmap)((new ImageConverter()).ConvertFrom(byteImg));

            int width;
            int height;
            if(img.Width > img.Height)
            {
                height = 272;
                float multiply = 272 / (float)img.Height;
                width = (int)(multiply * (float)(img.Width));
            }
            else
            {
                width = 244;
                float multiply = 244 / (float)img.Width;
                height = (int)(multiply * (float)(img.Height));
            }

            if(width < 244)
            {
                float temp = (float)244 / (float)width;
                width = 244;
                height = (int)((float)height * temp);
            }
            if(height < 272)
            {
                float temp = (float)272 / (float)height;
                height = 272;
                width = (int)((float)width * temp);
            }

            Bitmap bpImg = ResizeImage(img, width, height);
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle();
            rec.X = 0;
            rec.Y = 0;
            rec.Height = 272;
            rec.Width = 244;


            img = (System.Drawing.Image)bpImg.Clone(rec, bpImg.PixelFormat);
            ImageConverter _imageConverter = new ImageConverter();
            byte[] resultByteImg = (byte[])_imageConverter.ConvertTo(img, typeof(byte[]));
            return resultByteImg;
        }

        public Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static bool IsImage(IFormFile file)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(file.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var postedFileExtension = System.IO.Path.GetExtension(file.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!file.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //   Check whether the image size exceeding the limit or not
                //------------------------------------------ 
                int ImageMinimumBytes = 512;
                if (file.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                file.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            byte[] img = null;

            if(Input.ProfilePicute != null)
            {
                if(!IsImage(Input.ProfilePicute))
                {
                    StatusMessage = "Przesłany plik nie został rozpoznany jako zdjęcie.";
                    return RedirectToPage();
                }

                img = GetByteArrayFromImage(Input.ProfilePicute);
                img = CropImage(img);


                user.ProfilePicture = img;
                var setProfileResult = await _userManager.UpdateAsync(user);
                if(!setProfileResult.Succeeded)
                {
                    StatusMessage = "Pojawił się błąd podczas próby zapisania zdjęcia profilowego.";
                    return RedirectToPage();
                }
            }
                
            
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            /*
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            */
            bool userChanged = false;
            if (Input.LastName != user.LastName || Input.FirstName != user.FirstName || Input.BirthDate != user.BirthDate ||
                Input.Description != user.Description || Input.Localization != user.Localization)
                userChanged = true;

                if (Input.FirstName != user.FirstName)
                user.FirstName = Input.FirstName;
            if (Input.LastName != user.LastName)
                user.LastName = Input.LastName;
            if (Input.BirthDate != user.BirthDate)
                user.BirthDate = Input.BirthDate;
            if (Input.Description != user.Description)
                user.Description = Input.Description;
            if (Input.Localization != user.Localization)
                user.Localization = Input.Localization;


            if (userChanged)
            {
                var updateUserProfile = await _userManager.UpdateAsync(user);
                if (!updateUserProfile.Succeeded)
                {
                    StatusMessage = "Pojawił się błąd podczas próby zapisania profilu.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Twój profil został zaktualizowany";
            return RedirectToPage();
        }
    }
}
