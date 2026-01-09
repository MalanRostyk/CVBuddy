using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
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

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> Messages()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    List<Message> msgList = await _context.Messages
        //        .Where(m => m.RecieverId == userId)
        //        .OrderByDescending(m => m.SendDate)
        //        .ToListAsync();

        //    ViewBag.HasMesseges = msgList.Count > 0;

        //    return View(msgList);
        //}

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> Messages()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    List<Message> msgList = await _context.Messages
        //        .Where(m => m.RecieverId == userId)
        //        .OrderByDescending(m => m.SendDate)
        //        .ToListAsync();

        //    List<MessageVM> messageVMList = new();
        //    foreach(var message in msgList)
        //    {
        //        messageVMList.Add(new MessageVM
        //        {
        //            Mid = message.Mid,
        //            Sender = message.Sender,
        //            MessageString = message.MessageString,
        //            SendDate = message.SendDate,
        //            IsRead = message.IsRead,
        //            RecieverId = message.RecieverId
        //        });
        //    }

        //    ViewBag.HasMesseges = msgList.Count > 0;

        //    return View(msgList);
        //}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Messages()
        {
            var userId = _userManager.GetUserId(User);

            List<MessageVM> mVMList = await _context.Messages
                .Where(m => m.RecieverId == userId)
                .OrderByDescending(m => m.SendDate)
                .Select(m => new MessageVM
                {
                    Mid = m.Mid,
                    Sender = m.Sender,
                    MessageString = m.MessageString,
                    SendDate = m.SendDate,
                    IsRead = m.IsRead,
                    RecieverId = m.RecieverId
                })
                .ToListAsync();

            return View(mVMList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsRead(Message message, int mid)
        {
            //Message? oldState = await _context.Messages
            //    .Where(m => m.Mid == mid).FirstOrDefaultAsync();

            var oldState = await _context.Messages.FindAsync(mid);

            oldState!.IsRead = message.IsRead;
            await _context.SaveChangesAsync();
            return RedirectToAction("Messages");

        }

        [HttpGet]
        public async Task<IActionResult> ReadMsg(int mid)
        {
            //var msg = await _context.Messages
            //    .Where(m => m.Mid == mid).FirstOrDefaultAsync();
            var msg = await _context.Messages
                .FindAsync(mid);

            if (msg == null)
                return NotFound("Message could not be found.");

            MessageVM mVM = new MessageVM
            {
                Mid = msg.Mid,
                Sender = msg.Sender,
                MessageString = msg.MessageString,
                SendDate = msg.SendDate,
                IsRead = msg.IsRead,
                RecieverId = msg.RecieverId
            };
            return View(mVM);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessageConfirm(int mid)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Mid == mid);

            if (message == null)
                return NotFound("This message could not be found.");

            MessageVM mVM = new MessageVM
            {
                Mid = message.Mid,
                Sender = message.Sender,
                MessageString = message.MessageString,
                SendDate = message.SendDate,
                IsRead = message.IsRead,
                RecieverId = message.RecieverId
            };

            return View(mVM);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessage(MessageVM mVM)
        {
            //Undantagshantering ska göras för exceptionella situationer, det ska inte funka som en if sats för att fånga null värden här och var
            //utan det ska hanteras med kontroller, alltså en if-sats. Och då skriver vi ut ett informativt felmeddelande som ska ses av användare.
            //Exempelvis som här nedan, kontroller ska användas för att validera inputs(bortse från att int mid är ett routeat id). Hittas inte det som efterfrågas
            //så meddelar vi användaren om det. Det är alltså FÖRVÄNTADE SCENARION. Men när det gäller undantagshantering så innebär det att
            //att man ska fånga verkliga fel och ge begripliga och beskrivande felmeddelanden istället för att låta applikationen krascha
            try
            {
                //Hitta meddelandet med mid
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Mid == mVM.Mid);

                if (message == null)
                    return NotFound("The message you want to delete could not be found.");
                
                //Radera
                _context.Messages.Remove(message);

                //Savechangesasync
                await _context.SaveChangesAsync();

                //return RedirectToAction IEnumerable<Message> som tillhör användaren
                //var userId = _userManager.GetUserId(User);
                //List<Message> usersMessages = await _context.Messages.Where(m => m.RecieverId == mVM.RecieverId).ToListAsync();

                //return RedirectToAction("Messages", usersMessages);
                return RedirectToAction("Messages");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Fel användarnam/lösenord.");
                //return StatusCode(500, "An issue occured while trying to remove the message.");//StatusCode 500 betyder internal error.
                throw;
            }
            
        }

    }
}
