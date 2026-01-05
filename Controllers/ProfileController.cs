using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class ProfileController : BaseController
    {
        public ProfileController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c)
        {
        }
        [HttpGet]
        public IActionResult ReadProfile()
        {
            return View();
        }
    }
}
