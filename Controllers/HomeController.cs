using System.Diagnostics;
using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CVBuddy.Models.CVInfo;

namespace CVBuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly CVBuddyContext _context;
        public HomeController(CVBuddyContext c)
        {
            _context = c;
        }

        [Authorize]
        public IActionResult Index()
        {
            List<Cv> cvList = _context.Cvs.ToList();
            ViewBag.Cvn = cvList;
            ViewBag.Headline = "Preview Cvs";
            return View();
        }

        
    }
}
