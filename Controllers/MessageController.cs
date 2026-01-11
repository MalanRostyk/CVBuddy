using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class MessageController : BaseController
    {

        public MessageController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {
        }

        [HttpGet]
        public IActionResult SendMsg(string userId)
        {
            MessageVM msg = new();
            msg.RecieverId = userId;

            return View(msg);
        }

        [HttpPost]
        public async Task<IActionResult> SendMsg(MessageVM msgVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(msgVM);

                Message msg = new Message
                {
                    Mid = msgVM.Mid,
                    Sender = msgVM.Sender,
                    MessageString = msgVM.MessageString,
                    SendDate = msgVM.SendDate,
                    IsRead = msgVM.IsRead,
                    RecieverId = msgVM.RecieverId
                };

                await _context.AddAsync(msg);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not send message, internal error while trying to save your message to the database."});
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Messages()
        {
            try
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
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Internal error retrieveing your messages from the database."});
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsRead(Message message, int mid)
        {
            try
            {
                var oldState = await _context.Messages.FindAsync(mid);

                oldState!.IsRead = message.IsRead;
                await _context.SaveChangesAsync();

                return RedirectToAction("Messages");
            }
            catch (DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Could not save changes to read status, internal error while trying to save your changes." });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadMsg(int mid)
        {
            
            try
            {
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
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Error finding your message." });
            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteMessageConfirm(int mid)
        {
            try
            {
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Mid == mid);
                if (message == null)
                    throw new NullReferenceException("This message could not be found.");

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
            catch (NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message});

            }
            catch (Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error deleting the message" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessage(MessageVM mVM)
        {            
            try
            {

                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Mid == mVM.Mid);

                if (message == null)
                    throw new NullReferenceException("The message you want to delete could not be found.");
                
                //Radera
                _context.Messages.Remove(message);

                //Savechangesasync
                await _context.SaveChangesAsync();

                return RedirectToAction("Messages");
            }
            catch(DbUpdateException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Failed to delete the message, due to an issue saving your changes."});
            }
            catch(NullReferenceException e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = e.Message });
            }
            catch(Exception e)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error deleting the message" });
            }
        }
    }
}
