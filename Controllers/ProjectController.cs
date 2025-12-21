using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class ProjectController : Controller
    {
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
    }
}
