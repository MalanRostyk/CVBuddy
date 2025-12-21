using CVBuddy.Models;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class CvController : Controller
    {
        private readonly CVBuddyContext _context;
        public CvController(CVBuddyContext c)
        {
            _context = c;
        }

        public IActionResult CreateCv()
        {
            return View();
        }
    }
}
