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

        public IActionResult CreateCv()
        {
            ViewBag.Headline = "Create a Cv";
            ViewBag.EducationHLine = "Enter education details";
            ViewBag.ExperiencesHLine = "Enter experience details";
            ViewBag.SkillsHLine = "Enter skills details";
            return View(new Cv());
        }
    }
}
