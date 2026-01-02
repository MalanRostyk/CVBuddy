using System.Diagnostics;
using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class HomeController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly CVBuddyContext _context;
        public HomeController(UserManager<User> u, CVBuddyContext c)
        {
            _userManager = u;
            _context = c;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

    }
}
