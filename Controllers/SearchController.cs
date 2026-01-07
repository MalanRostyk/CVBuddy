using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class SearchController : HomeController
    {
        public SearchController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            List<User> users = new();

            if (User.Identity!.IsAuthenticated)
            {
                users = await _context.Users
                .Where(u => !u.IsDeactivated)
                .ToListAsync();
            }
            else
            {
                users = await _context.Users
                .Where(u => !u.IsDeactivated && !u.OneCv.IsPrivate)
                .ToListAsync();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users
                    .Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.LastName.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }

            return View(users);
        }
    }
}