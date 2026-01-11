using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        lvm.UserName, lvm.Password, isPersistent: lvm.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or password is incorrect.");
                    }
                }

                return View(lvm);
            }
            catch(Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel usr)
        {
            try
            {
                if (_userManager.Users.Any(u => u.Email == usr.Email))
                    ModelState.AddModelError(nameof(usr.Email), "Email already exists");

                if (_userManager.Users.Any(u => u.PhoneNumber == usr.PhoneNumber))
                    ModelState.AddModelError(nameof(usr.PhoneNumber), "Phone number already exists");

                if (_userManager.Users.Any(u => u.UserName == usr.UserName))
                    ModelState.AddModelError(nameof(usr.UserName), "User name already exists");

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
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View(usr);
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
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
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
        }
    }
}
