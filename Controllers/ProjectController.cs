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

        public ProjectController(UserManager<User> u, CVBuddyContext c) : base(u, c)
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
            User? user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                proj.UsersInProject.Add(user);//Lägg till user i projektet som participant för participant count
            }
            else
            {
                return NotFound("det var jag");
            }


            await _context.Projects.AddAsync(proj);//Lägg till proj i projects i snapshot
            await _context.SaveChangesAsync(); //Serialisera snapshot, proj läggs till i Db innan vi använder dess proj.Pid, eftersom att den är 0 oavsett vad, 
                                               //eftersom att Pid tilldelas först när den har serialiserats till Db

            var userId = _userManager.GetUserId(User); //Hämtar användarens id
                                                       //var cvId = await _context.Cvs.Where(cvs => cvs.UserId == userId).Select(cv => cv.Cid).FirstOrDefaultAsync(); //Hämtar användarens Cv via användarens id
            var projId = await _context.Projects.Where(createdProject => createdProject.Pid == proj.Pid).Select(project => project.Pid).FirstOrDefaultAsync(); //Hämtar tillbaka proj som skapades

            //await _context.CvProjects.AddAsync(new CvProject //Lägg till CvProject direkt i DbSet
            //{
            //    ProjId = projId,
            //    CvId = cvId

            //});

            //await _context.SaveChangesAsync(); //Serialisera utan konfikt, kan möjligen inte behövas. Har ej prövat, gäster har kommit

            await _context.ProjectUsers.AddAsync(new ProjectUser //Lägg till ProjectUsers direkt i DbSet
            {
                ProjId = projId,
                UserId = userId!,
                IsOwner = true
            });

            await _context.SaveChangesAsync();//Sista serialiseringen, och nu ska allt ha värden i rätt ordning
            #region comments
            //FUNKAR, najs. Felet var, Ändringarna som gjordes var, att allt behövde göra i en speciell ordning. Tilldela värden till proj innan Post metod.
            //I Post metod har inte proj ett Pid än. Lägg till proj i Dbset. Serialiser via save changes. Ett Pid tilldelas. 
            //Hämta användares id som samt cvs id som förr. Men hämta även samma projs Pid som serialiserades nyss.
            //Nu har alla variabler som behövs värden och kan därmed tilldelas till nya mellantabell objekt direkt i sina respektive DbSet tabeller, (save changes emellan dem för säkerhets skull)
            //Till sist save changes för resterande Dbset tabell. Allt sparat. Fungerar perfekt.

            //var userId = _userManager.GetUserId(User);
            //var cvId = _context.Cvs.Where(u => u.UserId == userId).Select(u => u.Cid).FirstOrDefault();
            //Console.WriteLine($"cvId: {cvId}, userId: {userId.ToString()}, proj.ProjId: {proj.Pid}, proj.PublisDate: {proj.PublisDate}, proj.StartDate: {proj.StartDate}, proj.Enddate: {proj.Enddate}, ");
            //proj.CvProjects.Add(new CvProject
            //{

            //    CvId = cvId,
            //    ProjId = proj.Pid
            //});
            //Console.WriteLine($"cvId: {cvId}, userId: {userId.ToString()}, proj.ProjId: {proj.Pid}, proj.PublisDate: {proj.PublisDate}, proj.StartDate: {proj.StartDate}, proj.Enddate: {proj.Enddate}, ");
            //proj.ProjectUsers.Add(new ProjectUser
            //{
            //    UserId = userId,
            //    ProjId = proj.Pid
            //});
            //Console.WriteLine($"cvId: {cvId}, userId: {userId.ToString()}, proj.ProjId: {proj.Pid}, proj.PublisDate: {proj.PublisDate}, proj.StartDate: {proj.StartDate}, proj.Enddate: {proj.Enddate}, ");


            //await _context.Projects.AddAsync(proj);
            //Console.WriteLine($"cvId: {cvId}, userId: {userId.ToString()}, proj.ProjId: {proj.Pid}, proj.PublisDate: {proj.PublisDate}, proj.StartDate: {proj.StartDate}, proj.Enddate: {proj.Enddate}, ");

            //Måste ha transaktion här annars kan det bli fel i databasen


            //await _context.Projects.AddAsync(proj);
            //await _context.SaveChangesAsync();

            //var cvProject = new CvProject
            //{
            //    CvId = cvId,
            //    ProjId = proj.Pid
            //};
            //await _context.CvProjects.AddAsync(cvProject);
            //await _context.SaveChangesAsync();

            //var userProject = new ProjectUser
            //{
            //    ProjId = proj.Pid,
            //    UserId = userId
            //};
            //await _context.ProjectUsers.AddAsync(userProject);
            //await _context.SaveChangesAsync();
            #endregion

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
