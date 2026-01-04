using Microsoft.AspNetCore.Mvc;
using CVBuddy.Models;

namespace CVBuddy.Controllers
{
    public class MessageController : Controller
    {
        [HttpGet]
        public IActionResult CreateMsg()
        {
            return View(new Message());
        }
    }
}
