using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class ProfileController : BaseController
    {
        public ProfileController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c)
        {
        }
        [HttpGet]
        public IActionResult ReadProfile(string userId)
        {
            var user = _context.Users
                .Include(u => u.OneAddress)
                .Include(u => u.OneCv)
                .Include(u => u.ProjectUsers)
                .Where(u => u.Id.Equals(userId))
                .FirstOrDefault();

            

            if (user == null)
                return NotFound("This Cvs user could not be found.");

            ViewBag.HasOneAdress = user.OneAddress != null;
            ViewBag.HasOneCv = user.OneCv != null; //HasOneCv för att om vi planerar på att man
                                                   //ska kunna komma till Profil sidan inte bara från en persons cv
                                                   //så är det inte garanterat att de har cv

            ViewBag.IsMyProfile = false;
            if (User.Identity!.IsAuthenticated)
            {
                var loggedInUserId = _userManager.GetUserId(User);
                ViewBag.IsMyProfile = loggedInUserId == userId;

            }

            return View(user);
        }
    }
}
