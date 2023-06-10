using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics;

namespace SocialNetwork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationContext db;
        static List<Post> posts;
        public HomeController(ILogger<HomeController> logger,ApplicationContext _db)
        {
            _logger = logger;
            db = _db;
        }
        static HomeController(){
            posts = new List<Post>();
            for(int i = 0;i<1;i++){
                Post post = new Post();
                post.Title = "Post from Controller";
                post.Author = "homecontroller";
                post.Text = "A very meaningful text";
                post.Likes = 42;
                post.PostType = PostType.Text;
                posts.Add(post);
                post = new Post();
                post.Title = "Another post from Controller";
                post.Author = "homecontroller";
                post.Image = "https://placehold.co/300x400";
                post.PostType = PostType.Image;
                posts.Add(post);
            }
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(posts);
        }
        [HttpGet]
        [Authorize]
        public IActionResult MakePost(){
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakePost(MakePostViewModel model){
            if(model != null){
                Post post = new Post();
                post.Title=model.Title;
                User user = await GetCurrentUser();
                post.Author = user.Name;
                if(!string.IsNullOrEmpty(model.Text)){
                    post.PostType = PostType.Text;
                    post.Text = model.Text;
                }
                else{
                    post.PostType = PostType.Image;
                    post.Image = model.Image;
                }
                posts.Add(post);
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
            var user = await db.Users.FirstOrDefaultAsync(x=>x.Name == User.Identity.Name);
            return user;
        }
    }
}