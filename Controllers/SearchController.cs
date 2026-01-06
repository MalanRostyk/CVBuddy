using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class SearchController : HomeController
    {
        public SearchController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {

        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            //var users = _context.Users.AsQueryable();//gör så att man bygger frågan först, och sen kör den som filter i db, inte i c#
            //if (!string.IsNullOrWhiteSpace(searchTerm))
            //{
            //    users = users.Where(u =>
            //        (u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
            //        u.LastName.ToLower().Contains(searchTerm.ToLower())) &&
            //        u.IsDeactivated != true).ToList();
            //}

            //Måste göras stegvis för att annars kan man få users som har avaktiverade konton i tom söksträng

            List<User> users = new();

            if (User.Identity!.IsAuthenticated)
            {
                //Inloggad göm deactivated account
                users = await _context.Users
                .Where(u => !u.IsDeactivated)
                .ToListAsync();
            }
            else
            {
                //utloggad göm alla deactivated account och privata profiler
                users = await _context.Users
                .Where(u => !u.IsDeactivated && !u.HasPrivateProfile)
                .ToListAsync();
            }

            //if (User.Identity!.IsAuthenticated)
            //{
            //    users = await _context.Users
            //    .Where(u => !u.IsDeactivated)
            //    .ToListAsync();
            //}
            //else
            //{
            //    users = await _context.Users
            //    .Where(u => !u.IsDeactivated && !u.OneCv.IsPrivate)
            //    .ToListAsync();
            //}



            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users
                    .Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.LastName.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }
            return View(users);
        }
    }
}