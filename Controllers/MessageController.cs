using CVBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CVBuddy.Controllers
{
    public class MessageController : BaseController
    {

        public MessageController(UserManager<User> u, CVBuddyContext c) : base(u, c)
        {
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
            ViewBag.WillEnterName = false;
            if (!User.Identity!.IsAuthenticated)
            {
                ViewBag.WillEnterName = true;
            }
            if (!ModelState.IsValid)
                return View(msg);

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

            ViewBag.HasMesseges = msgList.Count > 0;

            return View(msgList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsRead(Message message, int mid)
        {
            Message? oldState = _context.Messages
                .Where(m => m.Mid == mid).FirstOrDefault();

            oldState!.IsRead = message.IsRead;
            await _context.SaveChangesAsync();
            return RedirectToAction("Messages");
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
