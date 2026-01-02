using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace CVBuddy.Controllers
{
    public class ProjectController : HomeController
    {

        public ProjectController(UserManager<User> u, CVBuddyContext c) : base(u,c)
        {
            
        }

        [HttpGet]
        public IActionResult GetProject()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            ViewBag.ProjectCreateHeadline = "Create a Project";
            return View(new Project());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(Project proj)
        {
            await _context.Projects.AddAsync(proj);//Lägg till proj i projects i snapshot
            await _context.SaveChangesAsync(); //Serialisera snapshot, proj läggs till i Db innan vi använder dess proj.Pid, eftersom att den är 0 oavsett vad, 
            //eftersom att Pid tilldelas först när den har serialiserats till Db

            var userId = _userManager.GetUserId(User); //Hämtar användarens id
            var cvId = await _context.Cvs.Where(cvs => cvs.UserId == userId).Select(cv => cv.Cid).FirstOrDefaultAsync(); //Hämtar användarens Cv via användarens id
            var projId = await _context.Projects.Where(createdProject => createdProject.Pid == proj.Pid).Select(project => project.Pid).FirstOrDefaultAsync(); //Hämtar tillbaka proj som skapades

            await _context.CvProjects.AddAsync(new CvProject //Lägg till CvProject direkt i DbSet
            {
                ProjId = projId,
                CvId = cvId
            });

            await _context.SaveChangesAsync(); //Serialisera utan konfikt, kan möjligen inte behövas. Har ej prövat, gäster har kommit

            await _context.ProjectUsers.AddAsync(new ProjectUser //Lägg till ProjectUsers direkt i DbSet
            {
                ProjId = projId,
                UserId = userId
            });

            await _context.SaveChangesAsync();//Sista serialiseringen, och nu ska allt ha värden i rätt ordning


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


            return RedirectToAction("Index", "Home");
        }
    }
}
