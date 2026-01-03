using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class SearchController : HomeController
    {
        public SearchController(UserManager<User> u, CVBuddyContext c) : base(u, c)
        {

        }
        [HttpGet]
        public IActionResult Search(string searchTerm)
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
            var users = _context.Users
                .Where(u => u.IsDeactivated != true)
                .ToList();

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