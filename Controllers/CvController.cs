using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CVBuddy.Controllers
{
    public class CvController : Controller
    {
        private readonly CVBuddyContext _context;
        private readonly UserManager<User> _userManager;
        public CvController(UserManager<User> userManager, CVBuddyContext c)
        {
            _context = c;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateCv()
        {
            return View(new Cv());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCv(Cv cv)
        {

            if (cv.ImageFile == null || cv.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image");
                ViewBag.eror = "Please upload an image";
                return View(cv);
            }
            Debug.WriteLine("PASERADE IF SATSEN");
            var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
            Directory.CreateDirectory(uploadeFolder);

            var ext = Path.GetExtension(cv.ImageFile.FileName);//null
            var fileName = Guid.NewGuid().ToString() + ext;

            var filePath = Path.Combine(uploadeFolder, fileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await cv.ImageFile.CopyToAsync(stream);
            }
            cv.ImageFilePath = "/CvImages/" + fileName;

            //Tilldela user id till cv för realtion
            cv.UserId = _userManager.GetUserId(User);

            await _context.Cvs.AddAsync(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
