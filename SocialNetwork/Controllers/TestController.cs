using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class TestController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult AdminOnly()
        {
            return View();
        }
    }
}