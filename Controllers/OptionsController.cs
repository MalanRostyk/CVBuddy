using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class OptionsController : HomeController
    {
        public OptionsController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOptions()
        {
            OptionsViewModel optViewModel = new();

            var user = await _userManager.GetUserAsync(User);

            ViewBag.IsSetToDeactivated = user.IsDeactivated;
            ViewBag.HasSetProfilePrivate = user.HasPrivateProfile;
            optViewModel.IsDeactivated = ViewBag.IsSetToDeactivated;
            optViewModel.HasPrivateProfile = ViewBag.HasSetProfilePrivate;

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> GetOptions(OptionsViewModel optModel)
        {

            var user = await _userManager.GetUserAsync(User);

            user.IsDeactivated = optModel.IsDeactivated;
            user.HasPrivateProfile = optModel.HasPrivateProfile;

            await _userManager.UpdateAsync(user);
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}