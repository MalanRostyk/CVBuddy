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

        private Cv ConvertToCv(CvVM cvVM)
        {
            Cv cv = new();
            cv.Cid = cvVM.Cid;
            cv.Skills = cvVM.Skills;
            cv.Education = cvVM.Education;
            cv.Experiences = cvVM.Experiences;
            cv.Certificates = cvVM.Certificates;
            cv.PersonalCharacteristics = cvVM.PersonalCharacteristics;
            cv.PublishDate = cvVM.PublishDate;
            cv.Interests = cvVM.Interests;
            cv.ImageFilePath = cvVM.ImageFilePath;
            cv.ImageFile = cvVM.ImageFile;
            cv.ReadCount = cvVM.ReadCount;
            cv.UserId = cvVM.UserId;
            cv.OneUser = cvVM.OneUser;
            cv.UsersProjects = cvVM.UsersProjects;

            return cv;
        }

        private CvVM ConvertToCvVM(Cv cv)
        {
            CvVM cvVM = new();
            cvVM.Cid = cv.Cid;
            cvVM.Skills = cv.Skills;
            cvVM.Education = cv.Education;
            cvVM.Experiences = cv.Experiences;
            cvVM.Certificates = cv.Certificates;
            cvVM.PersonalCharacteristics = cv.PersonalCharacteristics;
            cvVM.PublishDate = cv.PublishDate;
            cvVM.Interests = cv.Interests;
            cvVM.ImageFilePath = cv.ImageFilePath;
            cvVM.ImageFile = cv.ImageFile;
            cvVM.ReadCount = cv.ReadCount;
            cvVM.UserId = cv.UserId;
            cvVM.OneUser = cv.OneUser;
            cvVM.UsersProjects = cv.UsersProjects;

            return cvVM;
        }

        private bool IsValidExtension(string extension)
        {
            string ext = extension.ToLower();
            if (ext == ".png" || ext == ".jpg" || ext == ".jfif" || ext == ".webp")
                return true;
            return false;
        }

        private async Task<CvVM> GetLoggedInUsersCvVMAsync()
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

            return ConvertToCvVM(cv!);
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

        private bool DeleteOldImageLocally(CvVM cvOld)
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
            return View(new CvVM());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCv(CvVM cvVM)
        {
            //var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (ModelState.IsValid)
                return View(cvVM);
            try
            {
                if (cvVM.ImageFile == null || cvVM.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Please upload an image");
                    ViewBag.eror = "Please upload an image";
                    return View(cvVM);
                }

                var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                Directory.CreateDirectory(uploadeFolder);

                var ext = Path.GetExtension(cvVM.ImageFile.FileName);//null

                if (!IsValidExtension(ext))
                    return View(cvVM);

                var fileName = Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(uploadeFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await cvVM.ImageFile.CopyToAsync(stream);
                }
                cvVM.ImageFilePath = "/CvImages/" + fileName;

                //Tilldela user id till cv för realtion
                cvVM.UserId = _userManager.GetUserId(User);

                Cv cv = new();

                cv.Cid = cvVM.Cid;
                cv.Skills = cvVM.Skills;
                cv.Education = cvVM.Education;
                cv.Experiences = cvVM.Experiences;
                cv.Certificates = cvVM.Certificates;
                cv.PersonalCharacteristics = cvVM.PersonalCharacteristics;
                cv.PublishDate = cvVM.PublishDate;
                cv.Interests = cvVM.Interests;
                cv.ImageFilePath = cvVM.ImageFilePath;
                cv.ImageFile = cvVM.ImageFile;
                cv.ReadCount = cvVM.ReadCount;
                cv.UserId = cvVM.UserId;
                cv.OneUser = cvVM.OneUser;
                cv.UsersProjects = cvVM.UsersProjects;

                await _context.Cvs.AddAsync(cv);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                //Om transaktionen inte lyckades, tas tillagda bilden bort lokalt här i samband med en rollback
                DeleteOldImageLocally(cvVM);

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
                bool hasEducation = false;
                var cvEdu = cv?.Education;

                if (cvEdu?.HighSchool != null || cvEdu?.HSProgram != null || cvEdu?.HSDate != null)
                    hasEducation = true;

                if (cvEdu?.Univeristy != null || cvEdu?.UniProgram != null || cvEdu?.UniDate != null)
                    hasEducation = true;

                if (hasEducation)
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
        public async Task<IActionResult> UpdateCv(CvVM cvVM)
        {
            if (!ModelState.IsValid)
                return View(cvVM);

            var cvOldVersion = await GetLoggedInUsersCvAsync();

            if (cvOldVersion == null)
                return NotFound();

            //----------------------------------------------------------------------------------------------------------------------------cvOldVersion.IsPrivate = cv.IsPrivate;
            cvOldVersion.ReadCount = cvVM.ReadCount;
            cvOldVersion.UserId = cvVM.UserId;
            
            if(cvVM.ImageFile != null)
            {

                if (!IsValidFileSize(cvVM.ImageFile.Length))
                    return View(cvVM);

                //var oldImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");

                //accept räcker inte måste validera extension på server nivå

                var extension = Path.GetExtension(cvVM.ImageFile.FileName);

                if (!IsValidExtension(extension))
                    return View(cvVM);
               
                var newFileName = Guid.NewGuid() + extension;
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                var fullPath = Path.Combine(directory, newFileName);

                DeleteOldImageLocally(ConvertToCvVM(cvOldVersion)); //Radera gamla bilden lokalt

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    await cvVM.ImageFile.CopyToAsync(fs);
                }

                cvOldVersion.ImageFile = cvVM.ImageFile;
                cvOldVersion.ImageFilePath = "/CvImages/" + newFileName;
            }          

            //Tilldela nya värdena från ViewModel objektet till det trackade Cvt från db

            //Experiences
            for(int i = 0; i < cvVM.Experiences.Count; i++)
            {
                if (cvOldVersion.Experiences.Count < cvVM.Experiences.Count)
                    cvOldVersion.Experiences.Add(new());

                cvOldVersion.Experiences[i].Title = cvVM.Experiences[i].Title;
                cvOldVersion.Experiences[i].Description = cvVM.Experiences[i].Description;
                cvOldVersion.Experiences[i].Company = cvVM.Experiences[i].Company;
                cvOldVersion.Experiences[i].StartDate = cvVM.Experiences[i].StartDate;
                cvOldVersion.Experiences[i].EndDate = cvVM.Experiences[i].EndDate;
            }

            //Education
            cvOldVersion.Education.HighSchool = cvVM.Education.HighSchool;
            cvOldVersion.Education.HSProgram = cvVM.Education.HSProgram;
            cvOldVersion.Education.HSDate = cvVM.Education.HSDate;

            cvOldVersion.Education.Univeristy = cvVM.Education.Univeristy;
            cvOldVersion.Education.UniProgram = cvVM.Education.UniProgram;
            cvOldVersion.Education.UniDate = cvVM.Education.UniDate;

            //Skills
            for (int i = 0; i < cvVM.Skills.Count; i++)
            {
                if(cvOldVersion.Skills.Count < cvVM.Skills.Count)
                    cvOldVersion.Skills.Add(new());
                
                cvOldVersion.Skills[i].ASkill = cvVM.Skills[i].ASkill;
                cvOldVersion.Skills[i].Description = cvVM.Skills[i].Description;
                cvOldVersion.Skills[i].Date = cvVM.Skills[i].Date;
            }

            //Interests
            for (int i = 0; i < cvVM.Interests.Count; i++)
            {
                if (cvOldVersion.Interests.Count < cvVM.Interests.Count)
                    cvOldVersion.Interests.Add(new());

                cvOldVersion.Interests[i].InterestName = cvVM.Interests[i].InterestName;
            }


            //Certificates
            for (int i = 0; i < cvVM.Certificates.Count; i++)
            {
                if (cvOldVersion.Certificates.Count < cvVM.Certificates.Count)
                    cvOldVersion.Certificates.Add(new());

                cvOldVersion.Certificates[i].CertName = cvVM.Certificates[i].CertName;
            }


            //PersonalCharacteristics
            for (int i = 0; i < cvVM.PersonalCharacteristics.Count; i++)
            {
                if (cvOldVersion.PersonalCharacteristics.Count < cvVM.PersonalCharacteristics.Count)
                    cvOldVersion.PersonalCharacteristics.Add(new());
                cvOldVersion.PersonalCharacteristics[i].CharacteristicName = cvVM.PersonalCharacteristics[i].CharacteristicName;
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
                DeleteOldImageLocally(ConvertToCvVM(cv));
                await _context.SaveChangesAsync();
            }catch(Exception e)
            {
                return NotFound(e);
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}
