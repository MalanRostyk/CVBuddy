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
            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CvImages");

            Directory.CreateDirectory(uploadFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(cv.ImageFile.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await cv.ImageFile.CopyToAsync(stream);
            }
            cv.ImageFilePath = "/CvImages/" + fileName;

            await _context.Cvs.AddAsync(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
