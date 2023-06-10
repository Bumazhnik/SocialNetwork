using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SocialNetwork.Controllers
{
    public class TestController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult AdminOnly(){
            return View();
        }
    }
}