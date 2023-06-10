using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Diagnostics;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        ApplicationContext db;
        IPasswordHasher<User> hasher;
        public AccountController(ApplicationContext _db,IPasswordHasher<User> _hasher){
            db = _db;
            hasher = _hasher;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.Name == model.Name);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Email = model.Email,Name = model.Name};
                    string hashPass = hasher.HashPassword(user, model.Password);
                    user.Password = hashPass;
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    db.Users.Add(user);
                    await db.SaveChangesAsync();

                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Incorrect username and/or password");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Name == model.Name);
                if (user != null)
                {
                    PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, model.Password);
                    switch (result)
                    {
                        case PasswordVerificationResult.Success:
                            await Authenticate(user); // аутентификация

                            return RedirectToAction("Index", "Home");

                        case PasswordVerificationResult.SuccessRehashNeeded:
                            user.Password = hasher.HashPassword(user, model.Password);
                            db.Entry(user).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            goto case PasswordVerificationResult.Success;
                    }
                }
                ModelState.AddModelError("", "Incorrect username and/or password");
            }
            return View(model);
        }
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}