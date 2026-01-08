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
            MessageVM msg = new();
            msg.RecieverId = userId;
            ViewBag.WillEnterName = !User.Identity!.IsAuthenticated;

            return View(msg);
        }
        [HttpPost]
        public async Task<IActionResult> SendMsg(MessageVM msgVM)
        {
            ViewBag.WillEnterName = !User.Identity!.IsAuthenticated;

            if (!ModelState.IsValid)
            {
                return View(msgVM);
            }

            Message msg = new();

            msg.Mid = msgVM.Mid;
            msg.Sender = msgVM.Sender;
            msg.MessageString = msgVM.MessageString;
            msg.SendDate = msgVM.SendDate;
            msg.IsRead = msgVM.IsRead;
            msg.RecieverId = msgVM.RecieverId;

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

        [HttpGet]
        public async Task<IActionResult> DeleteMessageConfirm(int mid)
        {
            return View(mid);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessage(int mid)
        {
            //Undantagshantering ska göras för exceptionella situationer, det ska inte funka som en if sats för att fånga null värden här och var
            //utan det ska hanteras med kontroller, alltså en if-sats. Och då skriver vi ut ett informativt felmeddelande som ska ses av användare.
            //Exempelvis som här nedan, kontroller ska användas för att validera inputs(bortse från att int mid är ett routeat id). Hittas inte det som efterfrågas
            //så meddelar vi användaren om det. Det är alltså FÖRVÄNTADE SCENARION. Men när det gäller undantagshantering så innebär det att
            //att man ska fånga verkliga fel och ge begripliga och beskrivande felmeddelanden istället för att låta applikationen krascha
            try
            {
                //Hitta meddelandet med mid
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Mid == mid);

                
                if (message == null)
                    return NotFound("The message you want to delete could not be found.");
                
                //Radera
                _context.Messages.Remove(message);

                //Savechangesasync
                await _context.SaveChangesAsync();

                //return RedirectToAction IEnumerable<Message> som tillhör användaren
                var userId = _userManager.GetUserId(User);
                List<Message> usersMessages = await _context.Messages.Where(m => m.RecieverId == userId).ToListAsync();
                
                return RedirectToAction("Messages", usersMessages);
            }
            catch(Exception e)
            {
                return StatusCode(500, "An issue occured while trying to remove the message.");//StatusCode 500 betyder internal error.
            }
            
        }

    }
}
