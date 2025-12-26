using System.Diagnostics;
using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize]
        public IActionResult Index()
        {
            //Allt detta för att en Users OneCv är null i Viewens foreach, så tilldelar manuellt en User sin OneCv
            var users = _context.Users.ToList();
            var cvs = _context.Cvs.ToList();

            foreach(var user in users)
            {
                foreach(var cv in cvs)
                {
                    if (user.OneCv.Cid == cv.Cid)
                        user.OneCv.Cid = cv.Cid;
                }
            }

            ViewBag.CvIndexHeadline = "Recent Cvs";
            return View(users);//För att ge Users till Index view, så Model inte är NULL
        }

    }
}
