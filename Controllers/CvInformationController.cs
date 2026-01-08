using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CVBuddy.Controllers
{
    public class CvInformationController : BaseController
    {
        public CvInformationController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm) { }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuildCv()//TILL BUILD CV-SIDAN ----->>>>
        {
            return View(new CvVM());
        }

        //--------------------CREATE---------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> BuildCv(CvVM cvVM)
        {
            if (!ModelState.IsValid)
                return View(cvVM);

            Cv cv = new Cv
            {
                ImageFilePath = cvVM.ImageFile.Name,
                UserId = _userManager.GetUserId(User)
            };

            cv.Education = new();

            if (cvVM.ImageFile == null || cvVM.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image");
                ViewBag.eror = "Please upload an image";
                return View(cvVM);
            }

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

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AddInterest()
        {
            
            return View(new InterestVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddInterest(InterestVM ivm)
        {
            if (!ModelState.IsValid)
                return View(ivm);

            Interest interest = new Interest
            {
                InterestName = ivm.InterestName
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.Interests.Add(interest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AddCertificate()
        {
            
            return View(new CertificateVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddCertificate(CertificateVM cvm)
        {
            if (!ModelState.IsValid)
                return View(cvm);

            Certificate certificate = new Certificate
            {
                CertName = cvm.CertName
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

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

            PersonalCharacteristic persChar = new PersonalCharacteristic
            {
                CharacteristicName = pvm.CharacteristicName
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.PersonalCharacteristics.Add(persChar);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

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

            var cv = await GetLoggedInUsersCvAsync();
            cv.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

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

            Experience exp = new Experience
            {
                Title = evm.Title,
                Description = evm.Description,
                Company = evm.Company,
                StartDate = evm.StartDate,
                EndDate = evm.EndDate
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.Experiences.Add(exp);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddEducation()
        {
            return View(new EducationVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(EducationVM evm)
        {
            if (!ModelState.IsValid)
                return View(evm);

            var cv = await GetLoggedInUsersCvAsync();

            cv.Education.Univeristy = evm.Univeristy;
            cv.Education.UniProgram = evm.UniProgram;
            cv.Education.UniDate = evm.UniDate;

            cv.Education.HighSchool = evm.HighSchool;
            cv.Education.HSProgram = evm.HSProgram;
            cv.Education.HSDate = evm.HSDate;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        //--------------------UPDATE---------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> UpdateCv()
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

            return View(cvVM);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateImage()
        {
            var cvVM = await UsersCvToCvVM();
            return View(cvVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(CvVM cvVM)
        {

            //if(!ModelState.IsValid)
            //    return View(cvVM);

            

            if (cvVM.ImageFile == null || cvVM.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image");
                ViewBag.eror = "Please upload an image";
                return View(cvVM);
            }

            var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
            Directory.CreateDirectory(uploadeFolder);

            var ext = Path.GetExtension(cvVM.ImageFile.FileName);//null

            //if (!IsValidExtension(ext))
            //    return View(cv);

            var fileName = Guid.NewGuid().ToString() + ext;

            var filePath = Path.Combine(uploadeFolder, fileName);

            var cv = await GetLoggedInUsersCvAsync();
            DeleteOldImageLocally(cv);
            cvVM.ImageFilePath = cv.ImageFilePath;
            cv.ImageFile = cvVM.ImageFile;
            cv.ImageFilePath = "/CvImages/" + fileName;
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await cv.ImageFile!.CopyToAsync(stream);
            }

            

            
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }



        //--------------------DELETE---------------------------------------------------


        //--------------------PRIVATE---------------------------------------------------
        
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
            if (cv != null)
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

        private async Task<string> GetImageFilePathForCvImage(Cv cv) //Ska tilldelas cv.ImageFilePath
        {
            var uploadeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CvImages");
            Directory.CreateDirectory(uploadeFolder);

            var ext = Path.GetExtension(cv.ImageFile.FileName);//null

            var fileName = Guid.NewGuid().ToString() + ext;

            var filePath = Path.Combine(uploadeFolder, fileName);
            
            string imageFilePath = "/CvImages/" + fileName;

            return imageFilePath;
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
            }
            catch (Exception e)
            {
                throw;
            }

            return false;
        }
    }
}
