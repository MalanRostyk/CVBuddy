using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class UserController : HomeController
    {
        public UserController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _context.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var user = await _context.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userVm = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                OneAddress = user.OneAddress ?? new Address()
            };
            return View(userVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserViewModel formUser)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(u => u.OneAddress).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (_userManager.Users.Any(u => u.Email == formUser.Email && u.Id != user.Id))
                ModelState.AddModelError(nameof(formUser.Email), "Email already exists");

            if (_userManager.Users.Any(u => u.PhoneNumber == formUser.PhoneNumber && u.Id != user.Id))
                ModelState.AddModelError(nameof(formUser.PhoneNumber), "Phone number already exists");

            if (_userManager.Users.Any(u => u.UserName == formUser.UserName && u.Id != user.Id))
                ModelState.AddModelError(nameof(formUser.UserName), "User name already exists");

            if (!ModelState.IsValid)
                return View(formUser);

            user.FirstName = formUser.FirstName;
            user.LastName = formUser.LastName;

            if(formUser.UserName != user.UserName)
            {
                var userNameResult = await _userManager.SetUserNameAsync(user, formUser.UserName);
                if (!userNameResult.Succeeded)
                {
                    foreach (var error in userNameResult.Errors)
                        ModelState.AddModelError(nameof(formUser.UserName), error.Description);
                    return View(formUser);
                }
            }

            if (formUser.Email != user.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, formUser.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, formUser.Email, token);
                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError(nameof(formUser.Email), error.Description);
                    
                    return View(formUser);
                }
            }

            if (formUser.PhoneNumber != user.PhoneNumber)
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, formUser.PhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    foreach (var error in phoneResult.Errors)
                        ModelState.AddModelError(nameof(formUser.PhoneNumber), error.Description);
                    return View(formUser);
                }
            }
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            if (formUser.OneAddress != null)
            {
                if (user.OneAddress == null)
                {
                    var newAddress = new Address
                    {
                        Country = formUser.OneAddress.Country ?? "",
                        City = formUser.OneAddress.City ?? "",
                        Street = formUser.OneAddress.Street ?? "",
                        UserId = user.Id
                    };
                    user.OneAddress = newAddress;
                    _context.Add(newAddress);//fick null
                }
                else
                {
                    user.OneAddress.Country = formUser.OneAddress.Country ?? "";
                    user.OneAddress.City = formUser.OneAddress.City ?? "";
                    user.OneAddress.Street = formUser.OneAddress.Street ?? "";
                    //_context.Update(user.OneAddress);
                }
                await _context.SaveChangesAsync(); //ska vara här
            }
            //await _context.SaveChangesAsync();//ej här testar bara
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
