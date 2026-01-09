using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
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
            var user = await _userManager.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        //[HttpGet]
        //public async Task<IActionResult> UpdateUser(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    return View(user);
        //}

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            try
            {
                var user = await _context.Users
                .Include(u => u.OneAddress)
                .FirstOrDefaultAsync(u => u.Id == id);

                //if (user == null)
                //{
                //    //return RedirectToAction("Login", "Account");

                //    //David - jag la till detta, men ingen annan stans såg det bara när jag testade efter mina ändringar och jag kom till login när jag skulle ändra mina uppgifter
                //    return NotFound("User could not be found.");
                //}

                if (user == null)
                    throw new NullReferenceException("User could not be found.");

                var userVm = new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AddressVm = user.OneAddress == null
                    ? null
                    : new AddressViewModel
                    {
                        Country = user.OneAddress.Country,
                        City = user.OneAddress.City,
                        Street = user.OneAddress.Street
                    }
                };
                return View(userVm);
            }
            catch(NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(User formUser)
        //{
        //    var user = await _userManager.Users
        //        .Include(u => u.OneAddress)
        //        .FirstOrDefaultAsync(u => u.Id == formUser.Id);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    user.FirstName = formUser.FirstName;
        //    user.LastName = formUser.LastName;

        //    if (formUser.Email != user.Email)
        //    {
        //        var token = await _userManager.GenerateChangeEmailTokenAsync(user, formUser.Email);
        //        var emailResult = await _userManager.ChangeEmailAsync(user, formUser.Email, token);
        //        if (!emailResult.Succeeded)
        //        {
        //            ModelState.AddModelError("", "Could not update email");
        //            return View(formUser);
        //        }
        //    }

        //    if (formUser.PhoneNumber != user.PhoneNumber)
        //    {
        //        await _userManager.SetPhoneNumberAsync(user, formUser.PhoneNumber);

        //    }
        //    await _context.SaveChangesAsync();

        //    if (formUser.OneAddress != null)
        //    {
        //        if (user.OneAddress == null)
        //        {
        //            user.OneAddress = new Address
        //            {
        //                Country = formUser.OneAddress.Country,
        //                City = formUser.OneAddress.City,
        //                Street = formUser.OneAddress.Street,
        //                UserId = user.Id
        //            };
        //            await _context.Addresses.AddAsync(user.OneAddress);//fick null
        //        }
        //        else
        //        {
        //            user.OneAddress.Country = formUser.OneAddress.Country;
        //            user.OneAddress.City = formUser.OneAddress.City;
        //            user.OneAddress.Street = formUser.OneAddress.Street;
        //            await _context.SaveChangesAsync();
        //        }
        //    }

        //    return RedirectToAction("GetUser", "User");
        //    //Identity bygger på säkerhet och token-baserade ändringar, microsoft tvingar oss att 
        //    //använda metoder som identity klassen har, de används för att lagra de fält på rätt sätt
        //    //med security stamp, unikhet, trigga rätt event osv
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserViewModel formUser)
        {
            //var user = await _context.Users.Include(u => u.OneAddress).FirstOrDefaultAsync(u => u.Id == formUser.UserId);
            var user = await _userManager.GetUserAsync(User);

            try
            {
                if (user == null)
                    return RedirectToAction("Login", "Account");

                if (_userManager.Users.Any(u => u.Email == formUser.Email && u.Id != user.Id))
                    ModelState.AddModelError(nameof(formUser.Email), "Email already exists");

                if (_userManager.Users.Any(u => u.PhoneNumber == formUser.PhoneNumber && u.Id != user.Id))
                    ModelState.AddModelError(nameof(formUser.PhoneNumber), "Phone number already exists");

                if (_userManager.Users.Any(u => u.UserName == formUser.UserName && u.Id != user.Id))
                    ModelState.AddModelError(nameof(formUser.UserName), "User name already exists");

                if (formUser.AddressVm != null && formUser.AddressVm.IsPartiallyFilled())
                {
                    ModelState.AddModelError("AddressVm.Country", "All address fields are required");
                    ModelState.AddModelError("AddressVm.City", "All address fields are required");
                    ModelState.AddModelError("AddressVm.Street", "All address fields are required");

                }

                if (!ModelState.IsValid)
                    return View(formUser);

                user.FirstName = formUser.FirstName;
                user.LastName = formUser.LastName;

                if (formUser.UserName != user.UserName)
                    await _userManager.SetUserNameAsync(user, formUser.UserName);

                if (formUser.Email != user.Email)
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, formUser.Email);
                    await _userManager.ChangeEmailAsync(user, formUser.Email, token);
                }

                if (formUser.PhoneNumber != user.PhoneNumber)
                    await _userManager.SetPhoneNumberAsync(user, formUser.PhoneNumber);

                //kontrollera om användaren har fyllt i adressfälten i formuläret
                if (formUser.AddressVm != null && !formUser.AddressVm.IsEmpty())//kollar om adressdelen finns i UserViewModel
                {                                                           //kollar om minst ett fält är iffylt, om villkoret är true, uppdaterar vi elle skapar adressen
                    if (user.OneAddress == null)                            //kollar om user redan har en adress i db, om true ingen adress finns än
                    {
                        user.OneAddress = new Address                       //skapar ett nytt Address objekt
                        {
                            UserId = user.Id                                //sätter UserId till users id
                        };
                        _context.Add(user.OneAddress);                      //lägger nya adressobjektet i dbcontext så att ef core vat att det ska sparas i db vid savechanges
                    }
                    user.OneAddress.Country = formUser.AddressVm.Country;     //uppdaterar adressfältet med värden från formUser
                    user.OneAddress.City = formUser.AddressVm.City;           //om adressen redan fanns så uppdateras de fält
                    user.OneAddress.Street = formUser.AddressVm.Street;       //omadressen är ny så tilldelas värdena på den nyss skapade raden
                }
                else                                                        //else körs om user länade adressfält tomma
                {
                    if (user.OneAddress != null)                             //kollar om user hade en adress i db
                    {
                        _context.Remove(user.OneAddress);                   //lägger adressobjektet i dbcontext så att ef core vet att den ska raderas från db vid savechanges
                        user.OneAddress = null;                             //sätts null, user har ingen relation med adressen
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("GetUser", "User");
                //Identity bygger på säkerhet och token-baserade ändringar, microsoft tvingar oss att 
                //använda metoder som identity klassen har, de används för att lagra de fält på rätt sätt
                //med security stamp, unikhet, trigga rätt event osv
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error trying to save your changes."});
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }

            
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
