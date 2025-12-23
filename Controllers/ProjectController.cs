using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

        [HttpPost]
        public IActionResult CreateProject(Project proj)
        {
            return View();
        }
    }
}
