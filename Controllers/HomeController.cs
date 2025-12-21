using System.Diagnostics;
using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVBuddy.Controllers
{
    public class HomeController : Controller
    {
        

        public HomeController()
        {
            
        }
        //[Authorize]
        public IActionResult Index()
        {
            User user = new();
            return View(user);
        }

        
    }
}
