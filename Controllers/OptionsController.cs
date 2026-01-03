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
        public OptionsController(UserManager<User> u, CVBuddyContext c) : base(u, c)
        {
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOptions()
        {
            OptionsViewModel optViewModel = new();

            var user = await _userManager.GetUserAsync(User);

            ViewBag.IsSetToPrivate = user.IsPrivate;
            ViewBag.IsSetToDeactivated = user.IsDeactivated;
            optViewModel.IsPrivate = ViewBag.IsSetToPrivate;
            optViewModel.IsDeactivated = ViewBag.IsSetToDeactivated;

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> GetOptions(OptionsViewModel optModel)
        {

            var user = await _userManager.GetUserAsync(User);

            user.IsPrivate = optModel.IsPrivate;
            user.IsDeactivated = optModel.IsDeactivated;

            await _userManager.UpdateAsync(user);
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}