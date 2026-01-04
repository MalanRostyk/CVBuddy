using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class MessageController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly CVBuddyContext _context;
        public MessageController(UserManager<User> u, CVBuddyContext c)
        {
            _userManager = u;
            _context = c;
        }


        [HttpGet]
        public IActionResult SendMsg(string userId)
        {
            Message msg = new();
            msg.RecieverId = userId;
            ViewBag.WillEnterName = false;
            if (!User.Identity!.IsAuthenticated)
            {
                ViewBag.WillEnterName = true;
            }
            return View(msg);
        }
        [HttpPost]
        public async Task<IActionResult> SendMsg(Message msg)
        {
            if (!ModelState.IsValid)
                return NotFound("Dumbass vet du inte hur man skickar saker eller");

            await _context.AddAsync(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Messages()
        {
            var userId = _userManager.GetUserId(User);
            List<Message> msgList = _context.Messages
                .Where(m => m.RecieverId == userId)
                .OrderByDescending(m => m.SendDate)
                .ToList();

            ViewBag.HasMessages = msgList.Count > 0;
            return View(msgList);
        }

        [HttpGet]
        public IActionResult ReadMsg(int mid)
        {
            var msg = _context.Messages
                .Where(m => m.Mid == mid).FirstOrDefault();
            if (msg == null)
                return NotFound("Message could not be found.");
            return View(msg);
        }
    }
}
