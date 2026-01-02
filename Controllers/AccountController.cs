using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;


namespace CVBuddy.Controllers
{
    public class AccountController : HomeController
    {
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> u, CVBuddyContext c, SignInManager<User> s) : base(u, c)
        {
            signInManager = s;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    lvm.UserName, lvm.Password, isPersistent: lvm.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Login = "login post fnkar inte";
            return View(lvm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel usr)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.FirstName = usr.FirstName;
                user.LastName = usr.LastName;
                user.Email = usr.Email;
                user.PhoneNumber = usr.PhoneNumber;
                user.UserName = usr.UserName;
                var result = await _userManager.CreateAsync(user, usr.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(usr);
        }


        [HttpGet]
        public async Task<IActionResult> Logout(string logout)
        {
            ViewBag.LogoutAreYouSure = "Are you sure you want to log out?";
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
