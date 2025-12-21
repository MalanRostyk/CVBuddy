using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;


namespace CVBuddy.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        public AccountController(UserManager<User> um, SignInManager<User> sm)
        {
            userManager = um;
            signInManager = sm;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel lvm = new LoginViewModel();
            ViewBag.Login = "login get funkar";
            return View(lvm);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(lvm.UserName, lvm.Password, isPersistent: lvm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Login = "login post fnkar";
            return View(lvm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            UserRegisterViewModel urvm = new UserRegisterViewModel();
            return View(urvm);
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel usr)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.UserName = usr.Name;
                var result = await userManager.CreateAsync(user, usr.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(usr);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
