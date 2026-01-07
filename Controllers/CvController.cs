using Azure.Messaging;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;

namespace CVBuddy.Controllers
{
    public class CvController : HomeController
    {
        public CvController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        private bool IsValidExtension(string extension)
        {
            string ext = extension.ToLower();
            if (ext == ".png" || ext == ".jpg" || ext == ".jfif" || ext == ".webp")
                return true;
            return false;
        }

        private async Task<Cv> GetLoggedInUsersCvAsync()
        {
            if (!(User.Identity!.IsAuthenticated))
                return new();
                
            //Ingen transaktion, Select statements(dvs, await _context...) är atomära, om ej i sekvens, behövs ej transaktion
            var userId = _userManager.GetUserId(User); //Datan kommer från db men man läser inte från Db i realtid, utan man hämtar det från inloggningscontexten, via ClaimsPrincipal, dvs user laddas vid inloggningen, läggs till i ClaimsPrincipal. Kan ej vara opålitlig. Därmet endast en read operation görs
            //Cv? cv = await _context.Cvs
            //        .Include(cv => cv.Education)
            //        .Include(cv => cv.Experiences)
            //        .Include(cv => cv.Skills)
            //        .Include(cv => cv.Certificates)
            //        .Include(cv => cv.PersonalCharacteristics)
            //        .Include(cv => cv.Interests)
            //        .Include(cv => cv.OneUser)
            //        .Include(cv => cv.CvProjects)
            //        .ThenInclude(cp => cp.OneProject)
            //        .FirstOrDefaultAsync(cv => cv.UserId == userId); //Kan göra cv till null ändå
            Cv? cv = await _context.Cvs
                    .Include(cv => cv.Education)
                    .Include(cv => cv.Experiences)
                    .Include(cv => cv.Skills)
                    .Include(cv => cv.Certificates)
                    .Include(cv => cv.PersonalCharacteristics)
                    .Include(cv => cv.Interests)
                    .Include(cv => cv.OneUser)
                    .ThenInclude(oneUser => oneUser!.ProjectUsers)
                    .FirstOrDefaultAsync(cv => cv.UserId == userId); //Kan göra cv till null ändå
            if(cv != null)
                cv.UsersProjects = await GetProjectsUserHasParticipatedIn(userId!);
            //if (cv == null) // Ska trigga try catch i action metod, INTE I PRIVAT HELPER METOD
            //    throw new NullReferenceException("Users Cv was not found");

            return cv;
        }

        private async Task<List<Project>> GetProjectsUserHasParticipatedIn(string userId)
        {
            List<Project> projectList = await _context.ProjectUsers
                .Where(pu => pu.UserId == userId)
                .Join(
                _context.Projects,
                pu => pu.ProjId,
                p => p.Pid,
                (pu, p) => p)
                .ToListAsync(); ;

            return projectList;
        }

        private bool IsValidFileSize(long fileSizeInBits)
        {
            long fiveMB = 5 * 1024 * 1024;

            if (fileSizeInBits <= fiveMB && fileSizeInBits != 0) //Kan ej vara null, longs standardvärde är 0 
                return true;
            return false;
        }

