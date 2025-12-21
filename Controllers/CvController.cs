using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
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

        [HttpGet]
        public IActionResult CreateCv()
        {
            return View(new Cv());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCv(Cv cv)
        {
            await _context.Cvs.AddAsync(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
