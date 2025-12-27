using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class UserController : HomeController
    {
        private readonly UserManager<User> userManager;
        public UserController(UserManager<User> u, CVBuddyContext c, UserManager<User> um) : base(u, c)
        {
            userManager = um;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }
    }
}
