using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class OptionsController : HomeController
    {
        public OptionsController(UserManager<User> u, CVBuddyContext c) : base(u, c)
        {

        }
        public IActionResult GetOptions()
        {
            return View();
        }
    }
}