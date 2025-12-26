using Azure.Messaging;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CVBuddy.Controllers
{
    public class CvController : HomeController
    {
        public CvController(UserManager<User> u, CVBuddyContext c) : base(u, c)
        {
        }

        [HttpGet]
        public IActionResult CreateCv()
        {
            bool cvFound = false;
            var cvsList = _context.Cvs.Select(cv => cv.UserId).ToList(); //Alla cvns userId
            var userId = _userManager.GetUserId(User);

            foreach (var cvUserId in cvsList)
            {
                if (cvUserId == userId)
                    cvFound = true;
            }
            if (cvFound)
            {
                return RedirectToAction("Index", "Home");//Användaren kommer tillbaka till startsidan om de redan har ett cv, Lägg till ett meddelande eller avaktiver knapp eller något, Testad och funkar!
            }
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
