using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class UserController : HomeController
    {
        private readonly UserManager<User> _userManager;
        public UserController(UserManager<User> u, CVBuddyContext c, UserManager<User> um) : base(u, c)
        {
            _userManager = um;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("GetUser", "User");
        }
    }
}
