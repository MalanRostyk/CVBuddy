using Castle.Components.DictionaryAdapter.Xml;
using CVBuddy.Models;
using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        //---------------------UpdateCv------------------------------------------UpdateCv---------------------


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


        //---------------------Image---------------------Image---------------------Image---------------------


        [HttpGet]
        public async Task<IActionResult> UpdateImage()
        {
            var cvVM = await UsersCvToCvVM();
            return View(cvVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(CvVM cvVM)
        {
            var cv = await GetLoggedInUsersCvAsync();
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

            var ext = Path.GetExtension(cvVM.ImageFile.FileName);//null

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

            return RedirectToAction("Index", "Home");
        }


        //---------------------BuildCv------------------------------------------BuildCv---------------------

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





        

        

        

        

        

        //---------------------Education------------------------------------------Education---------------------


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddEducation()//GLÖM EJ KNAPPAR BORT NÄR MAN SKAPOAR CV
        {
            var cv = await GetLoggedInUsersCvAsync();

            EducationVM eduVM = new EducationVM();
            eduVM.Univeristy = cv.Education.Univeristy;
            eduVM.UniProgram = cv.Education.UniProgram;
            eduVM.UniDate = cv.Education.UniDate;

            eduVM.HighSchool = cv.Education.HighSchool;
            eduVM.HSProgram = cv.Education.HSProgram;
            eduVM.HSDate = cv.Education.HSDate;

            return View(eduVM);
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

        //---------------------Certificate------------------------------------------Certificate---------------------

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
        public async Task<IActionResult> UpdateCertificate(int certId)
        {
            var userCv = await GetLoggedInUsersCvAsync();
            var certificate = userCv.Certificates.FirstOrDefault(c => c.CertId == certId);
            if (certificate == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var certificateVm = new CertificateVM
            {
                CertId = certificate.CertId,
                CertName = certificate.CertName
            };
            return View(certificateVm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCertificate(CertificateVM cvm)
        {
            if (!ModelState.IsValid)
                return View(cvm);

            var cv = await GetLoggedInUsersCvAsync();
            var certToUpdate = cv.Certificates.FirstOrDefault(c => c.CertId == cvm.CertId);
            if (certToUpdate != null)
            {
                certToUpdate.CertName = cvm.CertName;
                await _context.SaveChangesAsync();
            }
            return View("UpdateCv", await UsersCvToCvVM());
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCertificate(int certId)
        {
            var userCv = await GetLoggedInUsersCvAsync();
            var certificate = userCv.Certificates.FirstOrDefault(c => c.CertId == certId);
            _context.Certificates.Remove(certificate); 
            await _context.SaveChangesAsync();
            return View("UpdateCv", await UsersCvToCvVM());
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

            PersonalCharacteristic persChar = new PersonalCharacteristic
            {
                CharacteristicName = pvm.CharacteristicName
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.PersonalCharacteristics.Add(persChar);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public int PCId { get; set; }
        [HttpGet]
        public async Task<IActionResult> UpdatePersonalCharacteristic(int pcId)
        {
            var userCv = await GetLoggedInUsersCvAsync();
            var personalCharacteristic = userCv.PersonalCharacteristics.FirstOrDefault(c => c.PCId == pcId);
            if (personalCharacteristic == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var personalCharacteristicVm = new PersonalCharacteristicVM
            {
                PCId = personalCharacteristic.PCId,
                CharacteristicName = personalCharacteristic.CharacteristicName
            };
            return View(personalCharacteristicVm);
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePersonalCharacteristic(PersonalCharacteristicVM pvm)
        {
            if (!ModelState.IsValid)
                return View(pvm);

            var cv = await GetLoggedInUsersCvAsync();
            var personalCharacteristic = cv.PersonalCharacteristics.FirstOrDefault(pc => pc.PCId == pvm.PCId);
            if (personalCharacteristic != null)
            {
                personalCharacteristic.CharacteristicName = pvm.CharacteristicName;
                await _context.SaveChangesAsync();
            }
            return View("UpdateCv", await UsersCvToCvVM());
        }


        [HttpGet]
        public async Task<IActionResult> DeletePersonalCharacteristic(int pcId)
        {
            var userCv = await GetLoggedInUsersCvAsync();
            var personalCharacteristic = userCv.PersonalCharacteristics.FirstOrDefault(c => c.PCId == pcId);
            _context.PersonalCharacteristics.Remove(personalCharacteristic);
            await _context.SaveChangesAsync();
            return View("UpdateCv", await UsersCvToCvVM());
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

            Experience exp = new Experience
            {
                Title = evm.Title,
                Description = evm.Description,
                Company = evm.Company,
                StartDate = evm.StartDate ?? new DateTime(19000101),
                EndDate = evm.EndDate
            };

            var cv = await GetLoggedInUsersCvAsync();
            cv.Experiences.Add(exp);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateExperience(int exid)
        {
            var cv = await GetLoggedInUsersCvAsync();
            var experience = cv.Experiences.FirstOrDefault(e => e.Exid == exid);

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

        [HttpPost]
        public async Task<IActionResult> UpdateExperience(ExperienceVM exVM)
        {
            var cvVMForInvalidState = await UsersCvToCvVM();
            var exVMForInvalidState = cvVMForInvalidState.Experiences.FirstOrDefault(evm => evm.Exid == exVM.Exid);
            if (!ModelState.IsValid)
                return View(exVM);
                

            var cv = await GetLoggedInUsersCvAsync();
            var experience = cv.Experiences.FirstOrDefault(exp => exp.Exid == exVM.Exid);

            experience.Title = exVM.Title;
            experience.Description = exVM.Description;
            experience.Company = exVM.Company;
            experience.StartDate = exVM.StartDate ?? new DateTime(19000101);
            experience.EndDate = exVM.EndDate;

            await _context.SaveChangesAsync();

            return RedirectToAction("UpdateCv", await UsersCvToCvVM());
        }

        [HttpGet]
        public async Task<IActionResult> DeleteExperience(int exid) 
        {
            var cv = await GetLoggedInUsersCvAsync();
            var experience = cv.Experiences.FirstOrDefault(e => e.Exid == exid);
            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();
            return RedirectToAction("UpdateCv", await UsersCvToCvVM());
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

            var cv = await GetLoggedInUsersCvAsync();
            cv.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSkill(int sid)
        {
            var cv = await GetLoggedInUsersCvAsync();
            var skill = cv.Skills.FirstOrDefault(s => s.Sid == sid);

            SkillVM sVM = new SkillVM
            {
                Sid = skill.Sid,
                ASkill = skill.ASkill,
                Description = skill.Description,
                Date = skill.Date
            };
            return View(sVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSkill(SkillVM sVM)
        {

            if (!ModelState.IsValid)
                return View(sVM);

            var cv = await GetLoggedInUsersCvAsync();
            var skill = cv.Skills.FirstOrDefault(s => s.Sid == sVM.Sid);

            skill.ASkill = sVM.ASkill;
            skill.Description = sVM.Description;
            skill.Date = sVM.Date;

            await _context.SaveChangesAsync();
            return RedirectToAction("UpdateCv", sVM);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSkill(int sid)
        {
            var cv = await GetLoggedInUsersCvAsync();
            var skill = cv.Skills.FirstOrDefault(e => e.Sid == sid);
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction("UpdateCv", await UsersCvToCvVM());
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
        public async Task<IActionResult> UpdateInterest(int interestId)
        {
            var cv = await GetLoggedInUsersCvAsync();
            var interest = cv.Interests.FirstOrDefault(i => i.InterestId == interestId);

            InterestVM iVM = new InterestVM
            {
                InterestId = interest.InterestId,
                InterestName = interest.InterestName
            };

            return View(iVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInterest(InterestVM iVM)
        {
            if (!ModelState.IsValid)
                return View(iVM);

            var cv = await GetLoggedInUsersCvAsync();
            var interest = cv.Interests.FirstOrDefault(i => i.InterestId == iVM.InterestId);

            interest.InterestName = iVM.InterestName;

            await _context.SaveChangesAsync();

            return View("UpdateCv", await UsersCvToCvVM());
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

        
        private async Task<Cv> GetLoggedInUsersCvAsync()
        {
            if (!(User.Identity!.IsAuthenticated))
                return new();
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
            if (cv != null)
                cv.UsersProjects = await GetProjectsUserHasParticipatedIn(userId!);
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
