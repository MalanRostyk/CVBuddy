using Azure.Messaging;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

            var cvsList = _context.Cvs.Select(cv => cv.UserId).ToList(); //Alla cvns userId
            var userId = _userManager.GetUserId(User);
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


                try
                {
                    var usersCv = await GetLoggedInUsersCvAsync();
                    if (cv?.UserId != usersCv?.UserId)
                        await _context.Database.ExecuteSqlRawAsync("UPDATE Cvs SET ReadCount = ReadCount + 1 WHERE Cid = " + Cid); //Inkrementera ReadCount varje gång See Cv klickas
                }
                catch(NullReferenceException noCv)
                {
                    Debug.WriteLine("User has no cv, or cv was not found");
                }
                    

            }
            else//I else hämtas den inloggade användarens Cv, för "My Cv"
            {
                cv = await GetLoggedInUsersCvAsync();
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

        [HttpGet]
        public async Task<IActionResult> UpdateCv()
        {
            ViewBag.Headline = "Cv";
            ViewBag.HeadlineImage = "Image";
            ViewBag.HeadlineExperiences = "Experiences";
            ViewBag.HeadlineEducation = "Education";
            ViewBag.HeadlineSkill = "Skills";
            ViewBag.HeadlineCertificates = "Certificates";
            ViewBag.HeadlinePersonalCharacteristics = "Personal Characteristics";
            ViewBag.HeadlineInterest = "Interests";

            var cv = await GetLoggedInUsersCvAsync();
            if(cv != null)
            {
                return View(cv);
            }
            else
            {
                return RedirectToAction("CreateCv"); //Om man inte har ett cv än så kommer man till CreateCv
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCv(Cv cv)
        {
            var cvOldVersion = await GetLoggedInUsersCvAsync();

            if (cvOldVersion == null)
                return NotFound();

            //if (!ModelState.IsValid)
            //    return View(cvOldVersion);

            cvOldVersion.ReadCount = cv.ReadCount;
            cvOldVersion.UserId = cv.UserId;
            cvOldVersion.ImageFilePath = cv.ImageFilePath;

            //Tilldela nya värdena från ViewModel objektet till det trackade Cvt från db

            //Experiences
            for(int i = 0; i < cvOldVersion.Experiences.Count; i++)
            {
                cvOldVersion.Experiences[i].Title = cv.Experiences[i].Title;
                cvOldVersion.Experiences[i].Description = cv.Experiences[i].Description;
                cvOldVersion.Experiences[i].Company = cv.Experiences[i].Company;
                cvOldVersion.Experiences[i].StartDate = cv.Experiences[i].StartDate;
                cvOldVersion.Experiences[i].EndDate = cv.Experiences[i].EndDate;
            }

            //Education
            cvOldVersion.Education.HighSchool = cv.Education.HighSchool;
            cvOldVersion.Education.HSProgram = cv.Education.HSProgram;
            cvOldVersion.Education.HSDate = cv.Education.HSDate;

            cvOldVersion.Education.Univeristy = cv.Education.Univeristy;
            cvOldVersion.Education.UniProgram = cv.Education.UniProgram;
            cvOldVersion.Education.UniDate = cv.Education.UniDate;

            //Skills
            for (int i = 0; i < cvOldVersion.Skills.Count; i++)
            {
                cvOldVersion.Skills[i].ASkill = cv.Skills[i].ASkill;
                cvOldVersion.Skills[i].Description = cv.Skills[i].Description;
                cvOldVersion.Skills[i].Date = cv.Skills[i].Date;
            }

            //Interests
            for (int i = 0; i < cvOldVersion.Interests.Count; i++)
            {
                cvOldVersion.Interests[i].InterestName = cv.Interests[i].InterestName;
            }


            //Certificates
            for (int i = 0; i < cvOldVersion.Certificates.Count; i++)
            {
                cvOldVersion.Certificates[i].CertName = cv.Certificates[i].CertName;
            }


            //PersonalCharacteristics
            for (int i = 0; i < cvOldVersion.PersonalCharacteristics.Count; i++)
            {
                cvOldVersion.PersonalCharacteristics[i].CharacteristicName = cv.PersonalCharacteristics[i].CharacteristicName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCv(int Cid)
        {

            ViewBag.Headline = "Delete Cv";
            ViewBag.WarningMessage = "Are you sure you wan't to delete your Cv? This will permanently delete your Cv but" +
                ", none of the projects you created will be automatically connected to your new Cvs. You will have to find them and participate in them again"; //I felmeddelandet visas vad planen för projekten är
            //Cv cv = _context.Cvs.Find(Cid); //Ska inte använda Find för att annars får man inte med relaterade rader till Cv!!!!!!
            Cv cv = await GetLoggedInUsersCvAsync();
            return View(cv);
        }

        [HttpPost]
        public IActionResult DeleteCv(Cv cv)
        {
            _context.Cvs.Remove(cv);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
