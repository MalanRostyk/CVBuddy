using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class UserController : HomeController
    {

        private readonly SignInManager<User> _signInManager;
        public UserController(UserManager<User> u, CVBuddyContext c, SignInManager<User> signInManager) : base(u, c)
        {
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
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
        public async Task<IActionResult> UpdateUser(User formUser)
        {
            var user = await _userManager.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.Id == formUser.Id);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.FirstName = formUser.FirstName;
            user.LastName = formUser.LastName;

            if (formUser.Email != user.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, formUser.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, formUser.Email, token);
                if (!emailResult.Succeeded)
                {
                    ModelState.AddModelError("", "Could not update email");
                    return View(formUser);
                }
            }

            if (formUser.PhoneNumber != user.PhoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, formUser.PhoneNumber);

            }

            if (formUser.OneAddress != null)
            {
                if (user.OneAddress == null)
                {
                    user.OneAddress = new Address
                    {
                        Country = formUser.OneAddress.Country,
                        City = formUser.OneAddress.City,
                        Street = formUser.OneAddress.Street,
                        UserId = user.Id
                    };
                    await _context.Addresses.AddAsync(user.OneAddress);//fick null
                }
                else
                {
                    user.OneAddress.Country = formUser.OneAddress.Country;
                    user.OneAddress.City = formUser.OneAddress.City;
                    user.OneAddress.Street = formUser.OneAddress.Street;
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("GetUser", "User");
            //Identity bygger på säkerhet och token-baserade ändringar, microsoft tvingar oss att 
            //använda metoder som identity klassen har, de används för att lagra de fält på rätt sätt
            //med security stamp, unikhet, trigga rätt event osv
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword cp)
        {
            if (!ModelState.IsValid)
                return View(cp);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            var result = await _userManager.ChangePasswordAsync(user, cp.CurrentPassword, cp.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(cp);
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("GetUser", "User");
            //skapade en klass just för att ändra lösenord, eftersom man ska använda ChangePasswordAsync metoden, detta är rätt sätt att byta 
            //lösenord, den validerar och jämför om nuvarande lösenord användaren skrev in stämmer, och den hashar och har stämpel efteråt
        }
    }
}
