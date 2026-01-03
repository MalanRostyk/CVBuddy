using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CVBuddy.Controllers
{
    public class HomeController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly CVBuddyContext _context;
        public HomeController(UserManager<User> u, CVBuddyContext c)
        {
            _userManager = u;
            _context = c;
        }

        public async Task<IActionResult> Index()
        {

            var users = await _context.Users //Mindre kod gör samma utan valideringen, OM NÅGOT SAKNAS FÖR USER, SÅ MÅSTE DET INKLUDERAS
                                             //, IdentityUsers fält är inkluderade genom Arvet, men bara dem som är Mappade
                .Where(u => u.IsDeactivated != true)
                .Include(u => u.OneCv)
                .Include(u => u.ProjectUsers)
                .ThenInclude(pu => pu.Project)
                .ToListAsync();

            if (!User.Identity!.IsAuthenticated)
            {
                users = users
                    .Where(u => u.OneCv?.IsPrivate != true)
                    .ToList();
            }
            ViewBag.CvsExists = false;
            foreach(var user in users)
            {
                if(user.OneCv != null)
                {
                    ViewBag.CvsExists = true;
                }
            }
         
            

            var usersCv = await GetLoggedInUsersCvAsync();
            ViewBag.HasCv = usersCv != null;

            ViewBag.CvIndexHeadline = "Recent Cvs";

            return View(users);//För att ge Users till Index view, så Model inte är NULL
        }

        private async Task<Cv> GetLoggedInUsersCvAsync()
        {

            var userId = _userManager.GetUserId(User);
            Cv? cv = _context.Cvs
                    .Include(cv => cv.Education)
                    .Include(cv => cv.Experiences)
                    .Include(cv => cv.Skills)
                    .Include(cv => cv.Certificates)
                    .Include(cv => cv.PersonalCharacteristics)
                    .Include(cv => cv.Interests)
                    .Include(cv => cv.OneUser)
                    .Include(cv => cv.CvProjects)
                    .ThenInclude(cp => cp.OneProject)
                    .FirstOrDefault(cv => cv.UserId == userId); //Kan göra cv till null ändå
            if (cv == null)
                NotFound();

            return cv;
        }

    }
}
