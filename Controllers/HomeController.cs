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
    public class HomeController : BaseController
    {

        public HomeController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        public async Task<IActionResult> Index()
        {
            //Ingen ska se, något från konton som är deactivated
            var users = await _context.Users //Mindre kod gör samma utan valideringen, OM NÅGOT SAKNAS FÖR USER, SÅ MÅSTE DET INKLUDERAS
                                             //, IdentityUsers fält är inkluderade genom Arvet, men bara dem som är Mappade
                .Where(u => u.IsDeactivated != true)
                .Include(u => u.OneCv)
                .Include(u => u.ProjectUsers)
                .ThenInclude(pu => pu.Project)
                .ToListAsync();

                                                                    //if (!User.Identity!.IsAuthenticated)
                                                                    //{
                                                                    //    users = users
                                                                    //        .Where(u => u.OneCv?.IsPrivate != true)
                                                                    //        .ToList();
                                                                    //}

            //Om man är utloggad så ska man inte se privata profiler
            if (!User.Identity!.IsAuthenticated)
            {
                users = users
                    .Where(u => u.HasPrivateProfile != true)
                    .ToList();
            }

            users = users
                .OrderByDescending(u => u.OneCv?.PublishDate)
                .Take(9)
                .ToList();

            ViewBag.CvsExists = false;
            foreach (var user in users)
            {
                if (user.OneCv != null)
                {
                    ViewBag.CvsExists = true;
                }
            }



            var usersCv = await GetLoggedInUsersCvAsync();
            ViewBag.HasCv = usersCv != null;


            //Behövde göra kontrollen i view istället eftersom att kontrollen
            //görs en gång här, måste ha 1 kontroll för varje cv mot 1 userId
            //Därför ViewBag.userId
            //ViewBag.CanSend = true;

            ViewBag.userId = _userManager.GetUserId(User);


            //if (User.Identity!.IsAuthenticated)//Om användaren är inloggad
            //{
            //    if (ViewBag.HasCv)//och har ett cv
            //    {
            //        var userId = _userManager.GetUserId(User);
            //        if(usersCv!.OneUser!.Id == userId)//kolla om det är inloggade användarens cv
            //            ViewBag.CanSend = false;

            //    }
            //}

            //if (User.Identity!.IsAuthenticated)
            //{
            //    var userId = _userManager.GetUserId(User);
            //    if (ViewBag.HasCv)
            //    {
            //        if (userId != usersCv!.UserId)
            //            ViewBag.CanSend = false;
            //    }
            //}

            ViewBag.CvIndexHeadline = "Recent Cvs";

            var userId = _userManager.GetUserId(User);//ändring av project utskriften enda ner
            var isAuthenticated = User.Identity!.IsAuthenticated;

            var projList = await _context.Projects
                //.Where(p => p.Enddate == null)
                .Include(p => p.ProjectUsers)
                .ThenInclude(p => p.User)
                //.OrderByDescending(p => p.PublishDate)
                //.Take(10)
                .ToListAsync();

            var filteredProjects = new List<ProjectVM>();

            foreach(var project in projList)
            {
                var owner = project.ProjectUsers.FirstOrDefault(pu => pu.IsOwner);
                if (owner == null || owner.User.IsDeactivated)
                    continue;

                if(!isAuthenticated && owner.User.HasPrivateProfile)
                    continue;

                var usersInProject = project.ProjectUsers.Select(pu => pu.User).ToList();
                var activeUsers = usersInProject.Where(u => !u.IsDeactivated).ToList();
                var relation = userId == null 
                    ? null 
                    : project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);

                filteredProjects.Add(new ProjectVM
                {
                    Project = project,
                    UsersInProject = usersInProject,
                    ActiveUsers = isAuthenticated
                        ? activeUsers
                        : activeUsers.Where(u => !u.HasPrivateProfile).ToList(),
                    Owner = owner,
                    Relation = relation,
                    IsUserInProject = relation != null
                });
            }

            filteredProjects = filteredProjects
                .Where(pvm => pvm.Project.Enddate == null)
                .OrderByDescending(pvm => pvm.Project.PublishDate)
                .Take(10)
                .ToList();

            var vm = new HomeIndexViewModel
            {
                UserList = users,
                ProjectList = filteredProjects,
            };

            return View(vm);
        }

        private async Task<Cv> GetLoggedInUsersCvAsync()
        {

            var userId = "";
            Cv? cv;
            
            if (User.Identity!.IsAuthenticated)
            {
                userId = _userManager.GetUserId(User);
                cv = await _context.Cvs
                   .Include(cv => cv.Education)
                   .Include(cv => cv.Experiences)
                   .Include(cv => cv.Skills)
                   .Include(cv => cv.Certificates)
                   .Include(cv => cv.PersonalCharacteristics)
                   .Include(cv => cv.Interests)
                   .Include(cv => cv.OneUser)
                   .ThenInclude(oneUser => oneUser!.ProjectUsers)
                   .FirstOrDefaultAsync(cv => cv.UserId == userId); //Kan göra cv till null ändå
                    return cv;
            }
            //Cv? cv = await _context.Cvs----------------------------- byttes ut mot den nedan vid MANUAL MERGE
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


            //if (cv == null)
            //    NotFound();
            return null;
            
        }

    }
}
