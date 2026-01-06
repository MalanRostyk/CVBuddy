using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class MessageController : BaseController
    {

        public MessageController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [HttpGet]
        public IActionResult SendMsg(string userId)//Får inte med userId till POST sedan, därför är modelstate invalid och reciever null
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
            {
                return View(msg);
            }
            await _context.AddAsync(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Messages()
        {
            var userId = _userManager.GetUserId(User);
            List<Message> msgList = await _context.Messages
                .Where(m => m.RecieverId == userId)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();

            ViewBag.HasMesseges = msgList.Count > 0;

            return View(msgList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsRead(Message message, int mid)
        {
            Message? oldState = await _context.Messages
                .Where(m => m.Mid == mid).FirstOrDefaultAsync();

            oldState!.IsRead = message.IsRead;
            await _context.SaveChangesAsync();
            return RedirectToAction("Messages");
        }

        [HttpGet]
        public async Task<IActionResult> ReadMsg(int mid)
        {
            var msg = await _context.Messages
                .Where(m => m.Mid == mid).FirstOrDefaultAsync();
            if (msg == null)
                return NotFound("Message could not be found.");
            return View(msg);
        }
    }
}
