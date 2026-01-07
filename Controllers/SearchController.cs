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
            List<User> users = new();
            users = _context.Users
                .Include(u => u.OneAddress)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Experiences)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Education)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Skills)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Certificates)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Interests)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Interests)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.PersonalCharacteristics)
                .ToList();

            if (!User.Identity!.IsAuthenticated)
            {
                users = users.Where(u => !u.IsDeactivated).ToList();
            }
            else
            {
                users = users.Where(u => !u.IsDeactivated && !u.HasPrivateProfile).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users.Where(
                    u => u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.LastName.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }

            ViewBag.ResultCount = users.Count() == 0;
            return View(users);

            //var result = new List<User>();

            //if (!string.IsNullOrWhiteSpace(searchTerm))
            //{
            //    result = _context.Users.Where(u =>
            //        u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
            //        u.LastName.ToLower().Contains(searchTerm.ToLower()))
            //        .ToList();
            //    if (result.Count > 0)
            //    {

            //        if (User.Identity!.IsAuthenticated)
            //        {
            //            result = await _context.Users
            //            .Where(u => !u.IsDeactivated)
            //            .ToListAsync();
            //            return View(result);
            //        }
            //        else
            //        {
            //            result = await _context.Users
            //            .Where(u => !u.IsDeactivated && !u.HasPrivateProfile)
            //            .ToListAsync();
            //            return View(result);
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.NoResult = "No results found";
            //    }
            //}
            //return View(result);
        }
    }
}