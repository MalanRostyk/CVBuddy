using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.OneAddress)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    throw new NullReferenceException("User could not be found.");

                var userVm = new UserViewModel                      //skapar en UserViewModel objekt för att skicka till vyn
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AddressVm = user.OneAddress == null             //kollar om user har en adress i db
                    ? null                                          //om ingen adress finns, sätt AddressVm till null
                    : new AddressViewModel                          //om adress finns, skapa en ny AddressViewModel
                    {                                               //och fyll den med värden från user.OneAddress
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
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserViewModel formUser)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    return RedirectToAction("Login", "Account");

                if (_userManager.Users.Any(u => u.Email == formUser.Email && u.Id != user.Id))              //kollar om någon annan användare har samma email
                    ModelState.AddModelError(nameof(formUser.Email), "Email already exists");               //om true, lägg till ett felmeddelande i ModelState

                if (_userManager.Users.Any(u => u.PhoneNumber == formUser.PhoneNumber && u.Id != user.Id))
                    ModelState.AddModelError(nameof(formUser.PhoneNumber), "Phone number already exists");

                if (_userManager.Users.Any(u => u.UserName == formUser.UserName && u.Id != user.Id))
                    ModelState.AddModelError(nameof(formUser.UserName), "User name already exists");

                if (formUser.AddressVm != null && formUser.AddressVm.IsPartiallyFilled())                   //kollar om adressdelen är delvis ifylld
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
                {                                                               //kollar om minst ett fält är iffylt, om villkoret är true, uppdaterar vi elle skapar adressen
                    if (user.OneAddress == null)                                //kollar om user redan har en adress i db, om true ingen adress finns än
                    {
                        user.OneAddress = new Address                           //skapar ett nytt Address objekt
                        {
                            UserId = user.Id                                    //sätter UserId till users id
                        };
                        _context.Add(user.OneAddress);                          //lägger nya adressobjektet i dbcontext så att ef core vat att det ska sparas i db vid savechanges
                    }
                    user.OneAddress.Country = formUser.AddressVm.Country;       //uppdaterar adressfältet med värden från formUser
                    user.OneAddress.City = formUser.AddressVm.City;             //om adressen redan fanns så uppdateras de fält
                    user.OneAddress.Street = formUser.AddressVm.Street;         //omadressen är ny så tilldelas värdena på den nyss skapade raden
                }
                else                                                            //else körs om user länade adressfält tomma
                {
                    if (user.OneAddress != null)                                //kollar om user hade en adress i db
                    {
                        _context.Remove(user.OneAddress);                       //lägger adressobjektet i dbcontext så att ef core vet att den ska raderas från db vid savechanges
                        user.OneAddress = null;                                 //sätts null, user har ingen relation med adressen
                    }
                }
                await _context.SaveChangesAsync();                              //Identity bygger på säkerhet och token-baserade ändringar, microsoft tvingar oss att 
                                                                                //använda metoder som identity klassen har, de används för att lagra de fält på rätt sätt
                return RedirectToAction("GetUser", "User");                     //med security stamp, unikhet, trigga rätt event osv
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
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword cp)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(cp);

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    throw new NullReferenceException("User could was not found.");

                var result = await _userManager.ChangePasswordAsync(user, cp.CurrentPassword, cp.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(cp);
                }

                await _signInManager.RefreshSignInAsync(user);

                return RedirectToAction("GetUser", "User");
            }
            catch(NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error trying to save your changes." });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
        }
    }
}