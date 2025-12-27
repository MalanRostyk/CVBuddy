using Azure.Messaging;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            ViewBag.Headline = "Cv";
            ViewBag.HeadlineExperiences = "Experiences";
            ViewBag.HeadlineEducation = "Education";
            ViewBag.HeadlineSkill = "Skills";
            ViewBag.HeadlineCertificates = "Certificates";
            ViewBag.HeadlinePersonalCharacteristics = "Personal Characteristics";
            ViewBag.HeadlineInterest = "Interests";

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

        [HttpGet]
        public async Task<IActionResult> ReadCv(int? Cid) //måste heta exakt samma som asp-route-Cid="@item.OneCv.Cid". Detta Cid är Cid från det Cv som man klickar på i startsidan
        {
            Cv? cv;
            var userId = _userManager.GetUserId(User);
            if (Cid.HasValue)//Om man klickade på ett cv i Index, följer ett Cid med via asp-route-Cid, men om man klickar på My Cv(har ej asp-route...) så körs else blocket, eftersom inget Cid följer med
            {
                cv = _context.Cvs
                    .Include(cv => cv.Education)
                    .Include(cv => cv.Experiences)
                    .Include(cv => cv.Skills)
                    .Include(cv => cv.Certificates)
                    .Include(cv => cv.PersonalCharacteristics)
                    .Include(cv => cv.Interests)
                    .Include(cv => cv.OneUser)
                    .Include(cv => cv.CvProjects)
                    .ThenInclude(cp => cp.OneProject)//Inkludera relaterade project från cvProjects
                    .FirstOrDefault(cv => cv.Cid == Cid); //inkludera all detta för cv med Cid ett visst id och med first or default visas 404 not found istället för krasch
               
                await _context.Database.ExecuteSqlRawAsync("UPDATE Cvs SET ReadCount = ReadCount + 1 WHERE Cid = " + Cid); //Inkrementera ReadCount varje gång See Cv klickas
            }
            else//I else hämtas den inloggade användarens Cv, för "My Cv"
            {
                

                cv = _context.Cvs
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
            }
                
            

            if (cv == null)
                NotFound(); //Error meddelande som stoppar krasch, not found 404

            ViewBag.Headline = "Cv";
            ViewBag.HeadlineExperiences = "Experiences";
            ViewBag.HeadlineEducation = "Education";
            ViewBag.HeadlineSkill = "Skills";
            ViewBag.HeadlineCertificates = "Certificates";
            ViewBag.HeadlinePersonalCharacteristics = "Personal Characteristics";
            ViewBag.HeadlineInterest = "Interests";
            ViewBag.HeadlineProjects = "Projects";
            return View(cv);
        }
    }
}
