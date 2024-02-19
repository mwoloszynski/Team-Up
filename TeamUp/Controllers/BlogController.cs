using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;
using TeamUp.Models;

namespace TeamUp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<TeamUpUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BlogController(
            UserManager<TeamUpUser> userManager,
            IServiceProvider serviceProvider,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
            _hostingEnvironment = environment;
        }

        public IActionResult Index(int page=1)
        {


            var userId = _userManager.GetUserId(User);
            TeamUpUser user = new TeamUpUser();
            BlogUserModel model = new BlogUserModel();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                user = context.Users.Where(x => x.Id == userId).FirstOrDefault();
                model.User = user;

                List<TeamUpPost> posts = new List<TeamUpPost>();
                var _posts = context.Post.ToList();
                foreach (TeamUpPost p in _posts)
                    posts.Add(p);
                posts.Sort((x, y) => DateTime.Compare(x.CreationDate, y.CreationDate));
                posts.Reverse();

                model.Posts = posts;

                float nos = (float)(posts.Count - 7) / 8.0f;
                model.numberOfPages = (int)Math.Ceiling(nos) + 1;

                if (page < 1)
                    page = 1;
                model.currentPage = page;
            }


            return View(model);
        }

        public IActionResult Post(int id = 0)
        {
            PostUserModel PostUserModel = new PostUserModel();
            PostModel model = new PostModel();
            TeamUpUser user = new TeamUpUser();
            string userId;
            if (User != null)
                userId = _userManager.GetUserId(User);
            else
                userId = "";
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                user = context.Users.Where(x => x.Id == userId).FirstOrDefault();

                if (user != null)
                    if (user.isAdmin)
                        PostUserModel.isAdmin = 1;
                   


                var post = context.Post.Where(x => x.Id == id).FirstOrDefault();
                var content = context.PostContent.Where(x => x.PostId == id).ToList();
                for(int i=0; i<content.Count; i++)
                {
                    content.ElementAt(i).Content = content.ElementAt(i).Content.Replace(Environment.NewLine, "\n");
                }
                var images = context.PostImages.Where(x => x.PostId == id).ToList();
                var sources = context.PostSource.Where(x => x.PostId == id).ToList();

                model.Post = post;
                model.PostContent = content;
                model.PostImage = images;
                model.PostSource = sources;

                var allPosts = context.Post.ToList();
                allPosts.Sort((x, y) => DateTime.Compare(x.CreationDate, y.CreationDate));
                allPosts.Reverse();

                #region More Post 
                model.MorePosts = new List<TeamUpPost>();
                if (allPosts.ElementAt(0).Id == id)
                {
                    if(allPosts.Count >= 4)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(1));
                        model.MorePosts.Add(allPosts.ElementAt(2));
                        model.MorePosts.Add(allPosts.ElementAt(3));
                    } else if(allPosts.Count == 1)
                    {

                    } else if(allPosts.Count == 2)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(1));
                    } else if(allPosts.Count == 3)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(1));
                        model.MorePosts.Add(allPosts.ElementAt(2));
                    }
                } else if (allPosts.ElementAt(1).Id == id)
                {
                    if (allPosts.Count >= 4)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(2));
                        model.MorePosts.Add(allPosts.ElementAt(3));
                    }
                    else if (allPosts.Count == 1)
                    {

                    }
                    else if (allPosts.Count == 2)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                    }
                    else if (allPosts.Count == 3)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(2));
                    }
                } else if (allPosts.ElementAt(2).Id == id)
                {
                    if (allPosts.Count >= 4)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(1));
                        model.MorePosts.Add(allPosts.ElementAt(3));
                    }
                    else if (allPosts.Count == 1)
                    {

                    }
                    else if (allPosts.Count == 2)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                    }
                    else if (allPosts.Count == 3)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(1));
                    }
                } 
                else
                {
                    if (allPosts.Count >= 4)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(1));
                        model.MorePosts.Add(allPosts.ElementAt(2));
                    }
                    else if (allPosts.Count == 1)
                    {

                    }
                    else if (allPosts.Count == 2)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                    }
                    else if (allPosts.Count == 3)
                    {
                        model.MorePosts.Add(allPosts.ElementAt(0));
                        model.MorePosts.Add(allPosts.ElementAt(1));
                    }
                }
                #endregion
            }

            PostUserModel.PostModel = model;

            return View(PostUserModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id=0)
        {
            string returnUrl = Url.Content("~/Blog");
            var userId = _userManager.GetUserId(User);
            TeamUpUser user = new TeamUpUser();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                user = context.Users.Where(x => x.Id == userId).FirstOrDefault();

                if (user != null)
                    if (!user.isAdmin)
                        return LocalRedirect(returnUrl);

                if (id != 0)
                {
                    try
                    {
                        TeamUpPost toDeletePost = new TeamUpPost() { Id = id };

                        var toDeleteContent = context.PostContent.Where(x => x.PostId == id).ToList();
                        foreach (var toDelete in toDeleteContent)
                        {
                            context.PostContent.Attach(toDelete);
                            context.PostContent.Remove(toDelete);
                        }

                        var toDeleteSource = context.PostSource.Where(x => x.PostId == id).ToList();
                        foreach (var toDelete in toDeleteSource)
                        {
                            context.PostSource.Attach(toDelete);
                            context.PostSource.Remove(toDelete);
                        }

                        var toDeleteImage = context.PostImages.Where(x => x.PostId == id).ToList();
                        foreach (var toDelete in toDeleteImage)
                        {
                            context.PostImages.Attach(toDelete);
                            context.PostImages.Remove(toDelete);
                        }

                        context.Post.Attach(toDeletePost);
                        context.Post.Remove(toDeletePost);

                        await context.SaveChangesAsync();


                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(_hostingEnvironment.ContentRootPath + "\\sitemap.xml");
                        var namespaceUri = xmldoc.GetElementsByTagName("urlset").Item(0).NamespaceURI;


                        XmlNodeList projectNodes = xmldoc.GetElementsByTagName("url");
                        for (int i = 0; i < projectNodes.Count; i++)
                        {
                            if (projectNodes[i].ChildNodes.Item(0).InnerText == "https://team-up.pl/Blog/Post/" + id)
                            {
                                projectNodes[i].ParentNode.RemoveChild(projectNodes[i]);
                                break;
                            }
                        }

                        xmldoc.Save(_hostingEnvironment.ContentRootPath + "\\sitemap.xml");

                    }
                    catch (Exception ex)
                    {
                        await context.DisposeAsync();
                    }
                }
            }

            return LocalRedirect(returnUrl);
        }


        [Authorize]
        public IActionResult Create()
        {
            string returnUrl = Url.Content("~/Blog");
            var userId = _userManager.GetUserId(User);
            TeamUpUser user = new TeamUpUser();
            BlogUserModel model = new BlogUserModel();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                user = context.Users.Where(x => x.Id == userId).FirstOrDefault();
                model.User = user;
            }
            if (model.User != null)
                if (!model.User.isAdmin)
                    return LocalRedirect(returnUrl);

            return View();
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
            using (var target = new System.IO.MemoryStream())
            {
                image.SaveAsJpeg(target);
                

                return target.ToArray();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile mainImage, string category, string author, string title, string[] sectionTitle, 
            string[] sectionContent, IFormFile[] contentImages, string[] source, string header)
        {
            string returnUrl = Url.Content("~/Blog");
            var userId = _userManager.GetUserId(User);
            TeamUpUser user = new TeamUpUser();
            BlogUserModel model = new BlogUserModel();
            using (var context = _serviceProvider.GetService(typeof(TeamUpDbContext)) as TeamUpDbContext)
            {
                user = context.Users.Where(x => x.Id == userId).FirstOrDefault();
                model.User = user;
            if (model.User != null)
                if (!model.User.isAdmin)
                    return LocalRedirect(returnUrl);

                TeamUpPost Post = new TeamUpPost();
                Post.Header = header;
                Post.Title = title;
                Post.Author = author;
                Post.Category = category;
                Post.CreationDate = DateTime.Now;

                string upload = Path.Combine(_hostingEnvironment.WebRootPath, @"img\blog");
                string filePath = Path.Combine(upload, mainImage.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(fileStream);
                }

                Post.MainImage = mainImage.FileName;

                context.Post.Add(Post);
                await context.SaveChangesAsync();

                for(int i=0; i<sectionTitle.Count(); i++)
                {
                    TeamUpPostContent Content = new TeamUpPostContent();
                    Content.PostId = Post.Id;
                    Content.Subtitle = sectionTitle[i];
                    Content.Content = sectionContent[i];

                    context.PostContent.Add(Content);
                    await context.SaveChangesAsync();
                }

                for(int i=0; i<contentImages.Count(); i++)
                {
                    TeamUpPostImage Image = new TeamUpPostImage();
                    Image.PostId = Post.Id;
                    Image.Image = GetByteArrayFromImage(contentImages[i]);
                    Image.ImageName = contentImages[i].FileName;

                    context.PostImages.Add(Image);
                    await context.SaveChangesAsync();
                }

                for(int i=0; i<source.Count(); i++)
                {
                    TeamUpPostSource Source = new TeamUpPostSource();
                    Source.PostId = Post.Id;
                    Source.Source = source[i];
                    Source.ShortLink = source[i];

                    context.PostSource.Add(Source);
                    await context.SaveChangesAsync();
                }

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(_hostingEnvironment.ContentRootPath + "\\sitemap.xml");
                var namespaceUri = xmldoc.GetElementsByTagName("urlset").Item(0).NamespaceURI;
                XmlElement parent = xmldoc.CreateElement("url", namespaceUri);
                XmlElement loc = xmldoc.CreateElement("loc", namespaceUri);
                loc.InnerText = "https://team-up.pl/Blog/Post/" + Post.Id.ToString();
                XmlElement lastMod = xmldoc.CreateElement("lastmod", namespaceUri);
                lastMod.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                XmlElement priority = xmldoc.CreateElement("priority", namespaceUri);
                priority.InnerText = "0.8";
                parent.AppendChild(loc);
                parent.AppendChild(lastMod);
                parent.AppendChild(priority);
                xmldoc.DocumentElement.AppendChild(parent);
                xmldoc.Save(_hostingEnvironment.ContentRootPath + "\\sitemap.xml");

            }
            

            return LocalRedirect(returnUrl);
        }
    }
}

