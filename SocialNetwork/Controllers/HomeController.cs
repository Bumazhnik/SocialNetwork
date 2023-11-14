using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using System.Diagnostics;

namespace SocialNetwork.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext _db)
        {
            db = _db;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await db.Posts.ToListAsync());
        }
        [HttpGet]
        [Authorize]
        public IActionResult MakePost()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakePost(MakePostViewModel model)
        {
            if (model != null)
            {
                Post post = new Post();
                post.Title = model.Title;
                User user = await GetCurrentUser();
                post.Author = user.Name;
                if (!string.IsNullOrEmpty(model.Text))
                {
                    post.PostType = PostType.Text;
                    post.Text = model.Text;
                }
                else
                {
                    post.PostType = PostType.Image;
                    post.Image = model.Image;
                }
                db.Posts.Add(post);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<User> GetCurrentUser()
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            return user;
        }
    }
}