        private bool DeleteOldImageLocally(Cv cvOld)
        {
            string[]? cvOldFileImageNameArray = null;

            try
            {
                if (cvOld.ImageFilePath != null)
                {
                    cvOldFileImageNameArray = cvOld.ImageFilePath.Split("/");

                    if (cvOldFileImageNameArray.Length != 0)
                    {

                        string oldCvFileName = cvOldFileImageNameArray[cvOldFileImageNameArray.Length - 1];

                        string finalCvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages", oldCvFileName);

                        //Återskapar oldCvs gamla filepath för att den sparas med "c:\CvImages\Filnamn.Ext", Eftersom att sökvägen är relativ, så vi måste ge den CurrentDirectory och wwwroot för att den ska hittas för att raderas
                        if (System.IO.File.Exists(finalCvFilePath))
                        {
                            System.IO.File.Delete(finalCvFilePath);
                            Debug.WriteLine("Old image was found. Bör ha try catch oså!");
                            return true;
                        }
                    }
                }
            }catch(Exception e)
            {
                throw;
            }
            
            return false;
        }

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateCv()
        {
            //Ej ViewBags för att när man inte skapar cv korrekts så hamnar vi i samma view via samma action metod men ViewBag sätts inte i sådanna fall
            //ViewBag.Headline = "Cv";
            //ViewBag.HeadlineExperiences = "Experiences";
            //ViewBag.HeadlineEducation = "Education";
            //ViewBag.HeadlineSkill = "Skills";
            //ViewBag.HeadlineCertificates = "Certificates";
            //ViewBag.HeadlinePersonalCharacteristics = "Personal Characteristics";
            //ViewBag.HeadlineInterest = "Interests";

            //Ej behov av transaktion, av samma anledning som för GetLoggedInUsersCvAsync()
            var cvsList = await _context.Cvs.Select(cv => cv.UserId).ToListAsync(); //Alla cvns userId
            var userId = _userManager.GetUserId(User);
            return View(new Cv());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCv(Cv cv)
        {

            if (!ModelState.IsValid)
                return View(cv);
            try
            {
                if (cv.ImageFile == null || cv.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Please upload an image");
                    ViewBag.eror = "Please upload an image";
                    return View(cv);
                }

                var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                Directory.CreateDirectory(uploadeFolder);

                var ext = Path.GetExtension(cv.ImageFile.FileName);//null

                //if (!IsValidExtension(ext))
                //    return View(cv);

                var fileName = Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(uploadeFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
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
            catch (Exception e)
            {
                //Om transaktionen inte lyckades, tas tillagda bilden bort lokalt här i samband med en rollback
                DeleteOldImageLocally(cv);

                return NotFound(e);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadCv(int? Cid) //måste heta exakt samma som asp-route-Cid="@item.OneCv.Cid". Detta Cid är Cid från det Cv som man klickar på i startsidan
        {
            Cv? cv;
            try
            {
                if (Cid.HasValue)//Om man klickade på ett cv i Index, följer ett Cid med via asp-route-Cid, men om man klickar på My Cv(har ej asp-route...) så körs else blocket, eftersom inget Cid följer med
                {
                    //  Är inte Logged in Users cv som ska hämtas här, detta cv är det som ska visas
                    //cv = await _context.Cvs
                    //.Include(cv => cv.Education)
                    //.Include(cv => cv.Experiences)
                    //.Include(cv => cv.Skills)
                    //.Include(cv => cv.Certificates)
                    //.Include(cv => cv.PersonalCharacteristics)
                    //.Include(cv => cv.Interests)
                    //.Include(cv => cv.OneUser)
                    //.Include(cv => cv.CvProjects)//Relationen finns inte längre
                    //.ThenInclude(cp => cp.OneProject)//Inkludera relaterade project från cvProjects
                    //.FirstOrDefaultAsync(cv => cv.Cid == Cid); //inkludera all detta för cv med Cid ett visst id och med first or default visas 404 not found istället för krasch
                    cv = await _context.Cvs
                    .Include(cv => cv.Education)
                    .Include(cv => cv.Experiences)
                    .Include(cv => cv.Skills)
                    .Include(cv => cv.Certificates)
                    .Include(cv => cv.PersonalCharacteristics)
                    .Include(cv => cv.Interests)
                    .Include(cv => cv.OneUser)
                    .ThenInclude(oneUser => oneUser!.ProjectUsers)
                    .FirstOrDefaultAsync(cv => cv.Cid == Cid);

                    if(cv != null && User.Identity!.IsAuthenticated) //Måste vara inloggad för att se projekt i cv-sida
                        cv.UsersProjects = await GetProjectsUserHasParticipatedIn(cv.UserId!);

                    var usersCv = await GetLoggedInUsersCvAsync();//Hämtar eget cv för att det ska användas för att jämföra om det är den inloggade användares cv
                    ViewBag.NotLoggedInUsersCv = cv?.UserId != usersCv?.UserId; //bool för att gömma Delete på cvs som inte är den inloggade användaren
                    if (ViewBag.NotLoggedInUsersCv)
                    {
                        //Ingen transaktion behövd, Enskild Update-statements är atomära, sätter Row lock. Applikationen använder en lokal databas, alltså inga samtidiga updates kommer göras här. EJ ett problem
                        await _context.Database.ExecuteSqlRawAsync("UPDATE Cvs SET ReadCount = ReadCount + 1 WHERE Cid = " + Cid); //Inkrementera ReadCount varje gång See Cv klickas
                    }
                }
                else//I else hämtas den inloggade användarens Cv, för "My Cv"
                {
                    if (!User.Identity!.IsAuthenticated)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        cv = await GetLoggedInUsersCvAsync();
                        if (cv?.OneUser == null)
                            return RedirectToAction("CreateCv", "Cv");
                    }
                        
                }

                ViewBag.HasSetPrivateProfile = cv?.OneUser!.HasPrivateProfile;

                //För headlines om det finns något att visa under headlinen
                ViewBag.Headline = "Cv";
                

                ViewBag.CvOwnerFullName = " - " + cv?.OneUser.GetFullName();

                //Experiences
                if (cv?.Experiences.Count > 0)
                {
                    ViewBag.HeadlineExperiences = "Experiences";
                }


                //Education
                ViewBag.hasUniversity = false;
                ViewBag.hasHighSchool = false;
                var cvEdu = cv?.Education;

                if (cvEdu?.HighSchool != null || cvEdu?.HSProgram != null || cvEdu?.HSDate != null)
                    ViewBag.hasHighSchool = true;

                if (cvEdu?.Univeristy != null || cvEdu?.UniProgram != null || cvEdu?.UniDate != null)
                    ViewBag.hasUniversity = true;

                ViewBag.hasEducation = ViewBag.hasUniversity || ViewBag.hasHighSchool;
                if (ViewBag.hasEducation)
                    ViewBag.HeadlineEducation = "Education";

                //Skills
                if (cv?.Skills.Count > 0)
                {
                    ViewBag.HeadlineSkill = "Skills";
                }

                //Certificates
                if (cv?.Certificates.Count > 0)
                {
                    ViewBag.HeadlineCertificates = "Certificates";
                    ViewBag.HeadlineCertificatesSmall = "My Certificates";
                }

                //Personal Characteristics
                if (cv?.PersonalCharacteristics.Count > 0)
                {
                    ViewBag.HeadlinePersonalCharacteristics = "Personal Characteristics";
                    ViewBag.HeadlinePersonalCharacteristicsSmall = "My personal characteristics";

                }

                //Interests
                if (cv?.Interests.Count > 0)
                {
                    ViewBag.HeadlineInterest = "Interests";
                    ViewBag.HeadlineInterestSmall = "These are my interests";
                }

                //Projects
                //if (cv?.CvProjects.Count > 0)
                //{
                //    ViewBag.HeadlineProjects = "Projects";
                //    ViewBag.HeadlineProjectsSmall = "I have participated in these projects";
                //}
                if (cv?.UsersProjects.Count > 0)
                {
                    ViewBag.HeadlineProjects = "Projects";
                    ViewBag.HeadlineProjectsSmall = "I have participated in these projects";
                }
            }
            catch(Exception e)
            {
                return NotFound(e);
            }

            return View(cv);
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
            
            if (cv != null)
            {
                //----------------------------------------------------------------------------------------------------------------------------ViewBag.IsPrivate = cv.IsPrivate;
                return View(cv);
            }
            else
            {
                return NotFound();
            }
        }

        

        [HttpPost]
        public async Task<IActionResult> UpdateCv(Cv cv)
        {
            if (!ModelState.IsValid)
                return View(cv);

            var cvOldVersion = await GetLoggedInUsersCvAsync();

            if (ModelState.IsValid && cv.Education.DateOnlyEntered("Both"))
                Console.WriteLine("BIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifhBIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIISDFÅosifh ");

            if (cvOldVersion == null)
                return NotFound();

            //----------------------------------------------------------------------------------------------------------------------------cvOldVersion.IsPrivate = cv.IsPrivate;
            cvOldVersion.ReadCount = cv.ReadCount;
            cvOldVersion.UserId = cv.UserId;
            
            if(cv.ImageFile != null)
            {

                if (!IsValidFileSize(cv.ImageFile.Length))
                    return View(cv);

                //var oldImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");

                //accept räcker inte måste validera extension på server nivå

                var extension = Path.GetExtension(cv.ImageFile.FileName);

                if (!IsValidExtension(extension))
                    return View(cv);
               
                var newFileName = Guid.NewGuid() + extension;
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                var fullPath = Path.Combine(directory, newFileName);

                DeleteOldImageLocally(cvOldVersion); //Radera gamla bilden lokalt

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    await cv.ImageFile.CopyToAsync(fs);
                }

                cvOldVersion.ImageFile = cv.ImageFile;
                cvOldVersion.ImageFilePath = "/CvImages/" + newFileName;
            }          

            //Tilldela nya värdena från ViewModel objektet till det trackade Cvt från db

            //Experiences
            for(int i = 0; i < cv.Experiences.Count; i++)
            {
                if (cvOldVersion.Experiences.Count < cv.Experiences.Count)
                    cvOldVersion.Experiences.Add(new());

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
            for (int i = 0; i < cv.Skills.Count; i++)
            {
                if (cvOldVersion.Skills.Count < cv.Skills.Count)
                    cvOldVersion.Skills.Add(new());

                cvOldVersion.Skills[i].ASkill = cv.Skills[i].ASkill;
                cvOldVersion.Skills[i].Description = cv.Skills[i].Description;
                cvOldVersion.Skills[i].Date = cv.Skills[i].Date;
            }

            //Interests
            for (int i = 0; i < cv.Interests.Count; i++)
            {
                if (cvOldVersion.Interests.Count < cv.Interests.Count)
                    cvOldVersion.Interests.Add(new());

                cvOldVersion.Interests[i].InterestName = cv.Interests[i].InterestName;
            }


            //Certificates
            for (int i = 0; i < cv.Certificates.Count; i++)
            {
                if (cvOldVersion.Certificates.Count < cv.Certificates.Count)
                    cvOldVersion.Certificates.Add(new());

                cvOldVersion.Certificates[i].CertName = cv.Certificates[i].CertName;
            }


            //PersonalCharacteristics
            for (int i = 0; i < cv.PersonalCharacteristics.Count; i++)
            {
                if (cvOldVersion.PersonalCharacteristics.Count < cv.PersonalCharacteristics.Count)
                    cvOldVersion.PersonalCharacteristics.Add(new());
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
        public async Task<IActionResult> DeleteCv(Cv cv)
         {
            try
            {
                _context.Cvs.Remove(cv);
                DeleteOldImageLocally(cv);
                await _context.SaveChangesAsync();
            }catch(Exception e)
            {
                return NotFound(e);
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}
