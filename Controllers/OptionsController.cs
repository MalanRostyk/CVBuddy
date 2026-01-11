using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                OptionsViewModel optViewModel = new();

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    throw new NullReferenceException("User could not be retrieved.");

                ViewBag.IsSetToDeactivated = user.IsDeactivated;
                ViewBag.HasSetProfilePrivate = user.HasPrivateProfile;
                optViewModel.IsDeactivated = ViewBag.IsSetToDeactivated;
                optViewModel.HasPrivateProfile = ViewBag.HasSetProfilePrivate;

                return View();
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> GetOptions(OptionsViewModel optModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                if (user == null)
                    throw new NullReferenceException("User could not be retrieved.");

                user.IsDeactivated = optModel.IsDeactivated;
                user.HasPrivateProfile = optModel.HasPrivateProfile;

                await _userManager.UpdateAsync(user);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error saving the changes to the database."});
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            
        }
    }
}