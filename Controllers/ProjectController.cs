using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Transactions;

namespace CVBuddy.Controllers
{
    public class ProjectController : HomeController
    {

        public ProjectController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProject()
        {
            var userId = _userManager.GetUserId(User);

            var projects = await _context.Projects
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
                .ToListAsync();

            var myProjects = projects
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId)).ToList();//&& pu.IsOwner

            var otherProjects = projects
                .Where(p => !p.ProjectUsers.Any(pu => pu.UserId == userId)).ToList();//&& pu.IsOwner

            var vm = new ProjectIndexViewModel
            {
                MyProjects = myProjects,
                OtherProjects = otherProjects
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProject()
        {
            ViewBag.ProjectCreateHeadline = "Create a Project";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("inte jaag");
            //var userId = _userManager.GetUserId(User);

            //Lägga till sig själv i projektet som deltagare innan det skapas
            Project newProj = new();

            newProj.UsersInProject.Add(user);
            return View(newProj); //Att lägga till sig själv isom participant i ett projekt när det skapas funkar inte eftersom att Project.UsersInproject inte Serialiseras
                                  //Vill ej ändra model innan vi har mergeat tillsammans
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(Project proj)
        {
            if(!ModelState.IsValid)
                return View(proj);

            var userId = _userManager.GetUserId(User); //Hämtar användarens id

            //var cvId = await _context.Cvs
            //  .Where(cvs => cvs.UserId == userId)
            //  .Select(cv => cv.Cid).FirstOrDefaultAsync(); //Hämtar användarens Cv via användarens id

            //var projId = await _context.Projects
            //    .Where(createdProject => createdProject.Pid == proj.Pid)
            //    .Select(project => project.Pid).FirstOrDefaultAsync(); //Hämtar tillbaka proj som skapades, kan köra direkt proj.Pid

            await _context.Projects.AddAsync(proj);
            await _context.SaveChangesAsync();
            
            await _context.ProjectUsers.AddAsync(new ProjectUser //Lägg till ProjectUsers direkt i DbSet
            {
                ProjId = proj.Pid,
                UserId = userId!,
                IsOwner = true
            });

            //await _context.CvProjects.AddAsync(new CvProject //Lägg till CvProject direkt i DbSet
            //{
            //    ProjId = projId,
            //    CvId = cvId

            //});
            //Lägg till proj i projects i snapshot
            //Serialisera snapshot, proj läggs till i Db innan vi använder dess proj.Pid, eftersom att den är 0 oavsett vad, 
            //eftersom att Pid tilldelas först när den har serialiserats till Db

            await _context.SaveChangesAsync();//Sista serialiseringen, och nu ska allt ha värden i rätt ordning
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProject(int id)
        {
            var userId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Pid == id && p.ProjectUsers.Any(pu => pu.UserId == userId));

            if (project == null)
                return NotFound();

            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProject(Project toUpdate)
        {
            if (!ModelState.IsValid)
                return View(toUpdate);

            var userId = _userManager.GetUserId(User);

            var newProj = await _context.Projects
                .Include(pu => pu.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Pid == toUpdate.Pid && p.ProjectUsers.Any(pu => pu.UserId == userId));

            if (newProj == null)
                return NotFound();

            newProj.Title = toUpdate.Title;
            newProj.Description = toUpdate.Description;
            newProj.StartDate = toUpdate.StartDate;
            newProj.Enddate = toUpdate.Enddate;
            newProj.UsersInProject = toUpdate.UsersInProject;
            newProj.PublishDate = toUpdate.PublishDate;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProjectDetails(int PUId)
        {
            var userId = _userManager.GetUserId(User);

            var projectuser = await _context.ProjectUsers
                .Include(pu => pu.Project)
                .FirstOrDefaultAsync(pu => pu.PUId == PUId);

            if (projectuser == null) return NotFound();

            bool alreadyJoined = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjId == projectuser.ProjId && pu.UserId == userId);

            if (alreadyJoined)
                return RedirectToAction("GetProject");



            await _context.ProjectUsers.AddAsync(new ProjectUser
            {
                ProjId = projectuser.ProjId,
                UserId = userId,
                IsOwner = false
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
