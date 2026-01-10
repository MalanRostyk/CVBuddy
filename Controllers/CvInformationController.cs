using Castle.Components.DictionaryAdapter.Xml;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CVBuddy.Controllers
{
    public class CvInformationController : BaseController
    {
        public CvInformationController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm) { }


        //---------------------BuildCv------------------------------------------BuildCv---------------------

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuildCv()//TILL BUILD CV-SIDAN ----->>>>
        {
            return View(new CvVM());
        }

        [HttpPost]
        public async Task<IActionResult> BuildCv(CvVM cvVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(cvVM);
                Cv cv = new Cv
                {
                    ImageFilePath = cvVM.ImageFile!.Name, //ImageFile kan inte vara null, eftersom att den är [Required], stoppas av Modelstate.IsValid
                    UserId = _userManager.GetUserId(User)
                };

                cv.Education = new();

                //if (cvVM.ImageFile == null || cvVM.ImageFile.Length == 0)
                //{
                //    ModelState.AddModelError("ImageFile", "Please upload an image");
                //    ViewBag.eror = "Please upload an image";
                //    return View(cvVM);
                //}

                var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                Directory.CreateDirectory(uploadeFolder);

                var ext = Path.GetExtension(cvVM.ImageFile.FileName);//null

                //if (!IsValidExtension(ext))
                //    return View(cv);

                var fileName = Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(uploadeFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await cvVM.ImageFile.CopyToAsync(stream);
                }
                cv.ImageFilePath = "/CvImages/" + fileName;

                await _context.Cvs.AddAsync(cv);
                await _context.SaveChangesAsync();

                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not create Cv, encountered an error while saving to database" });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an unexpected error while trying to create your Cv." });
            }
        }

        //---------------------ReadCv------------------------------------------ReadCv---------------------


        [HttpGet]
        public async Task<IActionResult> ReadCv(int? Cid) //måste heta exakt samma som asp-route-Cid="@item.OneCv.Cid". Detta Cid är Cid från det Cv som man klickar på i startsidan
        {
            Cv? cv;
            try
            {
                if (Cid.HasValue)//Om man klickade på ett cv i Index, följer ett Cid med via asp-route-Cid, men om man klickar på My Cv(har ej asp-route...) så körs else blocket, eftersom inget Cid följer med
                {

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

                    if (cv == null)
                        throw new NullReferenceException("Could not find Cv");

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
                        return RedirectToAction("Login", "Account");//För att både inloggade och utloggade ska använder samma action metod
                    }
                    else
                    {
                        cv = await GetLoggedInUsersCvAsync();
                        //if (cv?.OneUser == null) <---- Var osäker på om OneUser va onödig att ha med här 
                        //    throw new NullReferenceException(""); 

                        if (cv == null)
                                return RedirectToAction("BuildCv");


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

                return View(cv);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (OperationCanceledException)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "A database operation was canceled while incrementing this Cvs read counter" });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error retrieving Cv and additional information. " });
            }


        }

        //---------------------UpdateCv------------------------------------------UpdateCv---------------------


        [HttpGet]
        public async Task<IActionResult> UpdateCv()
        {
            try
            {
                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                CvVM cvVM = new CvVM
                {
                    Cid = cv.Cid,
                    Skills = cv.Skills,
                    Education = cv.Education,
                    Experiences = cv.Experiences,
                    Certificates = cv.Certificates,
                    PersonalCharacteristics = cv.PersonalCharacteristics,
                    PublishDate = cv.PublishDate,
                    Interests = cv.Interests,
                    ImageFilePath = cv.ImageFilePath,
                    ReadCount = cv.ReadCount,
                    UserId = cv.UserId
                };

                return View(cvVM);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an unexpected error while getting your cv from the database." });
            }
        }


        //---------------------Image---------------------Image---------------------Image---------------------HÄÄÄÄR !!!!!!!!!!!!!!!!!!!!!!!


        [HttpGet]
        public async Task<IActionResult> UpdateImage()
        {
            var cvVM = await UsersCvToCvVM();
            return View(cvVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(CvVM cvVM)
        {
            try
            {
                var cv = await GetLoggedInUsersCvAsync();
                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cvVM.ImageFilePath = cv.ImageFilePath;

                if (!ModelState.IsValid)
                {
                    //ModelState.AddModelError(nameof(cvVM.ImageFile), "Please upload an image");
                    //foreach (var entry in ModelState)
                    //{
                    //    Console.WriteLine($"FIELD: {entry.Key}");
                    //    Console.WriteLine($"  AttemptedValue: {entry.Value.AttemptedValue}");

                    //    foreach (var error in entry.Value.Errors)
                    //    {
                    //        Console.WriteLine($"  ❌ {error.ErrorMessage}");
                    //    }
                    //}
                    return View("UpdateCv", await UsersCvToCvVM());//UsersCvToCvVM() eftersom att cvVMs properties är null
                                                                   //Så vi måste returnera ett cvVM med värden för att förse
                                                                   //UpdateCv view model med värden
                }

                var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
                Directory.CreateDirectory(uploadeFolder);

                var ext = Path.GetExtension(cvVM.ImageFile!.FileName);//null

                var fileName = Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(uploadeFolder, fileName);

                DeleteOldImageLocally(cv);

                cv.ImageFile = cvVM.ImageFile;
                cv.ImageFilePath = "/CvImages/" + fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await cv.ImageFile!.CopyToAsync(stream);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not update Cv, encountered an error while saving to database" });
            }
            catch (ArgumentException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }


            
        }

        //---------------------DeleteCv------------------------------------------DeleteCv---------------------


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteCv(int Cid)
        {

            ViewBag.Headline = "Delete Cv";
            ViewBag.WarningMessage = "Are you sure you wan't to delete your Cv? This will permanently delete your Cv but" +
                ", none of the projects you created will be automatically connected to your new Cvs. You will have to find them and participate in them again"; //I felmeddelandet visas vad planen för projekten är
            //Cv cv = _context.Cvs.Find(Cid); //Ska inte använda Find för att annars får man inte med relaterade rader till Cv!!!!!!
            Cv? cv = await GetLoggedInUsersCvAsync();


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
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error while trying to delete your cv, saving the changes failed." });
            }
            catch (ArgumentNullException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not find Cv." });

            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error while trying to delete your cv, saving the changes failed." });
            }
            return RedirectToAction("Index", "Home");
        }

        //---------------------Education------------------------------------------Education---------------------


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddEducation(int eid)//GLÖM EJ KNAPPAR BORT NÄR MAN SKAPOAR CV
        {

            //Om model som håller properties med komplexa objekt, Include + FirstOrDefaultAsync + null check
            //Om model med ej komplexa objekt som properties, FindAsync + null ckeck

            //FINNS INGEN DBSET FÖR EDUCATION
            //var edu = await _context.Education.FindAsync(eid); Annars ska detta funka
            /*var cv = await _context.Cvs.FindAsync(eid);*/ //Funkar ej, cv håller komplexa objekt
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();//Funkar och är korrekt, metoden anvnder Include + FirstOrDefaultAsync och null ckeck görs här

                var education = await _context.Education.FindAsync(eid);

                if (education == null)
                    throw new NullReferenceException("No Cv was found");

                //var edu = cv.Education;

                EducationVM eduVM = new EducationVM
                {
                    Univeristy = education.Univeristy,
                    UniProgram = education.UniProgram,
                    UniDate = education.UniDate,

                    HighSchool = education.HighSchool,
                    HSProgram = education.HSProgram,
                    HSDate = education.HSDate
                };

                return View(eduVM);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });

            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an unexpected error processing your request." });

            }

        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(EducationVM evm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(evm);

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cv.Education.Univeristy = evm.Univeristy;
                cv.Education.UniProgram = evm.UniProgram;
                cv.Education.UniDate = evm.UniDate;

                cv.Education.HighSchool = evm.HighSchool;
                cv.Education.HSProgram = evm.HSProgram;
                cv.Education.HSDate = evm.HSDate;

                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not save changes to database!" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }

        }

        //---------------------Certificate------------------------------------------Certificate---------------------

        [HttpGet]
        public async Task<IActionResult> AddCertificate()
        {

            return View(new CertificateVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddCertificate(CertificateVM cvm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(cvm);

                Certificate certificate = new Certificate
                {
                    CertName = cvm.CertName
                };

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cv.Certificates.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not update Cv, encountered an error while saving to add certificate, the database could not save your changes" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to add a certificate." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> UpdateCertificate(int certId)
        {

            try
            {
                //var userCv = await GetLoggedInUsersCvAsync();

                //if (userCv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var certificate = userCv.Certificates.FirstOrDefault(c => c.CertId == certId);
                //if (certificate == null)
                //{
                //    return RedirectToAction("Index", "Home");
                //}

                var certificate = await _context.Certificates.FindAsync(certId);
                if (certificate == null)
                    throw new NullReferenceException("Certificate could not be found.");

                var certificateVm = new CertificateVM
                {
                    CertId = certId,
                    CertName = certificate.CertName
                };
                return View(certificateVm);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateCertificate(CertificateVM cvm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(cvm);

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                var certToUpdate = cv.Certificates.FirstOrDefault(c => c.CertId == cvm.CertId);
                if (certToUpdate != null)
                {
                    certToUpdate.CertName = cvm.CertName;
                    await _context.SaveChangesAsync();
                }
                return View("UpdateCv", await UsersCvToCvVM());
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error trying to save your changes to the database" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> DeleteCertificate(int certId)
        {


            try
            {
                //var userCv = await GetLoggedInUsersCvAsync();

                //if (userCv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var certificate = userCv.Certificates.FirstOrDefault(c => c.CertId == certId);

                var certificate = await _context.Certificates.FindAsync(certId);

                if (certificate == null)
                    throw new NullReferenceException("Certificate could not be found.");

                _context.Certificates.Remove(certificate);
                await _context.SaveChangesAsync();
                return View("UpdateCv", await UsersCvToCvVM());
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error in the database while trying to delete certificate" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }


        }


        //---------------------PersonalCharacteristic------------------------------------------PersonalCharacteristic---------------------

        [HttpGet]
        public async Task<IActionResult> AddPersonalCharacteristic()
        {

            return View(new PersonalCharacteristicVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddPersonalCharacteristic(PersonalCharacteristicVM pvm)
        {

            if (!ModelState.IsValid)
                return View(pvm);

            try
            {

                PersonalCharacteristic persChar = new PersonalCharacteristic
                {
                    CharacteristicName = pvm.CharacteristicName
                };

                var cv = await GetLoggedInUsersCvAsync();
                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cv.PersonalCharacteristics.Add(persChar);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error trying to save your changes to the database" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdatePersonalCharacteristic(int pcId)
        {
            try
            {

                //var userCv = await GetLoggedInUsersCvAsync();

                //if (userCv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var personalCharacteristic = userCv.PersonalCharacteristics.FirstOrDefault(c => c.PCId == pcId);

                var personalCharacteristic = await _context.PersonalCharacteristics.FindAsync(pcId);

                if (personalCharacteristic == null)
                    throw new NullReferenceException("Personal characteristic could not be found.");

                var personalCharacteristicVm = new PersonalCharacteristicVM
                {
                    PCId = personalCharacteristic.PCId,
                    CharacteristicName = personalCharacteristic.CharacteristicName
                };
                return View(personalCharacteristicVm);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePersonalCharacteristic(PersonalCharacteristicVM pvm)
        {

            try
            {

                if (!ModelState.IsValid)
                    return View(pvm);

                var cv = await GetLoggedInUsersCvAsync();
                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                var personalCharacteristic = cv.PersonalCharacteristics.FirstOrDefault(pc => pc.PCId == pvm.PCId);

                if (personalCharacteristic == null)
                    throw new NullReferenceException("Personal characteristic could not be found.");

                personalCharacteristic.CharacteristicName = pvm.CharacteristicName;
                await _context.SaveChangesAsync();

                //if (personalCharacteristic != null)
                //{
                //    personalCharacteristic.CharacteristicName = pvm.CharacteristicName;
                //    await _context.SaveChangesAsync();
                //}
                return View("UpdateCv", await UsersCvToCvVM());
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error trying to save your changes to the database" });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Unexpected error while trying to process your request." });
            }


        }


        [HttpGet]
        public async Task<IActionResult> DeletePersonalCharacteristic(int pcId)
        {
            try
            {
                //var userCv = await GetLoggedInUsersCvAsync();

                //if (userCv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var personalCharacteristic = userCv.PersonalCharacteristics.FirstOrDefault(c => c.PCId == pcId);
                var personalCharacteristic = await _context.PersonalCharacteristics.FindAsync(pcId);

                if (personalCharacteristic == null)
                    throw new NullReferenceException("Personal characteristic could not be found.");

                _context.PersonalCharacteristics.Remove(personalCharacteristic);
                await _context.SaveChangesAsync();
                return View("UpdateCv", await UsersCvToCvVM());
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not delete personal characteristic due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error deleting personal characteristic" });
            }
        }




        //---------------------Experience------------------------------------------Experience---------------------

        [HttpGet]
        public async Task<IActionResult> AddExperience()
        {

            return View(new ExperienceVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddExperience(ExperienceVM evm)
        {
            if (!ModelState.IsValid)
                return View(evm);


            try
            {
                Experience exp = new Experience
                {
                    Title = evm.Title,
                    Description = evm.Description,
                    Company = evm.Company,
                    StartDate = evm.StartDate ?? new DateTime(19000101),
                    EndDate = evm.EndDate
                };

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");
                cv.Experiences.Add(exp);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not add experience due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error adding experience." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateExperience(int exid)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var experience = cv.Experiences.FirstOrDefault(e => e.Exid == exid);

                var experience = await _context.Experiences.FindAsync(exid);

                if (experience == null)
                    throw new NullReferenceException("Users experience could not be found.");

                ExperienceVM exVM = new ExperienceVM
                {
                    Exid = experience.Exid,
                    Title = experience.Title,
                    Description = experience.Description,
                    Company = experience.Company,
                    StartDate = experience.StartDate,
                    EndDate = experience.EndDate
                };
                return View(exVM);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to process your request." });
            }


        }

        [HttpPost]
        public async Task<IActionResult> UpdateExperience(ExperienceVM exVM)
        {

            try
            {
                if (!ModelState.IsValid)
                    return View(exVM);


                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                var experience = cv.Experiences.FirstOrDefault(exp => exp.Exid == exVM.Exid);

                if (experience == null)
                    throw new NullReferenceException("Experience could not be found.");

                experience.Title = exVM.Title;
                experience.Description = exVM.Description;
                experience.Company = exVM.Company;
                experience.StartDate = exVM.StartDate ?? new DateTime(19000101);
                experience.EndDate = exVM.EndDate;

                await _context.SaveChangesAsync();

                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not edit experience due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error updating experience." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> DeleteExperience(int exid)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var experience = cv.Experiences.FirstOrDefault(e => e.Exid == exid);

                var experience = await _context.Experiences.FindAsync(exid);

                if (experience == null)
                    throw new NullReferenceException("Users experience could not be found.");

                _context.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not delete experience due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error processing your request." });
            }

        }


        //---------------------Skill------------------------------------------Skill---------------------

        [HttpGet]
        public async Task<IActionResult> AddSkill()
        {

            return View(new SkillVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(SkillVM svm)
        {
            if (!ModelState.IsValid)
                return View(svm);

            Skill skill = new Skill
            {
                ASkill = svm.ASkill,
                Description = svm.Description,
                Date = svm.Date
            };
            try
            {
                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cv.Skills.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not add skill due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error processing your request." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> UpdateSkill(int sid)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var skill = cv.Skills.FirstOrDefault(s => s.Sid == sid);

                var skill = await _context.Skills.FindAsync(sid);

                if (skill == null)
                    throw new NullReferenceException("Skill could not be found.");

                SkillVM sVM = new SkillVM
                {
                    Sid = skill.Sid,
                    ASkill = skill.ASkill,
                    Description = skill.Description,
                    Date = skill.Date
                };
                return View(sVM);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to update skill, changes could not be saved." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateSkill(SkillVM sVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(sVM);

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                var skill = cv.Skills.FirstOrDefault(s => s.Sid == sVM.Sid);

                if (skill == null)
                    throw new NullReferenceException("Skill could not be found.");

                skill.ASkill = sVM.ASkill;
                skill.Description = sVM.Description;
                skill.Date = sVM.Date;

                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not update skill due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to update skill, changes could not be saved." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> DeleteSkill(int sid)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var skill = cv.Skills.FirstOrDefault(e => e.Sid == sid);

                var skill = await _context.Skills.FindAsync(sid);

                if (skill == null)
                    throw new NullReferenceException("Skill could not be found.");
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not delete skill due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to delete skill, changes could not be saved." });
            }
        }


        //---------------------Interest------------------------------------------Interest---------------------


        [HttpGet]
        public async Task<IActionResult> AddInterest()
        {
            return View(new InterestVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddInterest(InterestVM ivm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(ivm);

                Interest interest = new Interest
                {
                    InterestName = ivm.InterestName
                };

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                cv.Interests.Add(interest);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not add interest due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to add interest, changes could not be saved." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateInterest(int interestId)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var interest = cv.Interests.FirstOrDefault(i => i.InterestId == interestId);

                var interest = await _context.Interests.FindAsync(interestId);

                if (interest == null)
                    throw new NullReferenceException("Interest could not be found.");

                InterestVM iVM = new InterestVM
                {
                    InterestId = interest.InterestId,
                    InterestName = interest.InterestName
                };

                return View(iVM);
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to update interest, changes could not be saved." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateInterest(InterestVM iVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(iVM);

                var cv = await GetLoggedInUsersCvAsync();

                if (cv == null)
                    throw new NullReferenceException("Users Cv could not be found.");

                var interest = cv.Interests.FirstOrDefault(i => i.InterestId == iVM.InterestId);

                if (interest == null)
                    throw new NullReferenceException("Interest could not be found.");

                interest.InterestName = iVM.InterestName;

                await _context.SaveChangesAsync();

                return View("UpdateCv", await UsersCvToCvVM());
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not update interest due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to update interest, changes could not be saved." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteInterest(int interestId)
        {
            try
            {
                //var cv = await GetLoggedInUsersCvAsync();

                //if (cv == null)
                //    throw new NullReferenceException("Users Cv could not be found.");

                //var interest = cv.Interests.FirstOrDefault(i => i.InterestId == interestId);
                ////var cv = await GetLoggedInUsersCvAsync();
                ////var interest = cv.Interests.FirstOrDefault(i => i.InterestId == interestId);

                var interest = await _context.Interests.FindAsync(interestId);

                if (interest == null)
                    throw new NullReferenceException("Interest could not be found");
                _context.Interests.Remove(interest);
                await _context.SaveChangesAsync();
                return RedirectToAction("UpdateCv");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not delete interest due to an error saving changes to database." });
            }
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an internal error trying to delete interest, changes could not be saved." });
            }

        }

        //--------------------PRIVATE HELPERS---------------------------------------------------

        private async Task<CvVM> UsersCvToCvVM()
        {
            var cv = await GetLoggedInUsersCvAsync();

            CvVM cvVM = new CvVM
            {
                Cid = cv.Cid,
                Skills = cv.Skills,
                Education = cv.Education,
                Experiences = cv.Experiences,
                Certificates = cv.Certificates,
                PersonalCharacteristics = cv.PersonalCharacteristics,
                PublishDate = cv.PublishDate,
                Interests = cv.Interests,
                ImageFilePath = cv.ImageFilePath,
                ReadCount = cv.ReadCount,
                UserId = cv.UserId
            };

            return cvVM;
        }


        private async Task<Cv?> GetLoggedInUsersCvAsync() //Kan returnera null, och kan inte använda FindAsync, eftersom att entiteten håller komplexa objekt
        {
            if (!User.Identity!.IsAuthenticated)
                return null;

            var userId = _userManager.GetUserId(User);

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

            if (cv == null)
                return null;

            cv.UsersProjects = await GetProjectsUserHasParticipatedIn(userId!);

            return cv;
        }
        //private async Task<List<Project>> GetProjectsUserHasParticipatedIn(string userId)
        //{
        //    List<Project> projectList = await _context.ProjectUsers
        //        .Where(pu => pu.UserId == userId)
        //        .Join(
        //        _context.Projects,
        //        pu => pu.ProjId,
        //        p => p.Pid,
        //        (pu, p) => p)
        //        .ToListAsync(); ;
        //    return projectList;
        //}
        private async Task<List<Project>> GetProjectsUserHasParticipatedIn(string userId)//MED READCV!!!!!!!
        {
            var IsAuthenticated = User.Identity!.IsAuthenticated;

            List<Project> projectList = await _context.Projects
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Where(p =>
                    p.ProjectUsers.Any(pu => pu.UserId == userId) &&
                    p.ProjectUsers.Any(pu => pu.IsOwner && !pu.User.IsDeactivated) &&
                    (IsAuthenticated || !p.ProjectUsers.FirstOrDefault(pu => pu.IsOwner)!.User.HasPrivateProfile))
                .ToListAsync();

            return projectList;
        }
        private void DeleteOldImageLocally(Cv cvOld)
        {
            string[]? cvOldFileImageNameArray = null;
            if (cvOld.ImageFilePath != null)
            {
                cvOldFileImageNameArray = cvOld.ImageFilePath.Split("/");

                if (cvOldFileImageNameArray.Length != 0)
                {

                    string oldCvFileName = cvOldFileImageNameArray[cvOldFileImageNameArray.Length - 1];

                    string finalCvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages", oldCvFileName);

                    if (!System.IO.File.Exists(finalCvFilePath))
                        throw new ArgumentException("The old image could not be deleted since it was not found. " +
                            "Attempted too look for it at: " + finalCvFilePath + " Did you move it?");

                    System.IO.File.Delete(finalCvFilePath);
                }
            }
        }
    }
}
