using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class ProfileController : BaseController
    {
        public ProfileController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [HttpGet]
        public async Task<IActionResult> ReadProfile(string userId)
        {
            try
            {
                var user = await _context.Users
                .Include(u => u.OneAddress)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Experiences)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Education)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Skills)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Certificates)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.Interests)
                .Include(u => u.OneCv)
                .ThenInclude(cv => cv.PersonalCharacteristics)
                .FirstOrDefaultAsync(u => u.Id.Equals(userId));



                if (user == null)
                    return NotFound("Users Cv could not be found.");


                ViewBag.HasOneAdress = user.OneAddress != null;
                ViewBag.HasOneCv = user.OneCv != null; //HasOneCv för att om vi planerar på att man
                                                       //ska kunna komma till Profil sidan inte bara från en persons cv
                                                       //så är det inte garanterat att de har cv

                if (ViewBag.HasOneCv)
                {
                    ViewBag.HasExperience = user.OneCv!.Experiences.Count() > 0;
                    ViewBag.HasHighSchool = user.OneCv!.Education?.HighSchool != null;
                    ViewBag.HasUniveristy = user.OneCv!.Education?.Univeristy != null;
                    ViewBag.HasSkills = user.OneCv!.Skills.Count() > 0;
                    ViewBag.HasCertificates = user.OneCv!.Certificates.Count() > 0;
                    ViewBag.HasInterests = user.OneCv!.Interests.Count() > 0;
                    ViewBag.HasPersonalCharacteristics = user.OneCv!.PersonalCharacteristics.Count() > 0;
                    //Om använderen bara har en bild för sitt cv så syns en ensam rubrik, ta bort den med detta
                    ViewBag.HasCvWithOnlyImage = false;
                    if (ViewBag.HasExperience ||
                        ViewBag.HasHighSchool ||
                        ViewBag.HasUniveristy ||
                        ViewBag.HasSkills ||
                        ViewBag.HasCertificates ||
                        ViewBag.HasInterests ||
                        ViewBag.HasPersonalCharacteristics)
                    {
                        ViewBag.HasCvWithOnlyImage = true;
                    }

                }

                var projects = await _context.Projects
                    .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                    .Where(u => u.ProjectUsers.Any(pu => pu.UserId == userId))
                    .ToListAsync();

                var isAuthenticated = User.Identity!.IsAuthenticated;

                var projectVMs = new List<ProjectVM>();


                foreach (var project in projects)
                {
                    var owner = project.ProjectUsers
                        .FirstOrDefault(pu => pu.IsOwner);

                    if (owner == null)
                        continue;

                    if (owner.User.IsDeactivated)
                        continue;

                    if (!isAuthenticated && owner.User.HasPrivateProfile)
                        continue;

                    var usersInProject = project.ProjectUsers
                        .Select(pu => pu.User).ToList();

                    var activeUsers = usersInProject.Where(u => !u.IsDeactivated).ToList();

                    var relation = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);

                    projectVMs.Add(new ProjectVM
                    {
                        Project = project,
                        Owner = owner,
                        Relation = relation,
                        IsUserInProject = true,
                        ActiveUsers = isAuthenticated ? activeUsers : activeUsers.Where(u => !u.HasPrivateProfile).ToList()
                    });
                }

                List<Project> projList = await _context.ProjectUsers
                    .Where(pu => pu.UserId == userId)
                    .Join(
                    _context.Projects,
                    pu => pu.ProjId,
                    p => p.Pid,
                    (pu, p) => p)
                    .ToListAsync();

                ViewBag.HasJoinedProjects = projList.Count() > 0;

                ViewBag.IsMyProfile = false;
                if (User.Identity!.IsAuthenticated)
                {
                    var loggedInUserId = _userManager.GetUserId(User);
                    ViewBag.IsMyProfile = loggedInUserId == userId;

                }

                ProfileViewModel profViewModel = new();

                profViewModel.ViewUser = user;
                profViewModel.Cv = user.OneCv;
                profViewModel.Projects = projectVMs;


                return View(profViewModel);
            }
            catch(Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
            
        }
    }
}
